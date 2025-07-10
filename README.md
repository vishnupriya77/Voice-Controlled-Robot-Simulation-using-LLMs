# Voice-Controlled Robot Simulation using LLMs 🤖🎙️

|   License     |     |
| ------------- | :-------------: |
| Title  | [Creative Commons BY-NC-SA 4.0](https://creativecommons.org/licenses/by-nc-sa/4.0/legalcode) |
| Logo  | [![Creative Commons BY-NC-SA 4.0](https://i.creativecommons.org/l/by-nc-sa/4.0/88x31.png)](http://creativecommons.org/licenses/by-nc-sa/4.0/) |

This project demonstrates a fully working **voice-controlled robot simulation** using the [Reachy 2021 Unity simulator](https://github.com/pollen-robotics Simulator_Reachy2021), integrated with **LLMs (Large Language Models)** for natural language command processing.

---

## 📦 What This Project Does

- Loads the Reachy Mini simulation inside Unity
- Enables simple motion commands when you press keys (R-raise right arm, S-shake head, W-wave, L-raise left hand, H-rotate head)
- Accepts **text or voice input**
- Sends that input to a **LLM (via OpenRouter API)**
- Receives a structured response (e.g., `{ "action": "wave" }`)
- Executes corresponding motion in Unity

---

## 🔧 Tools Used

- Unity 2021.3.45f1
- Reachy 2021 Simulator by Pollen Robotics
- C# scripts
- OpenRouter API for LLMs (Mistral 7B)
- MiniJSON to parse flexible LLM response objects
- Windows DictationRecognizer for voice input
- Git & GitHub for version control

---

## 🏗️ System Architecture

```
User Input (Text/Voice)
        ↓
HuggingFaceChat.cs
        ↓
Send Prompt to LLM (OpenRouter)
        ↓
Receive JSON Action (e.g., {"action": "wave"})
        ↓
Trigger ReachyMotionTest.cs → Unity Animations
```

- `ReachyMotionTest.cs`: contains coroutine-based motions for Reachy
- `HuggingFaceChat.cs`: handles API request, parses LLM response, and triggers motion
- Dictation input via a mic button is linked to Unity’s UI using `DictationRecognizer`

---

## 🚀 How to Run

1. **Clone the repo**  

2. **Open in Unity**  
   Open the project in Unity 2021.3.45f1 or higher (LTS preferred).

3. **Open OfficeScene** from the `Assets` folder and press Play.

4. **Enter text** in the input field and click Submit, *or* press the **Mic button** to speak your command (e.g., “rotate your head”).

5. The LLM interprets the input and drives the robot action in Unity.

---

## 🔮 Future Enhancements

- Enable the robot to understand and perform combined or complex actions (e.g., wave and say hello).
- Integrate text-to-speech so the robot can verbally respond instead of just displaying text.
