// //If you've bought the Salsa Suite plugin and installed it, you should uncomment the next line to enable lipsyncing.
// //If you don't have it, comment it out, it should compile, but without the lipsyncing and eye movements.
// #define CRAZY_MINNOW_PRESENT

// #if CRAZY_MINNOW_PRESENT
// using CrazyMinnow.SALSA;
// #endif

// using Fusion;
// using SimpleJSON;
// using System;
// using System.Collections.Generic;
// using System.Collections;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;

// using System.IO;
// using System.IO.Compression;
// using System.Text;


// using static OpenAITextCompletionManager;

// public class AIManager : NetworkBehaviour
// {
//     public MicRecorder _microPhoneScript;
//     string _openAI_APIKey;
//     string _openAI_APIModel;
//     string _googleAPIkey;
//     string _elevenLabsAPIkey;
//     public GameObject _visuals;
//     AudioSource _audioSourceToUse = null;
//     Vector2 vTextOverlayPos = new Vector2(Screen.width * 0.58f, (float)Screen.height - ((float)Screen.height * 0.4f));
//     Vector2 vStatusOverlayPos = new Vector2(Screen.width * 0.44f, (float)Screen.height - ((float)Screen.height * 1.1f));
//     public TMPro.TextMeshProUGUI _dialogText;
//     public TMPro.TextMeshProUGUI _statusText;

//     public TMPro.TextMeshProUGUI playerNameText;

//     public string localPlayerName;

//     public Sprite recordingSprite;
//     public Sprite stoppedSprite;

//     public PlayerNameManager playerNameManager;

//     private string evaluation = "Here is the system, the exam is over. Could you give the Students Feedback, taking into account their grammar, the extent to which they covered the topic, whether they included the answers of others, the quality of their English language usage, and their use of a diverse and appropriate vocabulary. And Maybe give their a number of how good the Performance was on 1 the best to 6 the worst."; 

//     Queue<GTPChatLine> _chatHistory = new Queue<GTPChatLine>();

//     public Button _recordButton;
//     // Start is called before the first frame update
//     [Networked] int activeFriendIndex { get; set; }
//     Friend _activeFriend;
//     Animator _animator = null;

//     public TMPro.TextMeshProUGUI _friendNameGUI;

//     //Friend Management:

//     public static byte[] CompressString(string text)
//     {
//         byte[] byteArray = Encoding.UTF8.GetBytes(text);
//         using (var memoryStream = new MemoryStream())
//         {
//             using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
//             {
//                 gzipStream.Write(byteArray, 0, byteArray.Length);
//             }
//             return memoryStream.ToArray();
//         }
//     }

//     public static string DecompressString(byte[] compressedData)
//     {
//         using (var memoryStream = new MemoryStream(compressedData))
//         {
//             using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
//             {
//                 using (var reader = new StreamReader(gzipStream))
//                 {
//                     return reader.ReadToEnd();
//                 }
//             }
//         }
//     }

//     void Start()
//     {
//         localPlayerName = playerNameText.text;
//     }

//     public override void Spawned()
//     {
//         if (Object.HasStateAuthority)
//         {
//             activeFriendIndex = 0;
//         }
//         SetActiveFriendByIndex(activeFriendIndex);
//     }


//     public void SetActiveFriend(Friend newFriend)
//     {
//         if (newFriend == null) return;
//         _activeFriend = newFriend;
//         _audioSourceToUse = gameObject.GetComponent<AudioSource>();
//         _friendNameGUI.text = _activeFriend._name;

//         if (_friendNameGUI.text == "Unset")
//         {
//             _dialogText.text = "Before running this, edit the config_template.txt file to set your API keys, then rename the file to config.txt!";
//             return;
//         }

//         _dialogText.text = "Click Start for the character to introduce themselves.";
//         _statusText.text = "";

//         Debug.Log("Vor ForgetStuff ");
//         ForgetStuff();

//         List<GameObject> objs = new List<GameObject>();
//         Debug.Log("Nach GameObject");
//         RTUtil.AddObjectsToListByNameIncludingInactive(_visuals, "char_visual", true, objs);

//         foreach (GameObject obj in objs)
//         {
//             obj.SetActive(false);
//         }

//         var activeVisual = RTUtil.FindInChildrenIncludingInactive(_visuals, "char_visual_" + _activeFriend._visual);
//         if (activeVisual != null)
//         {
//             activeVisual.SetActive(true);
//         }

// #if CRAZY_MINNOW_PRESENT
//         var lipsyncModel = activeVisual.GetComponentInChildren<Salsa>();
//         if (lipsyncModel != null)
//         {
//             Debug.Log("Found salsa");
//             _audioSourceToUse = lipsyncModel.GetComponent<AudioSource>();
//         }
//         _animator = activeVisual.GetComponentInChildren<Animator>();
// #endif
//         SetListening(false);
//     }

//     public void SetActiveFriendByIndex(int index)
//     {
//         SetActiveFriend(Config.Get().GetFriendByIndex(index));
//     }

//     [Rpc(RpcSources.All, RpcTargets.All)]
//     public void RPC_SetActiveFriend(int friendIndex)
//     {
//         activeFriendIndex = friendIndex;
//         SetActiveFriendByIndex(friendIndex);
//     }

//     public void ModFriend(int mod)
//     {
//         int newFriendIndex = (activeFriendIndex + mod) % Config.Get().GetFriendCount();
//         if (newFriendIndex < 0) newFriendIndex = Config.Get().GetFriendCount() - 1;
//         RPC_SetActiveFriend(newFriendIndex);
//     }

//     public void OnPreviousFriend()
//     {
//         PlayClickSound();
//         ModFriend(-1);
//     }

//     public void OnNextFriend()
//     {
//         PlayClickSound();
//         ModFriend(1);
//     }



//     //Chat Text Management:

