using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using System.IO;
using System;
using GoogleTextToSpeech.Scripts.Data;
using GoogleTextToSpeech.Scripts;
using TMPro;

public class ExperimentManager : MonoBehaviour
{
    public string prePrompt;
    public string prompt;
    public Texture2D defaultTexture;
    public GameObject userMenuInterface;
    public GameObject chatInterface;
    public GameObject audioInterface; 
    private string participantCode;
    private int groupExperiment;
    private int languageCode;
    public TMP_InputField participantCodeTMP;
    public TMP_Dropdown groupExperimentTMP;
    public TMP_Dropdown languageCodeTMP;
    public AudioSource audioSource;
    private bool isWaitingForAudioResponse;
    public GeminiManager artificialInteligence;
    private byte[] screenshotBytes;

    [SerializeField] private VoiceScriptableObject[] voices;
    [SerializeField] private TextToSpeech textToSpeech;
    private Action<AudioClip> _audioClipReceived;
    private Action<BadRequestData> _errorReceived;
     private AudioManager audioManager;
    // Start is called before the first frame update

    public TMP_InputField userMessageTMP;
    public TMP_Text botMessageTMP;
    public TMP_Text userRequestTMP;
    private bool recording;

    private AudioClip clip;
    private int screenshotCount = 0;

    private byte[] bytes;
    private byte[] defaultBytesTexture;

    [Header("Time Settings")]
    [SerializeField] private GameObject objectToActivate;
    [SerializeField] private float delayInSeconds = 300f; // 5 minutes

    private Coroutine timerCoroutine;

    private Texture2D renderedTexture;
    private RenderTexture screenTexture;    

    public Camera assignedCamera;
    public TMP_InputField chatInputField;

    void Start()
    {
        audioManager = AudioManager.instance;
        isWaitingForAudioResponse = false;
        defaultBytesTexture = defaultTexture.EncodeToJPG();
    }

    // Update is called once per frame
     void Update() 
    {
        if(audioInterface.activeSelf)
        {
                    if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
        {
            {

                StartRecording();
                StartCoroutine(CaptureScreenshot());

            }
            
        } 
        if (UnityEngine.Input.GetKeyUp(KeyCode.Return))
        {
            byte[] userRecording = StopRecording();
            SendAudioToGemini(userRecording);

        }

        }

        if(chatInterface.activeSelf)
        {
            if (UnityEngine.Input.GetKey(KeyCode.LeftControl) && UnityEngine.Input.GetKeyDown(KeyCode.Return) && !chatInputField.isFocused)
            {
               StartCoroutine(CaptureScreenshot());
            }
            
        }

    }
        
    public void SaveParticipant()
    {
        participantCode = participantCodeTMP.text;
        groupExperiment = groupExperimentTMP.value;
        languageCode = languageCodeTMP.value;
        userMenuInterface.SetActive(false);

        if(groupExperiment == 0)
        {
            chatInterface.SetActive(false);
        } else
        {
            audioInterface.SetActive(false);
        }
    }    

    public void SendMessageToGemini()
    {
        prompt = userMessageTMP.text;

        userRequestTMP.text = prompt;

        userMessageTMP.text = "";

        string finalPrompt = prePrompt + " Visitor prompt: " + prompt;

        string userDataPrompt = participantCode + "_" + groupExperiment;

            try
        {
            if(screenshotBytes == null)
            {
                screenshotBytes = defaultBytesTexture;
            }
                        
            StartCoroutine(artificialInteligence.SendMultimodalDataToGAS(finalPrompt, screenshotBytes, userDataPrompt, (response) => {
                if (response != null)
                {
                    Debug.Log("Response received: " + response);

                    botMessageTMP.text = response;
                }
                else
                {
                    Debug.Log("Error occurred during request.");
                    audioManager.PlaySFX(2);
                }
            }));
            
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to read screenshot file: " + ex.Message);
            audioManager.PlaySFX(2);
        }

        //Cleaning
         screenshotBytes = null; 
    }

       public void StartRecording()
        {
            clip = Microphone.Start(null, false, 90, 16000);
            recording = true;
        }

