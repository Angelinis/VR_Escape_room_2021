using System;
using GoogleTextToSpeech.Scripts.Data;
using UnityEngine;
using System.Text.RegularExpressions;
using Input = GoogleTextToSpeech.Scripts.Data.Input;

[System.Serializable]
public class Password
{
    public string key;
}

namespace GoogleTextToSpeech.Scripts
{
    public class TextToSpeech : MonoBehaviour
    {
        private string apiKey;

        public TextAsset jsonFile;

        private Action<string> _actionRequestReceived;
        private Action<BadRequestData> _errorReceived;
        private Action<AudioClip> _audioClipReceived;

        private RequestService _requestService;
        private static AudioConverter _audioConverter;


        void Start()
        {
            Password keyPassword = JsonUtility.FromJson<Password>(jsonFile.text);

            apiKey = keyPassword.key;
        }

private string CleanTextForSpeech(string input)
{
    if (string.IsNullOrEmpty(input)) 
        return string.Empty;

    // 1. Standard string replace (only needs 2 arguments)
    string cleaned = input.Replace("*", "");

    // 2. Regex replace - MUST have 3 arguments: (input, pattern, replacement)
    cleaned = Regex.Replace(cleaned, @"[^\p{L}\p{N}\s\.,;:!\?\-\'""]", "");
    // 3. Regex replace - MUST have 3 arguments: (input, pattern, replacement)
    cleaned = Regex.Replace(cleaned, @"\s+", " ").Trim();

    return cleaned;
}

        public void GetSpeechAudioFromGoogle(string textToConvert, VoiceScriptableObject voice, Action<AudioClip> audioClipReceived,  Action<BadRequestData> errorReceived)
        {

            string cleanedText = CleanTextForSpeech(textToConvert);

            _actionRequestReceived += (requestData => RequestReceived(requestData,audioClipReceived));

            if (_requestService == null)
                _requestService = gameObject.AddComponent<RequestService>();

            if (_audioConverter == null)
                _audioConverter = gameObject.AddComponent<AudioConverter>();

            var dataToSend = new DataToSend
            {
                input =
                    new Input()
                    {
                        text = cleanedText
                    },
                voice =
                    new Voice()
                    {
                        languageCode = voice.languageCode,
                        name = voice.name
                    },
                audioConfig =
                    new AudioConfig()
                    {
                        audioEncoding = "MP3",
                        pitch = voice.pitch,
                        speakingRate = voice.speed
                    }
            };

            RequestService.SendDataToGoogle("https://texttospeech.googleapis.com/v1/text:synthesize", dataToSend,
                apiKey, _actionRequestReceived, errorReceived);
        }

        private static void RequestReceived(string requestData, Action<AudioClip> audioClipReceived)
        {
            var audioData = JsonUtility.FromJson<AudioData>(requestData);
            AudioConverter.SaveTextToMp3(audioData);
            _audioConverter.LoadClipFromMp3(audioClipReceived);
        }
    }
}