//     public void GetGPT3Text(string question)
//     {
//         if (Object.HasStateAuthority)
//         {
//             // build the prompt and make the request
//             OpenAITextCompletionManager textCompletionScript = gameObject.GetComponent<OpenAITextCompletionManager>();
//             Queue<GTPChatLine> lines = new Queue<GTPChatLine>();
//             lines.Enqueue(new GTPChatLine("system", GetBasePrompt()));

//             TrimHistoryIfNeeded();
//             foreach (GTPChatLine line in _chatHistory)
//             {
//                 lines.Enqueue(line);
//             }

//             lines.Enqueue(new GTPChatLine("system", GetDirectionPrompt()));
//             lines.Enqueue(new GTPChatLine("user", question));

//             string json = textCompletionScript.BuildChatCompleteJSON(lines, _activeFriend._maxTokensToGenerate, _activeFriend._temperature, _openAI_APIModel);
//             RTDB db = new RTDB();
//             db.Set("question", question);
//             db.Set("role", "user");

//             textCompletionScript.SpawnChatCompleteRequest(json, OnGPT3TextCompletedCallback, db, _openAI_APIKey);
//             UpdateStatusText(RTUtil.ConvertSansiToUnityColors("(AI is thinking) You said: `$" + question + "``"), 20);
//         }
//     }

// [Rpc(RpcSources.All, RpcTargets.All)]
// void RPC_UpdateChatHistory(byte[] compressedRole, byte[] compressedContent)
// {
//     string role = DecompressString(compressedRole);
//     string content = DecompressString(compressedContent);
//     _chatHistory.Enqueue(new GTPChatLine(role, content));
// }

// void OnGPT3TextCompletedCallback(RTDB db, JSONObject jsonNode)
// {
//     if (jsonNode == null)
//     {
//         Debug.Log("Got callback! Data: " + db.ToString());
//         UpdateStatusText(db.GetString("msg"));
//         return;
//     }

//     string reply = jsonNode["choices"][0]["message"]["content"];
//     if (reply.Length < 5)
//     {
//         Debug.Log("Error parsing reply: " + reply);
//         db.Set("english", "Error. I don't know what to say.");
//         db.Set("japanese", "エラーです。なんて言っていいのかわからない。");
//         byte[] compressedReply = CompressString(db.GetString("english"));
//         RPC_SayText(compressedReply);
//         return;
//     }

//     db.Set("english", reply);
//     db.Set("japanese", reply);

//     byte[] compressedEnglish = CompressString(db.GetString("english"));
//     RPC_SayText(compressedEnglish);

//     byte[] compressedRole = CompressString(db.GetString("role"));
//     byte[] compressedQuestion = CompressString(db.GetString("question"));
//     byte[] compressedReplyForHistory = CompressString(reply);
//     RPC_UpdateChatHistory(compressedRole, compressedQuestion);
//     RPC_UpdateChatHistory(CompressString("assistant"), compressedReplyForHistory);
// }

//  [Rpc(RpcSources.All, RpcTargets.All)]
// void RPC_SayText(byte[] compressedText)
// {
//     string text = DecompressString(compressedText);
//     Debug.Log($"RPC_SayText called with text: {text}");
//     SayText(text);
// }

// void SayText(string text)
// {
//     if (_activeFriend._googleVoice.Length > 1 && _googleAPIkey.Length > 1)
//     {
//         Debug.Log("using Google Voice");
//         string countryCode = _activeFriend._googleVoice.Substring(0, 5);
//         GoogleTextToSpeechManager ttsScript = gameObject.GetComponent<GoogleTextToSpeechManager>();
//         string json = ttsScript.BuildTTSJSON(text, countryCode, _activeFriend._googleVoice, 22050, _activeFriend._pitch, _activeFriend._speed);
//         ttsScript.SpawnTTSRequest(json, OnTTSCompletedCallback, new RTDB(), _googleAPIkey);
//         UpdateStatusText("Clearing throat...", 20);
//         UpdateDialogText(text);
//     }
//     else
//     {
//         UpdateDialogText(text);
//         UpdateStatusText("");
//     }
// }


//     void OnTTSCompletedCallback(RTDB db, byte[] wavData)
//     {
//         if (wavData == null)
//         {
//             Debug.Log("Error getting wav: " + db.GetString("msg"));
//         }
//         else
//         {
//             GoogleTextToSpeechManager ttsScript = gameObject.GetComponent<GoogleTextToSpeechManager>();
//             AudioSource audioSource = _audioSourceToUse;
//             audioSource.clip = ttsScript.MakeAudioClipFromWavFileInMemory(wavData);
//             audioSource.Play();
//         }

//         // Ensure that "english" key exists in the dictionary before attempting to get it
//         // if (db.ContainsKey("english"))
//         // {
//             // UpdateDialogText(db.GetString("english"));
//         // }
//         UpdateStatusText("");
//     }


//     // void OnTTSCompletedCallbackElevenLabs(RTDB db, AudioClip clip)
//     // {
//     //     if (clip == null)
//     //     {
//     //         Debug.Log("Error getting wav: " + db.GetString("msg"));
//     //     }
//     //     else
//     //     {
//     //         ElevenLabsTextToSpeechManager ttsScript = gameObject.GetComponent<ElevenLabsTextToSpeechManager>();
//     //         AudioSource audioSource = _audioSourceToUse;
//     //         audioSource.clip = clip;
//     //         audioSource.Play();
//     //     }

//     //     UpdateDialogText(db.GetString("japanese"));
//     //     UpdateStatusText("");
//     // }


//     //User Interaction:

//     // [Rpc(RpcSources.All, RpcTargets.All)]
//     // public void RPC_ToggleRecording(string playerName)
//     // {
        
