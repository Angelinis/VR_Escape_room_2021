using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine.EventSystems;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

using Meta.WitAi;
using Meta.WitAi.Requests;

public class ActivateVoice : MonoBehaviour
{

    public InputActionProperty activateButton;

   [Tooltip("Reference to the current voice service")]
    [SerializeField] private VoiceService _voiceService;
    // Start is called before the first frame update

    [SerializeField] private bool _activateImmediately = false;
    [SerializeField] private bool _deactivateAndAbort = false;

    // Current request
    private VoiceServiceRequest _request;
    private bool _isActive = false;    

    private AudioManager audioManager;

    public AudioClip loadingAudio;
    
    public bool isTraining = false;

    void Start()
    {
        audioManager = AudioManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (activateButton.action.WasPressedThisFrame())
        {
            if (!_isActive)
            {
                Activate();
                Debug.Log("activated");
            }
            else
            {
                Deactivate();
            }
        }
    }

        // Activate depending on settings
    private void Activate()
    {
        if (!_activateImmediately)
        {
            _request = _voiceService.Activate(GetRequestEvents());
        }
        else
        {
            _request = _voiceService.ActivateImmediately(GetRequestEvents());
        }
    }

    // Deactivate depending on settings
    private void Deactivate()
    {
        if (!_deactivateAndAbort)
        {
            _request.DeactivateAudio();
                   if(isTraining){
            audioManager.PlaySFX(2);
        } else {
            audioManager.PlaySFX(6);
        }
        }
        else
        {
            _request.Cancel();
                   if(isTraining){
            audioManager.PlaySFX(2);
        } else {
            audioManager.PlaySFX(6);
        }
        }
    }

    // Get events
    private VoiceServiceRequestEvents GetRequestEvents()
    {
        VoiceServiceRequestEvents events = new VoiceServiceRequestEvents();
        events.OnInit.AddListener(OnInit);
        audioManager.PlayAccessibleDescription(loadingAudio);
        events.OnComplete.AddListener(OnComplete);
        return events;
    }
    // Request initialized
    private void OnInit(VoiceServiceRequest request)
    {
        _isActive = true;
        
        // RefreshActive();
    }
    // Request completed
    private void OnComplete(VoiceServiceRequest request)
    {
        
        _isActive = false;
        if(isTraining){
            audioManager.PlaySFX(2);
        } else {
            audioManager.PlaySFX(6);
        }
        
        // RefreshActive();
    }
}