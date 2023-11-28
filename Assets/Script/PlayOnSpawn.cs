using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayOnSpawn : MonoBehaviour
{
    private AudioManager audioManager;
    public ActionBasedController xrController; // Attach the ActionBasedController directly in the Inspector

    // Start is called before the first frame update
    void Start()
    {
        audioManager = AudioManager.instance;

        // Play the introduction when the object is spawned
        PlayIntro();
    }

    // Update is called once per frame
    void Update()
    {
        // Check for button press
        if (xrController.selectAction.action.ReadValue<float>() > 0.5f)
        {
            // Trigger the repeat of the second and third audio clips
            RepeatSecondAndThirdClips();
        }
    }

    void PlayIntro()
    {
        if (audioManager != null)
        {
            audioManager.PlayDescription(0);
            StartCoroutine(WaitForClipEnd(0));
        }
    }

    IEnumerator WaitForClipEnd(int clipIndex)
    {
        yield return new WaitForSeconds(audioManager.descriptionSource.clip.length);

        // Play the next clip after the current one has finished
        if (clipIndex == 0)
        {
            PlaySecondClip();
        }
        else if (clipIndex == 1)
        {
            PlayThirdClip();
        }
        // Add more conditions for additional clips if needed
    }

    void PlaySecondClip()
    {
        if (audioManager != null)
        {
            Debug.Log("Playing the second clip!");
            audioManager.PlayDescription(1); // Adjust the index as needed
            StartCoroutine(WaitForClipEnd(1));
        }
    }

    void PlayThirdClip()
    {
        if (audioManager != null)
        {
            Debug.Log("Playing the third clip!");
            audioManager.PlayDescription(2); // Adjust the index as needed
            // Add StartCoroutine(WaitForClipEnd(2)); if you want to continue the pattern
        }
    }

    void RepeatSecondAndThirdClips()
    {
        // You may need to adjust the logic here based on your specific requirements
        StopAllCoroutines(); // Stop the current audio sequence
        PlaySecondClip();    // Start playing the second clip
        PlayThirdClip();     // Start playing the third clip
    }
}