//     //     if (!_microPhoneScript.IsRecording())
//     //     {
//     //         StopTalking();
//     //         Debug.Log("Recording started by player: " + playerName);
//     //         // _recordButton.GetComponent<Image>().color = Color.red;
//     //         _recordButton.GetComponent<Image>().sprite = recordingSprite;
//     //         _microPhoneScript.StartRecording();
//     //         PlayClickSound();
//     //         SetListening(true);
//     //     }
//     //     else
//     //     {
//     //         Debug.Log("Recording stopped by player: " + playerName);
//     //         // _recordButton.GetComponent<Image>().color = Color.white;
//     //         _recordButton.GetComponent<Image>().sprite = stoppedSprite;
//     //         PlayClickSound();
//     //         string outputFileName = Application.temporaryCachePath + "/temp.wav";
//     //         _microPhoneScript.StopRecordingAndProcess(outputFileName, playerName);
//     //         SetListening(false);
//     //     }
//     // }

// public void ToggleRecording()
// {
//     string playerName = playerNameText.text;

//     if (!_microPhoneScript.IsRecording())
//     {
//         StopTalking();
//         Debug.Log("Recording started by player: " + playerName);
//         _recordButton.GetComponent<Image>().sprite = recordingSprite;
//         _microPhoneScript.StartRecording();
//         PlayClickSound();
//         SetListening(true);
//         RPC_NotifyRecordingStarted(playerName);
//     }
//     else
//     {
//         Debug.Log("Recording stopped by player: " + playerName);
//         _recordButton.GetComponent<Image>().sprite = stoppedSprite;
//         PlayClickSound();
//         string outputFileName = Application.temporaryCachePath + "/temp.wav";
//         _microPhoneScript.StopRecordingAndProcess(outputFileName, playerName);
//         SetListening(false);
//         RPC_NotifyRecordingStopped(playerName);
//     }
// }

// [Rpc(RpcSources.All, RpcTargets.All)]
// public void RPC_NotifyRecordingStarted(string playerName)
// {
//     if (playerName != localPlayerName)
//     {
//         Debug.Log("Player " + playerName + " started recording.");
//         // Optionally update UI to reflect other player's recording state
//     }
// }

// [Rpc(RpcSources.All, RpcTargets.All)]
// public void RPC_NotifyRecordingStopped(string playerName)
// {
//     if (playerName != localPlayerName)
//     {
//         Debug.Log("Player " + playerName + " stopped recording.");
//         // Optionally update UI to reflect other player's recording state
//     }
// }



//     // public void ToggleRecording()
//     // {
//     //     // if (Object.HasStateAuthority)
//     //     // {
//     //     string playerName = playerNameText.text;

//     //     RPC_ToggleRecording(playerName);
//     //     // }
//     // }

//     [Rpc(RpcSources.All, RpcTargets.All)]
//     public void RPC_OnStopButton()
//     {
//         PlayClickSound();
//         StopTalking();
//     }

//     [Rpc(RpcSources.All, RpcTargets.All)]
//     public void RPC_OnCopyButton()
//     {
//         PlayClickSound();
//         string text = _dialogText.text;
//         if (text.Length > 0)
//         {
//             GUIUtility.systemCopyBuffer = text;
//             UpdateStatusText("Copied to clipboard");
//         }
//         else
//         {
//             UpdateStatusText("Nothing to copy");
//         }
//     }

//     [Rpc(RpcSources.All, RpcTargets.All)]
//     public void RPC_Evaluation(){

//        if (Object.HasStateAuthority)
//         {
//             // build the prompt and make the request
//             OpenAITextCompletionManager textCompletionScript = gameObject.GetComponent<OpenAITextCompletionManager>();
//             Queue<GTPChatLine> lines = new Queue<GTPChatLine>();
//             lines.Enqueue(new GTPChatLine("system", GetBasePrompt()));

//             TrimHistoryIfNeeded();
//             foreach (GTPChatLine line in _chatHistory)
//             {
//                 lines.Enqueue(line);
//             }

//             lines.Enqueue(new GTPChatLine("system", GetDirectionPrompt()));
//             lines.Enqueue(new GTPChatLine("user", evaluation));

//             string json = textCompletionScript.BuildChatCompleteJSON(lines, 800, _activeFriend._temperature, _openAI_APIModel);
//             RTDB db = new RTDB();
//             db.Set("question", evaluation);
//             db.Set("role", "user");

//             textCompletionScript.SpawnChatCompleteRequest(json, OnGPT3TextCompletedCallback, db, _openAI_APIKey);
//             UpdateStatusText(RTUtil.ConvertSansiToUnityColors("(AI is currently evaluating"), 20);
//         }
//     }

//     [Rpc(RpcSources.All, RpcTargets.All)]
//     public void RPC_OnAdviceButton()
//     {
//         ForgetStuff();
//         PlayClickSound();

//         if (Object.HasStateAuthority)
//         {
//             OpenAITextCompletionManager textCompletionScript = gameObject.GetComponent<OpenAITextCompletionManager>();
//             Queue<GTPChatLine> lines = new Queue<GTPChatLine>();
//             lines.Enqueue(new GTPChatLine("system", GetBasePrompt()));

//             TrimHistoryIfNeeded();
//             foreach (GTPChatLine line in _chatHistory)
//             {
//                 lines.Enqueue(line);
//             }

//             string playerNames = "Your Students are: " +  playerNameManager.playerNames.ToString();
//             Debug.Log("Player Names are: " + playerNames);

//             string question = GetAdvicePrompt();
//             question = playerNames + question;
//             Debug.Log("Adivce Prompt: " + question);
//             lines.Enqueue(new GTPChatLine("system", GetDirectionPrompt()));
//             lines.Enqueue(new GTPChatLine("system", question));

//             string json = textCompletionScript.BuildChatCompleteJSON(lines, _activeFriend._maxTokensToGenerate, _activeFriend._temperature, _openAI_APIModel);
//             RTDB db = new RTDB();
//             db.Set("role", "system");
//             db.Set("question", question);
//             textCompletionScript.SpawnChatCompleteRequest(json, OnGPT3TextCompletedCallback, db, _openAI_APIKey);
//             UpdateStatusText(RTUtil.ConvertSansiToUnityColors("Thinking..."), 20);
//             UpdateDialogText("");
//         }
//     }

