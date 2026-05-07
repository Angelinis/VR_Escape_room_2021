using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LabelSpawner : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject labelPrefab;
    public Vector3 offset = Vector3.zero; // Base offset from the target's pivot

    [Header("Text & Scaling Settings")]
    public float fontSize = 3f;
    public float baseScale = 0.01f;      // Canvas baseline scale
    public float scaleMultiplier = 0.005f; // How fast it grows with distance
    public float minScale = 0.01f;         // Close-up minimum scale
    public float maxScale = 0.02f;         // Far-away maximum scale

    [Header("Label Position & Clipping")]
    [Tooltip("How high above the target's pivot the label's base will appear.")]
    public float labelVerticalOffset = 0.4f;
    [Tooltip("How far in front of its calculated base position the label's visual text will appear, to prevent clipping into objects.")]
    public float labelPushOutDistance = 0.05f; // e.g., 5cm

    [Header("Visibility & Performance")]
    [Tooltip("Perform full visibility (frustum, occlusion) and distance text checks every N frames. 1 = every frame, 5 = every 5th frame.")]
    public int visibilityUpdateInterval = 3;
    [Tooltip("Maximum distance from the camera a label will be visible.")]
    public float maxLabelViewDistance = 50f;
    [Tooltip("Layers that will occlude labels during raycast checks.")]
    public LayerMask occlusionLayerMask = Physics.DefaultRaycastLayers; // By default, everything but Ignore Raycast layer
    [Range(1, 9)] // 1 (center only) up to 9 (center + 8 corners/edges of bounds)
    [Tooltip("Number of raycasts for occlusion check. More rays = more accurate but slightly more expensive.")]
    public int numberOfOcclusionRaycasts = 5; // e.g., center, top, bottom, left, right

    private MetaDataAccessible[] metaObjects;

    private struct LabelData
    {
        public Transform labelTransform;
        public Transform target;
        public TMP_Text textComp;
        public string baseName;
        public float currentDistance;
        public Outline outline;
        public Renderer targetRenderer; // Renderer of the object the label points to
        public Collider targetCollider; // Collider of the object for more robust bounds/raycast
    }

    private List<LabelData> labels = new();
    private int frameCounter = 0;
    private Vector3 camPos; // Cached once per frame
    private Plane[] frustumPlanes = new Plane[6]; // Cached for frustum culling

    void Start()
    {
        if (labelPrefab == null) { Debug.LogError("LabelSpawner: Prefab missing!"); return; }

        if (mainCamera == null) { Debug.LogError("No Camera"); enabled = false; return; }
        
        metaObjects = FindObjectsByType<MetaDataAccessible>(FindObjectsSortMode.None);

        foreach (MetaDataAccessible meta in metaObjects)
        {
            // Ensure target has a renderer or collider for bounds and raycast checks
            Renderer targetRenderer = meta.GetComponent<Renderer>();
            Collider targetCollider = meta.GetComponent<Collider>();

            if (targetRenderer == null && targetCollider == null)
            {
                Debug.LogWarning($"LabelSpawner: Object '{meta.name}' has MetaDataAccessible but no Renderer or Collider. Labels will be less accurate for culling. Adding a default BoxCollider for bounds as a fallback.", meta.gameObject);
                targetCollider = meta.gameObject.AddComponent<BoxCollider>();
                (targetCollider as BoxCollider).isTrigger = true; // Don't want it to affect physics
            }
            
            // Add or get outline component
            Outline outline = meta.gameObject.GetComponent<Outline>();
            if (outline == null)
            {
                outline = meta.gameObject.AddComponent<Outline>();
            }
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineColor = Color.yellow;
            outline.OutlineWidth = 2f;
            outline.enabled = false; // Start disabled, enable with label visibility

            // Spawn Label
            GameObject label = Instantiate(labelPrefab, meta.transform.position + offset, Quaternion.identity);
            label.name = $"Label_{meta.gameObject.name}";

            Canvas canvas = label.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.WorldSpace;
                label.transform.localScale = Vector3.one * baseScale; // Start at base scale
            }

            TMP_Text textComp = label.GetComponentInChildren<TMP_Text>();
            if (textComp == null) { Debug.LogWarning($"No TMP_Text in prefab for {meta.name}", label); Destroy(label); continue; }

            string baseName = string.IsNullOrEmpty(meta.labelOverride) ? meta.gameObject.name : meta.labelOverride;
            textComp.text = baseName;
            textComp.fontSize = fontSize;

            labels.Add(new LabelData
            {
                labelTransform = label.transform,
                target         = meta.transform,
                textComp       = textComp,
                baseName       = baseName,
                outline        = outline,
                targetRenderer = targetRenderer,
                targetCollider = targetCollider
            });
        }
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        camPos = mainCamera.transform.position; // Cache camera position
        frameCounter++;
        bool performFullUpdate = (frameCounter % visibilityUpdateInterval) == 0;

        // Update frustum planes once per frame for all labels
        GeometryUtility.CalculateFrustumPlanes(mainCamera, frustumPlanes);

        // Perform visibility and transformation updates for all labels
        PerformLabelUpdates(performFullUpdate);
    }

    /// <summary>
    /// Updates the position, scale, billboard, and visibility of all labels.
    /// This function is called by LateUpdate and can also be called publicly to force an update.
    /// </summary>
    /// <param name="forceFullUpdate">If true, forces a full visibility check and distance text update regardless of interval.</param>
    private void PerformLabelUpdates(bool forceFullUpdate)
    {
        // Process labels in reverse to safely remove destroyed ones
        for (int i = labels.Count - 1; i >= 0; i--)
        {
            LabelData data = labels[i]; // Get a copy of the struct

            // Handle destroyed targets or labels
            if (data.labelTransform == null || data.target == null)
            {
                if (data.labelTransform != null) Destroy(data.labelTransform.gameObject);
                if (data.outline != null) Destroy(data.outline); // Clean up outline too
                labels.RemoveAt(i);
                continue;
            }
            
            // Previous one
            // Vector3 currentLabelBasePosition = data.target.position + offset + Vector3.up * labelVerticalOffset;
            // --- 1. Calculate Label Base Position (before billboarding and push-out) ---
            Bounds bounds = data.targetRenderer != null ? data.targetRenderer.bounds : data.targetCollider.bounds;

            // Find the exact top-center of the 3D model
            Vector3 topOfMesh = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);

            // Put the label above the top of the mesh
            Vector3 currentLabelBasePosition = topOfMesh + offset + (Vector3.up * labelVerticalOffset);            
            // --- 2. Billboarding ---
            // Rotate the label to face the camera. Its 'forward' will then point away from the camera.
            // Temporarily set position for accurate rotation calculation.
            data.labelTransform.position = currentLabelBasePosition;
            // data.labelTransform.rotation = Quaternion.LookRotation(camPos - currentLabelBasePosition); // Look AT camera, so forward points AWAY from camera.
            Vector3 directionToLook = data.labelTransform.position - mainCamera.transform.position;

            // Look in that direction
            data.labelTransform.rotation = Quaternion.LookRotation(directionToLook);

            // --- 3. Adaptive Scaling ---
            float dist = Vector3.Distance(currentLabelBasePosition, camPos);
            float currentScale = Mathf.Clamp(
                baseScale + dist * scaleMultiplier,
                minScale,
                maxScale
            );
            data.labelTransform.localScale = Vector3.one * currentScale;

            // --- 4. Visibility Culling (Distance, Frustum, Multi-Raycast Occlusion) ---
            bool isVisible = IsLabelEffectivelyVisible(data, dist, currentLabelBasePosition, forceFullUpdate);

           // 1. Toggle Label active state based on visibility
            if (data.labelTransform.gameObject.activeSelf != isVisible)
            {
                data.labelTransform.gameObject.SetActive(isVisible);
            }

            // 2. ALWAYS check the Outline separately, so it never gets stuck!
            if (data.outline != null && data.outline.enabled != isVisible)
            {
                data.outline.enabled = isVisible; 
            }

            // Only perform anti-clipping push-out and text updates for active labels
            if (isVisible)
            {
                // --- 5. Anti-Clipping Push-Out ---
                // Push the label forward along its own (billboarded) forward vector
                // This ensures the visual text plane is always 'labelPushOutDistance' in front of 'currentLabelBasePosition'
                // data.labelTransform.position = currentLabelBasePosition + data.labelTransform.forward * labelPushOutDistance;
                data.labelTransform.position = currentLabelBasePosition - (data.labelTransform.forward * labelPushOutDistance);

                // --- 6. Update Text (Distance display) ---
                if (forceFullUpdate)
                {
                    UpdateDistanceText(data, dist);
                    data.currentDistance = dist;
                    // Since it's a struct, we must save the copy back into the list
                    labels[i] = data; 
                }
            }
            // else: If not visible, its position, rotation, and scale are already set (from steps 1-3) but it's inactive,
            // which is fine as it won't be rendered.
        }
    }

    /// <summary>
    /// Determines if a label should be considered visible based on distance, frustum, and multi-raycast occlusion.
    /// This is the "funnel" (frustum) and "multiple rays to object" logic.
    /// </summary>
