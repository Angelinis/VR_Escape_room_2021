using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayOnSpawn : MonoBehaviour
{
    private AudioManager audioManager;
    public ActionBasedController xrController;

    private Queue<int> clipQueue = new Queue<int>();
    private Coroutine currentCoroutine;

    void Start()
    {
        audioManager = AudioManager.instance;
        PlayIntro();
    }

    void Update()
    {
        if (xrController.selectAction.action.ReadValue<float>() > 0.5f)
        {
            RepeatIntroductionClips();
        }
    }

    void PlayIntro()
    {
        clipQueue.Clear();
        EnqueueClip(0);
        EnqueueClip(1);
        EnqueueClip(2);
        PlayNextClip();
    }

    void RepeatIntroductionClips()
    {
        clipQueue.Clear();
        EnqueueClip(1);
        EnqueueClip(2);
        PlayNextClip();
    }

    void EnqueueClip(int clipIndex)
    {
        clipQueue.Enqueue(clipIndex);
    }

    void PlayNextClip()
    {
        if (clipQueue.Count > 0)
        {
            int clipIndex = clipQueue.Dequeue();
            currentCoroutine = StartCoroutine(PlayClip(clipIndex, PlayNextClip));
        }
    }

    IEnumerator PlayClip(int clipIndex, System.Action callback)
    {
        if (audioManager != null)
        {
            audioManager.PlayDescription(clipIndex);
            yield return new WaitForSeconds(audioManager.descriptionSource.clip.length);

            callback?.Invoke(); // Invoke the callback to play the next clip
        }
    }
}