//     // //UI Visuals
//     // [Rpc(RpcSources.All, RpcTargets.All)]
//     // public void RPC_UpdateStatusText(string msg, float timer = 3)
//     // {
//     //     Debug.Log($"RPC_UpdateStatusText is executed: {msg} at {Time.time}");
//     //     // _statusText.text = msg;
//     //     if (_statusText != null)
//     //     {
//     //         _statusText.text = msg;
//     //         // StartCoroutine(ForceRefresh(_statusText));
//     //     }
//     // }

//     // [Rpc(RpcSources.All, RpcTargets.All)]
//     // public void RPC_UpdateDialogText(string msg)
//     // {
//     //     Debug.Log($"RPC_UpdateDialogText is executed: {msg} at {Time.time}");
//     //     // _dialogText.text = msg;
//     //     if (_dialogText != null)
//     //     {
//     //         _dialogText.text = msg;
//     //     }
//     // }

//     [Rpc(RpcSources.All, RpcTargets.All)]
// public void RPC_UpdateDialogText(byte[] compressedMsg)
// {
//     string msg = DecompressString(compressedMsg);
//     Debug.Log($"RPC_UpdateDialogText is executed: {msg} at {Time.time}");
//     if (_dialogText != null)
//     {
//         _dialogText.text = msg;
//     }
// }

//     // private IEnumerator ForceRefresh(TMP_Text textComponent)
//     // {
//     //     textComponent.ForceMeshUpdate(); // For TextMeshPro components
//     //     yield return null;
//     //     textComponent.ForceMeshUpdate(); // Forcing again in the next frame
//     // }

//     // void UpdateStatusText(string msg, float timer = 3)
//     // {
//     //     Debug.Log($"Updating status text: {msg} at {Time.time}");
//     //     if (Object.HasStateAuthority)
//     //     {
//     //         RPC_UpdateStatusText(msg, timer);
//     //     }
//     // }

//     [Rpc(RpcSources.All, RpcTargets.All)]
// public void RPC_UpdateStatusText(byte[] compressedMsg, float timer = 3)
// {
//     string msg = DecompressString(compressedMsg);
//     Debug.Log($"RPC_UpdateStatusText is executed: {msg} at {Time.time}");
//     if (_statusText != null)
//     {
//         _statusText.text = msg;
//     }
// }

// void UpdateStatusText(string msg, float timer = 3)
// {
//     Debug.Log($"Updating status text: {msg} at {Time.time}");
//     if (Object.HasStateAuthority)
//     {
//         byte[] compressedMsg = CompressString(msg);
//         RPC_UpdateStatusText(compressedMsg, timer);
//     }
// }

//     // void UpdateDialogText(string msg)
//     // {
//     //     Debug.Log($"Updating dialog text: {msg} at {Time.time}");
//     //     if (Object.HasStateAuthority)
//     //     {
//     //         RPC_UpdateDialogText(msg);
//     //     }
//     // }
    
    
// void UpdateDialogText(string msg)
// {
//     Debug.Log($"Updating dialog text: {msg} at {Time.time}");
//     if (Object.HasStateAuthority)
//     {
//         byte[] compressedMsg = CompressString(msg);
//         RPC_UpdateDialogText(compressedMsg);
//     }
// }



//     //Rest
//     void SetListening(bool bNew)
//     {
//         if (_animator != null)
//         {
//             _animator.SetBool("Listening", bNew);
//         }
//     }

//     void SetTalking(bool bNew)
//     {
//         if (_animator != null)
//         {
//             _animator.SetBool("Talking", bNew);
//         }
//     }

//     public void SetGoogleAPIKey(string key)
//     {
//         _googleAPIkey = key;
//     }

//     public void SetOpenAI_APIKey(string key)
//     {
//         _openAI_APIKey = key;
//     }
//     public void SetOpenAI_Model(string model)
//     {
//         _openAI_APIModel = model;
//     }

//     public void SetElevenLabsAPIKey(string key)
//     {
//         _elevenLabsAPIkey = key;
//     }

//     public string GetAdvicePrompt()
//     {
//         return _activeFriend._advicePrompt;
//     }


//     public int CountWords(string input)
//     {
//         if (string.IsNullOrWhiteSpace(input))
//         {
//             return 0;
//         }

//         // Split the input into words and return the count
//         string[] words = input.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
//         return words.Length;
//     }

//     string GetBasePrompt()
//     {
//         return _activeFriend._basePrompt;
//     }

//     string GetDirectionPrompt()
//     {
//         return _activeFriend._directionPrompt;
//     }

//     void TrimHistoryIfNeeded()
//     {
//         int tokenSize = CountWords(GetBasePrompt());
//         int historyTokenSize = 0;
//         //tokenSize of all words in _chatHistory
//         foreach (GTPChatLine line in _chatHistory)
//         {
//             historyTokenSize += CountWords(line._content);
//         }

//         int maxTokenUseForPromptsAndHistory = tokenSize + _activeFriend._friendTokenMemory; //too high and the text gets... corrupted...

//         if (tokenSize + historyTokenSize > maxTokenUseForPromptsAndHistory)
//         {
//             //remove oldest lines until we are under the max
//             while (tokenSize + historyTokenSize > maxTokenUseForPromptsAndHistory)
//             {
//                 //we always remove things in pairs, the request, and the answer.

//                 GTPChatLine line = _chatHistory.Dequeue();
//                 historyTokenSize -= CountWords(line._content);
//                 line = _chatHistory.Dequeue();
//                 historyTokenSize -= CountWords(line._content);
//                 line = _chatHistory.Dequeue();
//                 historyTokenSize -= CountWords(line._content);
//             }
//         }

//         Debug.Log("Prompt tokens: " + tokenSize + " History token size:" + historyTokenSize);

//     }


//     public int GetFriendIndex()
//     {
//         if (_activeFriend == null)
//             return 0;
//         else
//             return _activeFriend._index;

//     }

