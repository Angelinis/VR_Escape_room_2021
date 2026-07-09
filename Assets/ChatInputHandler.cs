using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatInputHandler : MonoBehaviour
{
    public TMP_InputField inputField;
    public ExperimentManager manager;

    void OnEnable()
    {
        // Subscribe to the Submit event
        inputField.onSubmit.AddListener(HandleSubmit);
    }

    void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        inputField.onSubmit.RemoveListener(HandleSubmit);
    }

    private void HandleSubmit(string text)
    {        
        if (!string.IsNullOrEmpty(text))
        {
            manager.SendMessageToGemini();
        }
    }

}