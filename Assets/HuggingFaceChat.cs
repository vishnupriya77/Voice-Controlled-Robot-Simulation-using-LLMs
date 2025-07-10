using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using MiniJSON; // ✅ Make sure MiniJSON.cs is in your project
using UnityEngine.Windows.Speech; // ✅ For DictationRecognizer

public class HuggingFaceChat : MonoBehaviour
{
    [Header("OpenRouter Settings")]
    public string openRouterAPIKey = "sk-or-..."; // Replace with your actual key
    public string model = "mistralai/mistral-7b-instruct";

    [Header("UI Elements")]
    public TMP_InputField userInput;
    public TextMeshProUGUI responseText;

    [Header("Action Triggering")]
    public ReachyMotionTest reachyMotion;

    [Header("Last Parsed Action")]
    public string latestAction = "";

    private DictationRecognizer recognizer;

    void Start()
    {
        recognizer = new DictationRecognizer();
        recognizer.InitialSilenceTimeoutSeconds = 5;
        recognizer.AutoSilenceTimeoutSeconds = 3;

        recognizer.DictationResult += (text, confidence) =>
        {
            Debug.Log("🎤 Voice Input: " + text);
            userInput.text = text;
            SendPrompt();
        };

        recognizer.DictationComplete += (completionCause) =>
        {
            Debug.Log("🛑 Dictation stopped: " + completionCause);
        };

        recognizer.DictationError += (error, hresult) =>
        {
            Debug.LogError("❌ Dictation error: " + error);
        };
    }

    public void StartListening()
    {
        Debug.Log("🎙️ Starting voice recognition...");
        recognizer.Start();
    }

    public void SendPrompt()
    {
        string prompt = userInput.text.Trim();
        if (!string.IsNullOrEmpty(prompt))
        {
            Debug.Log("📤 Sending prompt: " + prompt);
            StartCoroutine(SendToOpenRouter(prompt));
        }
        else
        {
            responseText.text = "⚠️ Please enter a message.";
        }
    }

    IEnumerator SendToOpenRouter(string prompt)
    {
        string url = "https://openrouter.ai/api/v1/chat/completions";

        string fullPrompt = $"You are a robot control system. Based on this user input: \"{prompt}\", respond with one JSON object using only ONE action field. Valid actions: wave, raise_right_arm, raise_left_arm, shake_head, rotate_head, say. Do NOT explain anything. Only return JSON like {{\"action\":\"wave\"}} or {{\"action\":\"say\", \"phrase\":\"hello\"}}.";

        OpenRouterRequest payload = new OpenRouterRequest
        {
            model = model,
            messages = new Message[]
            {
                new Message { role = "user", content = fullPrompt }
            }
        };

        string jsonPayload = JsonUtility.ToJson(payload);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {openRouterAPIKey}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string result = request.downloadHandler.text;
            Debug.Log("✅ Raw response: " + result);

            try
            {
                var parsed = Json.Deserialize(result) as Dictionary<string, object>;
                var choices = parsed["choices"] as List<object>;
                var messageDict = ((choices[0] as Dictionary<string, object>)["message"]) as Dictionary<string, object>;
                var content = messageDict["content"].ToString();

                Debug.Log("🤖 Assistant: " + content);
                responseText.text = content;
                Debug.Log("📦 Raw assistant content to parse: " + content);

                var actionJson = Json.Deserialize(content) as Dictionary<string, object>;
                if (actionJson != null && actionJson.ContainsKey("action"))
                {
                    string action = actionJson["action"].ToString();
                    string phrase = actionJson.ContainsKey("phrase") ? actionJson["phrase"].ToString() : "";

                    latestAction = action;
                    Debug.Log("🚀 Parsed action: " + latestAction);
                    Debug.Log("🔧 Calling TriggerAction with: " + action + ", phrase: " + phrase);
                    TriggerAction(action, phrase);
                }
                else
                {
                    Debug.LogWarning("⚠️ No 'action' field found in assistant response.");
                }
            }
            catch
            {
                Debug.LogError("❌ Failed to parse JSON content.");
                responseText.text = "⚠️ Failed to understand the response.";
            }
        }
        else
        {
            Debug.LogError("❌ HTTP Error: " + request.error);
            responseText.text = "Error: " + request.error;
        }
    }

    void TriggerAction(string action, string phrase = "")
    {
        Debug.Log("🔧 TriggerAction() called with action: " + action + ", phrase: " + phrase);

        if (reachyMotion == null)
        {
            Debug.LogWarning("⚠️ ReachyMotionTest reference not assigned in Inspector!");
            return;
        }

        switch (action.ToLower())
        {
            case "wave":
                Debug.Log("👉 Executing: wave");
                reachyMotion.StartCoroutine(reachyMotion.WaveRightHand());
                break;

            case "raise_right_arm":
                Debug.Log("👉 Executing: raise_right_arm");
                reachyMotion.RaiseRightArm();
                break;

            case "raise_left_arm":
                Debug.Log("👉 Executing: raise_left_arm");
                reachyMotion.RaiseLeftArm();
                break;

            case "shake_head":
                Debug.Log("👉 Executing: shake_head");
                reachyMotion.StartCoroutine(reachyMotion.ShakeHead());
                break;

            case "rotate_head":
                Debug.Log("👉 Executing: rotate_head");
                reachyMotion.RotateHead(0f, 30f, 30f);
                break;

            case "say":
            case "speak":
                Debug.Log("🗣️ Speaking: " + phrase);
                responseText.text = phrase;
                break;

            default:
                Debug.LogWarning("🤷 Unknown action received: " + action);
                break;
        }
    }

    // ----- JSON Format Classes -----
    [System.Serializable]
    public class OpenRouterRequest
    {
        public string model;
        public Message[] messages;
    }

    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }
}