//     //  public void ProcessMicAudioByFileName(string fAudioFileName)
//     // {
//     //     OpenAISpeechToTextManager speechToTextScript = gameObject.GetComponent<OpenAISpeechToTextManager>();

//     //     byte[] fileBytes = System.IO.File.ReadAllBytes(fAudioFileName);
//     //     string prompt = "";

//     //     RTDB db = new RTDB();

//     //     //let's add strings from the recent conversation to the prompt text
//     //     foreach (GTPChatLine line in _chatHistory)
//     //     {
//     //         prompt += line._content + "\n";
//     //         if (prompt.Length > 180)
//     //         {
//     //             //whisper will only processes the last 200 words I read
//     //             break;
//     //         }
//     //     }

//     //     if (prompt == "")
//     //     {
//     //         //no history yet?  Ok, use the base prompt, better than nothing
//     //         prompt = _activeFriend._basePrompt;
//     //     }


//     //     speechToTextScript.SpawnSpeechToTextRequest(prompt, OnSpeechToTextCompletedCallback, db, _openAI_APIKey, fileBytes);
//     //     UpdateStatusText("Understanding speech...", 20);

//     // }

//     public void ProcessMicAudioByFileName(string fAudioFileName, string playerName)
//     {
//         OpenAISpeechToTextManager speechToTextScript = gameObject.GetComponent<OpenAISpeechToTextManager>();

//         byte[] fileBytes = System.IO.File.ReadAllBytes(fAudioFileName);
//         //string playerName1 = playerNameText.text; 
//         //string prompt = $"{playerName1}: "; // Include the player's name at the beginning of the prompt
//         string prompt = "";

//         RTDB db = new RTDB();
//         db.Set("playerName", playerName);

//         // Let's add strings from the recent conversation to the prompt text
//         foreach (GTPChatLine line in _chatHistory)
//         {
//             prompt += line._content + "\n";
//             if (prompt.Length > 180)
//             {
//                 // Whisper will only process the last 200 words I read
//                 break;
//             }
//         }

//         if (prompt == "")
//         {
//             // No history yet? Ok, use the base prompt, better than nothing
//             prompt = _activeFriend._basePrompt;
//         }

//         speechToTextScript.SpawnSpeechToTextRequest(prompt, OnSpeechToTextCompletedCallback, db, _openAI_APIKey, fileBytes);
//         UpdateStatusText($"Understanding speech for...", 20); // Update the status text with the player name
//     }


//     void OnSpeechToTextCompletedCallback(RTDB db, JSONObject jsonNode)
//     {

//         string playerName = db.GetString("playerName");
//         if (jsonNode == null)
//         {
//             //must have been an error
//             Debug.Log("Got callback! Data: " + db.ToString());
//             UpdateStatusText(db.GetString("msg"));
//             return;
//         }

//         string reply = jsonNode["text"];
//         // string playerName2 = playerNameText.text; 
//         Debug.Log(playerName);
//         reply = playerName + ": " + reply;
//         Debug.Log("Heard: " + reply);
//         UpdateStatusText("Heard: " + reply);
//         GetGPT3Text(reply);
//     }

//     // public void ForgetStuff()
//     // {
//     //     RPC_ForgetStuff();
//     // }

//     [Rpc(RpcSources.All, RpcTargets.All)]
//     public void RPC_ForgetStuff()
//     {
//         _chatHistory.Clear();
//         StopTalking();
//     }

//     public void ForgetStuff()
//     {
//         Debug.Log("ForgetStuff Aufgerufen");
//         _chatHistory.Clear();
//         StopTalking();
//     }

   

//     public void OnForgetButton()
//     {
//         //Clear chat history
//         PlayClickSound();   //write a message about it
//                             //RTQuickMessageManager.Get().ShowMessage("Chat history cleared");
//         RPC_ForgetStuff();
//     }

//     public void PlayClickSound()
//     {
//         RTMessageManager.Get().Schedule(0, RTAudioManager.Get().PlayEx, "muffled_bump", 0.5f, 1.0f, false, 0.0f);
//     }

//     public void StopTalking()
//     {
//         AudioSource audioSource = _audioSourceToUse;
//         audioSource.Stop();
//         SetTalking(false);

//     }

// }


#define CRAZY_MINNOW_PRESENT

#if CRAZY_MINNOW_PRESENT
using CrazyMinnow.SALSA;
#endif

using Fusion;
using SimpleJSON;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.IO.Compression;
using System.Text;
using static OpenAITextCompletionManager;

public class AIManager : NetworkBehaviour
{
    public MicRecorder _microPhoneScript;
    string _openAI_APIKey;
    string _openAI_APIModel;
    string _googleAPIkey;
    string _elevenLabsAPIkey;
    public GameObject _visuals;
    AudioSource _audioSourceToUse = null;
    Vector2 vTextOverlayPos = new Vector2(Screen.width * 0.58f, (float)Screen.height - ((float)Screen.height * 0.4f));
    Vector2 vStatusOverlayPos = new Vector2(Screen.width * 0.44f, (float)Screen.height - ((float)Screen.height * 1.1f));
    public TMPro.TextMeshProUGUI _dialogText;
    public TMPro.TextMeshProUGUI _statusText;

    public TMPro.TextMeshProUGUI playerNameText;
    public string localPlayerName;

    public Sprite recordingSprite;
    public Sprite stoppedSprite;

    public PlayerNameManager playerNameManager;

    private string evaluation = "Here is the system, the exam is over. Could you give the Students Feedback, taking into account their grammar, the extent to which they covered the topic, whether they included the answers of others, the quality of their English language usage, and their use of a diverse and appropriate vocabulary. And Maybe give their a number of how good the Performance was on 1 the best to 6 the worst.";

    Queue<GTPChatLine> _chatHistory = new Queue<GTPChatLine>();

    public Button _recordButton;

    [Networked] int activeFriendIndex { get; set; }
    Friend _activeFriend;
    Animator _animator = null;

    public TMPro.TextMeshProUGUI _friendNameGUI;