private bool IsLabelEffectivelyVisible(LabelData data, float distanceToCamera, Vector3 calculatedLabelBasePosition, bool fullUpdate)
    {
        // Early exit if crucial components are missing
        if (data.target == null || data.labelTransform == null) return false;

        // --- 1. Distance Culling ---
        if (distanceToCamera > maxLabelViewDistance) return false;

        // --- 2. Frustum Culling (Broad-phase) ---
        Bounds targetBounds;
        if (data.targetRenderer != null) targetBounds = data.targetRenderer.bounds;
        else if (data.targetCollider != null) targetBounds = data.targetCollider.bounds;
        else return false; 

        if (!GeometryUtility.TestPlanesAABB(frustumPlanes, targetBounds)) return false;

        // --- 3. Occlusion Culling (Single Raycast: Object to Camera) ---
        Vector3 rayOrigin = targetBounds.center;
        Vector3 toCamera = camPos - rayOrigin;
        float distToCam = toCamera.magnitude;
        Vector3 rayDirection = toCamera / distToCam; // Normalized direction

        bool isVisible = true; // Assume visible until proven blocked

        RaycastHit[] hits = Physics.RaycastAll(rayOrigin, rayDirection, distToCam, occlusionLayerMask);

        foreach (RaycastHit hit in hits)
        {
            // 1. Ignore the target object itself or its children
            if (hit.collider.gameObject == data.target.gameObject || hit.collider.transform.IsChildOf(data.target.transform))
            {
                continue;
            }

            // 2. IGNORE YOUR HEAD! 
            // This ignores the camera and any colliders on your player rig (body, head, etc)
            if (hit.collider.transform.IsChildOf(mainCamera.transform.root))
            {
                continue;
            }

            // If we hit ANYTHING ELSE (a wall, floor, another object), the line of sight is broken.
            isVisible = false;
            break;
        }

        return isVisible;
    }
    /// <summary>
    /// Generates points on or around the target bounds for multi-raycast occlusion.
    /// </summary>


    private void UpdateDistanceText(LabelData data, float dist)
    {
        data.textComp.text = $"{data.baseName}\n<size=80%><color=#aaaaaa>{dist:F1}m</color></size>";
    }

    /// <summary>
    /// Forces an immediate update of all label visibilities based on distance, frustum, and raycast occlusion.
    /// This function implements the requested "hide not seen labels" logic.
    /// </summary>
    public void HideNotSeenLabels()
    {
        Debug.Log("Forcing immediate label visibility update (HideNotSeenLabels).");
        // We calculate frustum planes here as well, in case LateUpdate hasn't run yet or we need an immediate refresh.
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera != null)
        {
            camPos = mainCamera.transform.position;
            GeometryUtility.CalculateFrustumPlanes(mainCamera, frustumPlanes);
        }
        PerformLabelUpdates(true); // Force full update for all labels
    }

    /// <summary>
    /// Shows all labels, regardless of culling checks.
    /// </summary>
    public void ShowAllLabels()
    {
        Debug.Log("Showing all labels.");
        foreach (LabelData data in labels)
        {
            if (data.labelTransform != null)
            {
                data.labelTransform.gameObject.SetActive(true);
                if (data.outline != null) data.outline.enabled = true;
            }
        }
    }

    /// <summary>
    /// Sets the enabled state of all outlines associated with the labels.
    /// </summary>
    // public void SetOutlinesEnabled(bool enabled)
    // {
    //     foreach (LabelData label in labels)
    //     {
    //         if (label.outline != null)
    //             label.outline.enabled = enabled;
    //     }
    // }
}