        private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
        {
            using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
            {
                using (var writer = new BinaryWriter(memoryStream))
                {
                    writer.Write("RIFF".ToCharArray());
                    writer.Write(36 + samples.Length * 2);
                    writer.Write("WAVE".ToCharArray());
                    writer.Write("fmt ".ToCharArray());
                    writer.Write(16);
                    writer.Write((ushort)1);
                    writer.Write((ushort)channels);
                    writer.Write(frequency);
                    writer.Write(frequency * channels * 2);
                    writer.Write((ushort)(channels * 2));
                    writer.Write((ushort)16);
                    writer.Write("data".ToCharArray());
                    writer.Write(samples.Length * 2);

                    foreach (var sample in samples)
                    {
                        writer.Write((short)(sample * short.MaxValue));
                    }
                }
                return memoryStream.ToArray();
            }
        }

public byte[] StopRecording()
{
    int position = Microphone.GetPosition(null);

    if (position <= 0)
    {
        Debug.LogWarning("No microphone data recorded.");
        Microphone.End(null);
        recording = false;
        return null; // ⭐ return null on early exit
    }

    Microphone.End(null);

    position = Mathf.Min(position, clip.samples);

    var samples = new float[position * clip.channels];
    clip.GetData(samples, 0);

    bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);

    recording = false;


    return bytes; // ⭐ return the bytes
}


   public void SendAudioToGemini(byte[] audioBytes)
{
    string finalPrompt = prePrompt;

    string userDataPrompt = participantCode + "_" + groupExperiment;

    VoiceScriptableObject voice = voices[languageCode];


    try
    {
        StartCoroutine(artificialInteligence.SendAudioDataToGAS(finalPrompt, audioBytes, screenshotBytes, userDataPrompt, (response) => {
            if (response != null)
            {
                Debug.Log("Response received: " + response);

                if (!isWaitingForAudioResponse)
                {
                    isWaitingForAudioResponse = true;
                    _errorReceived += ErrorReceived;
                    _audioClipReceived += AudioClipReceived;
                    textToSpeech.GetSpeechAudioFromGoogle(response, voice, _audioClipReceived, _errorReceived);
                }
            }
            else
            {
                Debug.Log("Error occurred during request.");
                audioManager.PlaySFX(2);
            }
        }));
    }
    catch (Exception ex)
    {
        Debug.LogError("Failed to send audio: " + ex.Message);
        audioManager.PlaySFX(2);
    }
}


    private void ErrorReceived(BadRequestData badRequestData)
    {
        Debug.Log($"Error {badRequestData.error.code} : {badRequestData.error.message}");
        audioManager.PlaySFX(2);
    }
    private void AudioClipReceived(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
        isWaitingForAudioResponse = false;
    }

    public void StartTimer()
        {
            // If a timer is already running, stop it first to prevent overlapping timers
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
            }

            timerCoroutine = StartCoroutine(ActivationRoutine());
        }

        private IEnumerator ActivationRoutine()
        {
            yield return new WaitForSeconds(delayInSeconds);

            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Target object is not assigned in the inspector.", this);
            }
        }


    private IEnumerator CaptureScreenshot()
{

    // assignedCamera.gameObject.SetActive(true);
    
    yield return new WaitForEndOfFrame();

    screenshotCount++;
    string screenshotFileName = "/Screenshot_" + screenshotCount + "_" + Screen.width + "X" + Screen.height + ".png";

    string screenShotPath = Application.persistentDataPath + screenshotFileName;


    if (screenTexture == null) 
        screenTexture = new RenderTexture(Screen.width, Screen.height, 24); // 24 for better depth

    assignedCamera.targetTexture = screenTexture;
    assignedCamera.Render();

    // 4. Read pixels
    RenderTexture.active = screenTexture;
    if (renderedTexture == null)
        renderedTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    
    renderedTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
    renderedTexture.Apply();

    // 5. Clean up
    assignedCamera.targetTexture = null;
    RenderTexture.active = null;

    // assignedCamera.gameObject.SetActive(false);


    screenshotBytes = renderedTexture.EncodeToPNG();

    // Debug.Log("Screenshot captured");

    // try
    // {
    //     System.IO.File.WriteAllBytes(screenShotPath, screenshotBytes);
    //     Debug.Log("Screenshot saved to: " + screenShotPath);
    // }
    // catch (Exception ex)
    // {
    //     Debug.LogError("Failed to save screenshot file: " + ex.Message);
    // }


}



}