    void Start()
    {
        localPlayerName = playerNameText.text;
    }

    // public static byte[] CompressString(string text)
    // {
    //     byte[] byteArray = Encoding.UTF8.GetBytes(text);
    //     using (var memoryStream = new MemoryStream())
    //     {
    //         using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
    //         {
    //             gzipStream.Write(byteArray, 0, byteArray.Length);
    //         }
    //         return memoryStream.ToArray();
    //     }
    // }

    // public static string DecompressString(byte[] compressedData)
    // {
    //     using (var memoryStream = new MemoryStream(compressedData))
    //     {
    //         using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
    //         {
    //             using (var reader = new StreamReader(gzipStream))
    //             {
    //                 return reader.ReadToEnd();
    //             }
    //         }
    //     }
    // }

     public static byte[] CompressString(string text)
    {
        //Brotli
        byte[] byteArray = Encoding.UTF8.GetBytes(text);
        using (var memoryStream = new MemoryStream())
        {
            using (var brotliStream = new BrotliStream(memoryStream, System.IO.Compression.CompressionLevel.Optimal))
            {
                brotliStream.Write(byteArray, 0, byteArray.Length);
            }
            return memoryStream.ToArray();
        }
    }

    public static string DecompressString(byte[] compressedData)
    {
        //Brotli
        using (var memoryStream = new MemoryStream(compressedData))
        {
            using (var brotliStream = new BrotliStream(memoryStream, CompressionMode.Decompress))
            {
                using (var reader = new StreamReader(brotliStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }


    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            activeFriendIndex = 0;
        }
        SetActiveFriendByIndex(activeFriendIndex);
    }

    public void SetActiveFriend(Friend newFriend)
    {
        if (newFriend == null) return;
        _activeFriend = newFriend;
        _audioSourceToUse = gameObject.GetComponent<AudioSource>();
        _friendNameGUI.text = _activeFriend._name;

        if (_friendNameGUI.text == "Unset")
        {
            _dialogText.text = "Before running this, edit the config_template.txt file to set your API keys, then rename the file to config.txt!";
            return;
        }

        _dialogText.text = "Click Start for the character to introduce themselves.";
        _statusText.text = "";

        ForgetStuff();

        List<GameObject> objs = new List<GameObject>();
        RTUtil.AddObjectsToListByNameIncludingInactive(_visuals, "char_visual", true, objs);

        foreach (GameObject obj in objs)
        {
            obj.SetActive(false);
        }

        var activeVisual = RTUtil.FindInChildrenIncludingInactive(_visuals, "char_visual_" + _activeFriend._visual);
        if (activeVisual != null)
        {
            activeVisual.SetActive(true);
        }

#if CRAZY_MINNOW_PRESENT
        var lipsyncModel = activeVisual.GetComponentInChildren<Salsa>();
        if (lipsyncModel != null)
        {
            Debug.Log("Found salsa");
            _audioSourceToUse = lipsyncModel.GetComponent<AudioSource>();
        }
        _animator = activeVisual.GetComponentInChildren<Animator>();
#endif
        SetListening(false);
    }

    public void SetActiveFriendByIndex(int index)
    {
        SetActiveFriend(Config.Get().GetFriendByIndex(index));
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetActiveFriend(int friendIndex)
    {
        activeFriendIndex = friendIndex;
        SetActiveFriendByIndex(friendIndex);
    }

    public void ModFriend(int mod)
    {
        int newFriendIndex = (activeFriendIndex + mod) % Config.Get().GetFriendCount();
        if (newFriendIndex < 0) newFriendIndex = Config.Get().GetFriendCount() - 1;
        RPC_SetActiveFriend(newFriendIndex);
    }

    public void OnPreviousFriend()
    {
        PlayClickSound();
        ModFriend(-1);
    }

    public void OnNextFriend()
    {
        PlayClickSound();
        ModFriend(1);
    }

    // Chat Text Management:

    public void GetGPT3Text(string question)
    {
        Debug.Log($"[GetGPT3Text] Processing question for player: {localPlayerName}");
        OpenAITextCompletionManager textCompletionScript = gameObject.GetComponent<OpenAITextCompletionManager>();
        Queue<GTPChatLine> lines = new Queue<GTPChatLine>();
        lines.Enqueue(new GTPChatLine("system", GetBasePrompt()));

        TrimHistoryIfNeeded();
        foreach (GTPChatLine line in _chatHistory)
        {
            lines.Enqueue(line);
        }

        lines.Enqueue(new GTPChatLine("system", GetDirectionPrompt()));
        lines.Enqueue(new GTPChatLine("user", question));

        string json = textCompletionScript.BuildChatCompleteJSON(lines, _activeFriend._maxTokensToGenerate, _activeFriend._temperature, _openAI_APIModel);
        RTDB db = new RTDB();
        db.Set("question", question);
        db.Set("role", "user");

        textCompletionScript.SpawnChatCompleteRequest(json, OnGPT3TextCompletedCallback, db, _openAI_APIKey);
        UpdateStatusText(RTUtil.ConvertSansiToUnityColors($"(AI is thinking) You said: {question}"), 20);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_UpdateChatHistory(byte[] compressedRole, byte[] compressedContent)
    {
        string role = DecompressString(compressedRole);
        string content = DecompressString(compressedContent);
        _chatHistory.Enqueue(new GTPChatLine(role, content));
    }

    void OnGPT3TextCompletedCallback(RTDB db, JSONObject jsonNode)
    {
        string playerName =localPlayerName;
        Debug.Log($"[OnGPT3TextCompletedCallback] AI response for player: {playerName}");

        if (jsonNode == null)
        {
            Debug.Log("Got callback! Data: " + db.ToString());
            UpdateStatusText(db.GetString("msg"));
            return;
        }

        string reply = jsonNode["choices"][0]["message"]["content"];
        if (reply.Length < 5)
        {
            Debug.Log("Error parsing reply: " + reply);
            db.Set("english", "Error. I don't know what to say.");
            db.Set("japanese", "エラーです。なんて言っていいのかわからない。");
            byte[] compressedReply = CompressString(db.GetString("english"));
            RPC_SayText(compressedReply);
            return;
        }

        db.Set("english", reply);
        db.Set("japanese", reply);

        byte[] compressedEnglish = CompressString(db.GetString("english"));
        RPC_SayText(compressedEnglish);

        byte[] compressedRole = CompressString(db.GetString("role"));
        byte[] compressedQuestion = CompressString(db.GetString("question"));
        byte[] compressedReplyForHistory = CompressString(reply);
        RPC_UpdateChatHistory(compressedRole, compressedQuestion);
        RPC_UpdateChatHistory(CompressString("assistant"), compressedReplyForHistory);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_SayText(byte[] compressedText)
    {
        string text = DecompressString(compressedText);
        Debug.Log($"RPC_SayText called with text: {text}");
        SayText(text);
    }

    void SayText(string text)
    {
        Debug.Log($"[SayText] AI is saying: {text}");
        if (_activeFriend._googleVoice.Length > 1 && _googleAPIkey.Length > 1)
        {
            Debug.Log("using Google Voice");
            string countryCode = _activeFriend._googleVoice.Substring(0, 5);
            GoogleTextToSpeechManager ttsScript = gameObject.GetComponent<GoogleTextToSpeechManager>();
            string json = ttsScript.BuildTTSJSON(text, countryCode, _activeFriend._googleVoice, 22050, _activeFriend._pitch, _activeFriend._speed);
            ttsScript.SpawnTTSRequest(json, OnTTSCompletedCallback, new RTDB(), _googleAPIkey);
            UpdateStatusText("Clearing throat...", 20);
            UpdateDialogText(text);
        }
        else
        {
            UpdateDialogText(text);
            UpdateStatusText("");
        }
    }

    void OnTTSCompletedCallback(RTDB db, byte[] wavData)
    {
        if (wavData == null)
        {
            Debug.Log("Error getting wav: " + db.GetString("msg"));
        }
        else
        {
            GoogleTextToSpeechManager ttsScript = gameObject.GetComponent<GoogleTextToSpeechManager>();
            AudioSource audioSource = _audioSourceToUse;
            audioSource.clip = ttsScript.MakeAudioClipFromWavFileInMemory(wavData);
            audioSource.Play();
        }

        UpdateStatusText("");
    }

    public void ToggleRecording()
    {
        string playerName = playerNameText.text;

        if (!_microPhoneScript.IsRecording())
        {
            StopTalking();
            Debug.Log("Recording started by player: " + playerName);
            _recordButton.GetComponent<Image>().sprite = recordingSprite;
            _microPhoneScript.StartRecording();
            PlayClickSound();
            SetListening(true);
            RPC_NotifyRecordingStarted(playerName);
        }
        else
        {
            Debug.Log("Recording stopped by player: " + playerName);
            _recordButton.GetComponent<Image>().sprite = stoppedSprite;
            PlayClickSound();
            string outputFileName = Application.temporaryCachePath + "/temp.wav";
            _microPhoneScript.StopRecordingAndProcess(outputFileName, playerName);
            SetListening(false);
            RPC_NotifyRecordingStopped(playerName);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_NotifyRecordingStarted(string playerName)
    {
        if (playerName != localPlayerName)
        {
            Debug.Log("Player " + playerName + " started recording.");
            // Optionally update UI to reflect other player's recording state
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_NotifyRecordingStopped(string playerName)
    {
        if (playerName != localPlayerName)
        {
            Debug.Log("Player " + playerName + " stopped recording.");
            // Optionally update UI to reflect other player's recording state
        }
    }

    public void ProcessMicAudioByFileName(string fAudioFileName, string playerName)
    {
        OpenAISpeechToTextManager speechToTextScript = gameObject.GetComponent<OpenAISpeechToTextManager>();

        byte[] fileBytes = System.IO.File.ReadAllBytes(fAudioFileName);
        string prompt = "";

        RTDB db = new RTDB();
        db.Set("playerName", playerName);

        foreach (GTPChatLine line in _chatHistory)
        {
            prompt += line._content + "\n";
            if (prompt.Length > 180)
            {
                break;
            }
        }

        if (prompt == "")
        {
            prompt = _activeFriend._basePrompt;
        }

        speechToTextScript.SpawnSpeechToTextRequest(prompt, OnSpeechToTextCompletedCallback, db, _openAI_APIKey, fileBytes);
        UpdateStatusText($"Understanding speech for {playerName}...", 20);
    }

    void OnSpeechToTextCompletedCallback(RTDB db, JSONObject jsonNode)
    {
        string playerName = db.GetString("playerName");
        Debug.Log($"[OnSpeechToTextCompletedCallback] AI processing response for player: {playerName}");

        if (jsonNode == null)
        {
            Debug.Log("Got callback! Data: " + db.ToString());
            UpdateStatusText(db.GetString("msg"));
            return;
        }

        string reply = jsonNode["text"];
        reply = playerName + ": " + reply;
        Debug.Log("Heard: " + reply);
        UpdateStatusText("Heard: " + reply);
        GetGPT3Text(reply);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_OnStopButton()
    {
        PlayClickSound();
        StopTalking();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_OnCopyButton()
    {
        PlayClickSound();
        string text = _dialogText.text;
        if (text.Length > 0)
        {
            GUIUtility.systemCopyBuffer = text;
            UpdateStatusText("Copied to clipboard");
        }
        else
        {
            UpdateStatusText("Nothing to copy");
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Evaluation()
    {
        OpenAITextCompletionManager textCompletionScript = gameObject.GetComponent<OpenAITextCompletionManager>();
        Queue<GTPChatLine> lines = new Queue<GTPChatLine>();
        lines.Enqueue(new GTPChatLine("system", GetBasePrompt()));

        TrimHistoryIfNeeded();
        foreach (GTPChatLine line in _chatHistory)
        {
            lines.Enqueue(line);
        }

        lines.Enqueue(new GTPChatLine("system", GetDirectionPrompt()));
        lines.Enqueue(new GTPChatLine("user", evaluation));

        string json = textCompletionScript.BuildChatCompleteJSON(lines, 800, _activeFriend._temperature, _openAI_APIModel);
        RTDB db = new RTDB();
        db.Set("question", evaluation);
        db.Set("role", "user");

        textCompletionScript.SpawnChatCompleteRequest(json, OnGPT3TextCompletedCallback, db, _openAI_APIKey);
        UpdateStatusText(RTUtil.ConvertSansiToUnityColors("(AI is currently evaluating)"), 20);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_OnAdviceButton()
    {
        ForgetStuff();
        PlayClickSound();

        OpenAITextCompletionManager textCompletionScript = gameObject.GetComponent<OpenAITextCompletionManager>();
        Queue<GTPChatLine> lines = new Queue<GTPChatLine>();
        lines.Enqueue(new GTPChatLine("system", GetBasePrompt()));

        TrimHistoryIfNeeded();
        foreach (GTPChatLine line in _chatHistory)
        {
            lines.Enqueue(line);
        }

        string playerNames = "Your Students are: " + playerNameManager.playerNames.ToString();
        Debug.Log("Player Names are: " + playerNames);

        string question = GetAdvicePrompt();
        question = playerNames + question;
        Debug.Log("Advice Prompt: " + question);
        lines.Enqueue(new GTPChatLine("system", GetDirectionPrompt()));
        lines.Enqueue(new GTPChatLine("system", question));

        string json = textCompletionScript.BuildChatCompleteJSON(lines, _activeFriend._maxTokensToGenerate, _activeFriend._temperature, _openAI_APIModel);
        RTDB db = new RTDB();
        db.Set("role", "system");
        db.Set("question", question);
        textCompletionScript.SpawnChatCompleteRequest(json, OnGPT3TextCompletedCallback, db, _openAI_APIKey);
        UpdateStatusText(RTUtil.ConvertSansiToUnityColors("Thinking..."), 20);
        UpdateDialogText("");
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_UpdateDialogText(byte[] compressedMsg)
    {
        string msg = DecompressString(compressedMsg);
        Debug.Log($"RPC_UpdateDialogText is executed: {msg} at {Time.time}");
        if (_dialogText != null)
        {
            _dialogText.text = msg;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_UpdateStatusText(byte[] compressedMsg, float timer = 3)
    {
        string msg = DecompressString(compressedMsg);
        Debug.Log($"RPC_UpdateStatusText is executed: {msg} at {Time.time}");
        if (_statusText != null)
        {
            _statusText.text = msg;
        }
    }

    public void UpdateStatusText(string msg, float timer = 3)
    {
        Debug.Log($"Updating status text: {msg} at {Time.time}");
        byte[] compressedMsg = CompressString(msg);
        RPC_UpdateStatusText(compressedMsg, timer);
    }

    public void UpdateDialogText(string msg)
    {
        Debug.Log($"Updating dialog text: {msg} at {Time.time}");
        byte[] compressedMsg = CompressString(msg);
        RPC_UpdateDialogText(compressedMsg);
    }

    void SetListening(bool bNew)
    {
        if (_animator != null)
        {
            _animator.SetBool("Listening", bNew);
        }
    }

    void SetTalking(bool bNew)
    {
        if (_animator != null)
        {
            _animator.SetBool("Talking", bNew);
        }
    }

    public void SetGoogleAPIKey(string key)
    {
        _googleAPIkey = key;
    }

    public void SetOpenAI_APIKey(string key)
    {
        _openAI_APIKey = key;
    }
    public void SetOpenAI_Model(string model)
    {
        _openAI_APIModel = model;
    }

    public void SetElevenLabsAPIKey(string key)
    {
        _elevenLabsAPIkey = key;
    }

    public string GetAdvicePrompt()
    {
        return _activeFriend._advicePrompt;
    }

    public int CountWords(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return 0;
        }

        string[] words = input.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        return words.Length;
    }

    string GetBasePrompt()
    {
        return _activeFriend._basePrompt;
    }

    string GetDirectionPrompt()
    {
        return _activeFriend._directionPrompt;
    }

    void TrimHistoryIfNeeded()
    {
        int tokenSize = CountWords(GetBasePrompt());
        int historyTokenSize = 0;

        foreach (GTPChatLine line in _chatHistory)
        {
            historyTokenSize += CountWords(line._content);
        }

        int maxTokenUseForPromptsAndHistory = tokenSize + _activeFriend._friendTokenMemory;

        if (tokenSize + historyTokenSize > maxTokenUseForPromptsAndHistory)
        {
            while (tokenSize + historyTokenSize > maxTokenUseForPromptsAndHistory)
            {
                GTPChatLine line = _chatHistory.Dequeue();
                historyTokenSize -= CountWords(line._content);
                line = _chatHistory.Dequeue();
                historyTokenSize -= CountWords(line._content);
            }
        }

        Debug.Log("Prompt tokens: " + tokenSize + " History token size:" + historyTokenSize);
    }

    public int GetFriendIndex()
    {
        if (_activeFriend == null)
            return 0;
        else
            return _activeFriend._index;
    }

    public void ForgetStuff()
    {
        Debug.Log("ForgetStuff Aufgerufen");
        _chatHistory.Clear();
        StopTalking();
    }

    public void OnForgetButton()
    {
        PlayClickSound();
        RPC_ForgetStuff();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ForgetStuff()
    {
        _chatHistory.Clear();
        StopTalking();
    }

    public void PlayClickSound()
    {
        RTMessageManager.Get().Schedule(0, RTAudioManager.Get().PlayEx, "muffled_bump", 0.5f, 1.0f, false, 0.0f);
    }

    public void StopTalking()
    {
        AudioSource audioSource = _audioSourceToUse;
        audioSource.Stop();
        SetTalking(false);
    }
}
