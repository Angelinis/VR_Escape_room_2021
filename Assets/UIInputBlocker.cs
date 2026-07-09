using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

public class UIInputBlocker : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private XRDeviceSimulator deviceSimulator;

    // Debe ser el Transform que realmente mueve el simulador
    // (el Camera dentro del XR Origin, el que tiene el TrackedPoseDriver)
    [SerializeField] private Transform cameraTransform;

    private Vector3 _lastPosition;
    private Quaternion _lastRotation;
    private bool _isBlocking;

    private void OnEnable()
    {
        inputField.onSelect.AddListener(OnFieldSelected);
        inputField.onDeselect.AddListener(OnFieldDeselected);
    }

    private void OnDisable()
    {
        inputField.onSelect.RemoveListener(OnFieldSelected);
        inputField.onDeselect.RemoveListener(OnFieldDeselected);
    }

    private void OnFieldSelected(string text)
    {
        _lastPosition = cameraTransform.position;
        _lastRotation = cameraTransform.rotation;

        deviceSimulator.enabled = false;
        _isBlocking = true;
    }

    private void OnFieldDeselected(string text)
    {
        _isBlocking = false;
        deviceSimulator.enabled = true;
    }

    // Se ejecuta DESPUÉS del TrackedPoseDriver, así que gana la pelea
    private void LateUpdate()
    {
        if (_isBlocking)
        {
            cameraTransform.position = _lastPosition;
            cameraTransform.rotation = _lastRotation;
        }
    }
}