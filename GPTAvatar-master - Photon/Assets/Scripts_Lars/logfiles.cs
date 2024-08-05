using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.UI;

/// <summary>
/// This is an extension for the software GPTAvatar created by Seth Robinson, https://github.com/SethRobinson/GPTAvatar
/// 
/// The class LogFiles is part of OKAPI lab's / Maximilian C. Fink's OKAPILabExtensions for GPTAvatar
/// The class allows to create logfiles for educational/psychological/other research
/// In order to create good logfiles, the xAPI logfile standard is used: https://xapi.com/
/// This extension can be used in the desktop versions of GPTAvatar. It has been tested on windows and could potentially also work on MacOS and Linux.
/// It will probably not work well on WebGL builds and on builds for Android/IOS.
/// 
/// To use this extension, the following steps have to be done
/// 1) Copy this file in Unity somehwere under your assets (e.g., in a separate folder OKAPILabExtensions)
/// 2) Click in the Unity hierarchy on "Canvas" and open the child object "Panel"
/// 3) Click on the "Panel" component on "Add Component" and then type/select "LogFiles" -> this adds the C# file
/// Now everything should run as intended. No further changes should be neccessary. Try it in play mode and make builds :)
/// 
/// When using GPTAvatar the following has to be done
/// 1) Enter the PIN/code of the user for whom you want to create logfile in the textbox that appears upon start and presse enter
/// -> this will be the file name of the created .csv
/// 2) After using GPTAvatar, the log file will be in this folder 
/// C:\Users\USERNAME\AppData\LocalLow\Robinson Technologies\GPTAvatar\
/// 3) Search for the user name
/// 4) For correct formating of the .csv file, click on column A. Then click on data -> text in columns -> separated -> comma as seperator
///
/// If you use GPTAvatar for psychological/educational research, please either cite our article 
/// http://www.dx.doi.org/10.3389/feduc.2024.1416307/abstract or Seth Robinson's github repository.
/// If you need more code modification or want to collaborate for scientific purposes, please reach out to Maximilian C. Fink (maximilian.fink@yahoo.com)
/// I'm always happy to help and conduct research together :)
/// </summary>

// Write logfiles According to xAPI standard
public class logfiles : MonoBehaviour
{
    private StreamWriter streamWriter;
    private string userNumber;
    private string userRequestTranscribed;
    private string answerByLLM;

    [SerializeField]
    private TMP_InputField userInputField; // the field for storing the user number

    [SerializeField]
    private GameObject panel;

    [SerializeField]
    private TMP_Text userRequestTextComponent; // set to "statusText" in the editor

    [SerializeField]
    private TMP_Text answerByLLMComponent; // set to "DialogText" in the editor!

    private void Awake()
    {
       
        setUpPanel();
        setUpInputField();
    }

    // finding the panel where objects are put on
    // Find the panel GameObject
    private void setUpPanel()
    {
        // Find the panel GameObject
        panel = GameObject.Find("Panel2");

        if (panel == null)
        {
            Debug.LogError("Panel with name 'Panel' not found!");
            return;
        }

    }

    private void setUpInputField()
    {
        // Create a new GameObject for the TMP_InputField
        GameObject inputFieldObject = new GameObject("userInputField");

        // Set the parent of the input field to the panel
        inputFieldObject.transform.SetParent(panel.transform);

        // Add a RectTransform component (required for UI elements)
        RectTransform rectTransform = inputFieldObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(250, 50); // Set the size of the input field
        rectTransform.localScale = Vector3.one;

        // Position it within the panel
        rectTransform.anchoredPosition = new Vector2(0, 0); // Adjust as needed for positioning within the panel
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Add an Image component for the background
        Image backgroundImage = inputFieldObject.AddComponent<Image>();
        backgroundImage.color = Color.white; // Set the background color

        // Add the TMP_InputField component and assign it to userInputField2
        userInputField = inputFieldObject.AddComponent<TMP_InputField>();

        // Add a TextMeshProUGUI component (for the input field text)
        GameObject textGameObject = new GameObject("Text");
        textGameObject.transform.SetParent(inputFieldObject.transform);
        TextMeshProUGUI textComponent = textGameObject.AddComponent<TextMeshProUGUI>();
        textComponent.text = "Type VP-Number & Enter";
        textComponent.enableWordWrapping = false;
        textComponent.extraPadding = true;
        textComponent.fontSize = 18; // Adjust the font size as needed
        textComponent.alignment = TextAlignmentOptions.MidlineJustified;
        textComponent.color = Color.black; // Set the font color to black

        // Set the RectTransform of the text component to match the input field
        RectTransform textRectTransform = textComponent.GetComponent<RectTransform>();
        textRectTransform.sizeDelta = rectTransform.sizeDelta;
        textRectTransform.anchorMin = Vector2.zero;
        textRectTransform.anchorMax = Vector2.one;
        textRectTransform.offsetMin = new Vector2(0, 0); // Set padding on the left
        textRectTransform.offsetMax = new Vector2(0, 0); // Set padding on the right

        // Assign the text component to the TMP_InputField
        userInputField.textComponent = textComponent;

        // Ensure the RectTransform is properly set
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one;

        // Use anonymous functions to add event listeners
        userInputField.onEndEdit.AddListener(delegate { writeUserNumberOnEnter(); });
        userInputField.onEndEdit.AddListener(delegate { MakeInputFieldInvisible(inputFieldObject); });
    }

    // Method to make the input field invisible
    private void MakeInputFieldInvisible(GameObject inputFieldObject)
    {
        inputFieldObject.SetActive(false);
    }

    // the start method searches and assigns all components
    // on the different gameObjects ONLY if they are not manually set
    // it should work this way in GPTAvatar, but you can modify the references here if necessary
    private void Start()
    {

        // --- Set the userRequestTextComponent
        // Check if the GameObject was manually set / is not set
        if (userRequestTextComponent == null)
        {
            // Get the TextMeshPro component attached to the GameObject
            userRequestTextComponent = GameObject.Find("StatusText").GetComponent<TMP_Text>();

            // Check if the TextMeshProUGUI component was found
            if (userRequestTextComponent != null)
            {
                Debug.Log("userRequestTextComponent found");
            }
            else
            {
                Debug.Log("userRequestTextComponent not found");
            }
        }
        else
        {
            Debug.Log("GameObject named 'userRequestTextComponent' was manually set");
        }

        // --- Set the answerByLLMComponent
        // Check if the GameObject was manually set / is not set
        if (answerByLLMComponent == null)
        {
            // Get the TextMeshPro component attached to the GameObject
            answerByLLMComponent = GameObject.Find("DialogText").GetComponent<TMP_Text>();

            // Check if the TextMeshProUGUI component was found
            if (answerByLLMComponent != null)
            {
                Debug.Log("answerByLLMComponent found");
            }
            else
            {
                Debug.Log("answerByLLMComponent not found");
            }
        }
        else
        {
            Debug.Log("GameObject named 'answerByLLMComponent' was manually set");
        }

        // --- Set the userInputField
        // Check if the GameObject was manually set / is not set
        if (userInputField == null)
        {
            // Get the TextMeshPro component attached to the GameObject
            userInputField = GameObject.Find("UserNumberInputField").GetComponent<TMP_InputField>();

            // Check if the TextMeshProUGUI component was found
            if (userInputField != null)
            {
                Debug.Log("userInputField found");
            }
            else
            {
                Debug.Log("userInputField not found");
            }
        }
        else
        {
            Debug.Log("GameObject named 'userInputField' was manually set");
        }


        // --- Create a start file for initializing the method and set references
        CreateNewFile("start.csv");


    }

    // Method to create a new file
    public void CreateNewFile(string fileName)
    {

        // create the file path
        string filePath = Path.Combine(Application.persistentDataPath, fileName); // persistant dataPath writes on my Machine to C:\Users\llm-dtec\AppData\LocalLow\Robinson Technologies\GPTAvatar
        Debug.Log("Creating file at: " + filePath);

        try
        {
            // Ensure any existing writer is closed
            CloseFile();

            // Create a new StreamWriter instance
            streamWriter = new StreamWriter(filePath, false);

            // Write CSV header
            streamWriter.WriteLine("id,actor,verb,object,result,context,timestamp,stored,authority,attachments");
            Debug.Log("File created successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to create file: " + e.Message);
        }
    }

    // Update method to check for text changes
    private void Update()
    {
        // check for changes in this variable
        if (userRequestTextComponent.text != userRequestTranscribed)
        {
            string placeholder = userRequestTextComponent.text;
            string plainText = Regex.Replace(placeholder, "<.*?>", String.Empty);

            userRequestTranscribed = plainText;
            userRequestTextComponent.text = plainText;//necessary to override this!

            if (userRequestTranscribed.Equals("Thinking...") ||
                userRequestTranscribed.Equals("Clearing throat...") ||
                userRequestTranscribed.Equals("Understanding speech...")
                )
            {
                writeGPTAvatarAction();
                //Debug.Log("The output is " + userRequestTranscribed + "-> WritingAvatarAction");
            }
            else if (userRequestTranscribed.Contains("(AI is thinking) You said:"))
            {
                writeUserRequestTranscribed();
                //Debug.Log("The output is " + userRequestTranscribed + "-> WritingUserRequest/Question");
            }

        }

        // check for changes in this variable
        if (answerByLLMComponent.text != answerByLLM)
        {
            // use regular expressions to filter out html tags
            string placeholder = answerByLLMComponent.text;
            string plainText = Regex.Replace(placeholder, "<.*?>", String.Empty);

            //string plainText = StripRichTagsFromStr(placeholder);
            Debug.Log("plainText" + plainText);

            answerByLLM = plainText;
            answerByLLMComponent.text = plainText;//necessary to override this!
          
            writeLLMAnswer();
            //Debug.Log("The output is " + answerByLLM + " -> Writing logfile for LLMAnswer");
        }
    }

    // Method to add content to the file
    public void AddContent(string id, string actor, string verb, string obj, string result, string context, string timestamp, string stored, string authority, string attachments)
    {
        try
        {
            if (streamWriter == null)
            {
                Debug.LogError("StreamWriter is not initialized. Call CreateNewFile first.");
                return;
            }

            string line = $"{id},{actor},{verb},{obj},{result},{context},{timestamp},{stored},{authority},{attachments}";
            streamWriter.WriteLine(line);
            streamWriter.Flush(); // Ensure the content is written to the file
            Debug.Log("Content added to file: " + line);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to write to file: " + e.Message);
        }
    }

    // This function is called when the user number is submitted
    // it creates a file with the user name and then
    // sets the streamWriter to it
    public void writeUserNumberOnEnter()
    {
        userNumber = userInputField.text;

        if (userNumber == "")
            CreateNewFile("empty.csv");

        CreateNewFile(userNumber + ".csv");

        AddContent(userNumber,
                "GPTAvatar",
                "entered as first user number",
                "GPTAvatar version " + Application.version,
                "true",
                "NA",
                System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssK"),
                "NA",
                "NA",
                "NA");
    }

    public void writeLLMAnswer()
    {
        //answerByLLM = answerByLLMComponent.text;
        // Escape each field by wrapping it in double quotes and escaping existing double quotes

        AddContent(userNumber,
                "LLM",
                "spoke",
                "\"" + answerByLLM + "\"",
                "true",
                "Answer generated by LLM",
                System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssK"),
                "NA",
                "NA",
                "NA");
    }

    public void writeGPTAvatarAction()
    {
        AddContent(userNumber,
                "GPTAvatar",
                "performed",
                "\"" + userRequestTranscribed + "\"",
                "true",
                "Status update by GPTAvatar",
                System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssK"),
                "NA",
                "NA",
                "NA");
    }

    // escape characters are necessary
    // https://stackoverflow.com/questions/8090759/how-to-write-a-value-which-contain-comma-to-a-csv-file-in-c
    public void writeUserRequestTranscribed()
    {
        //userRequestTranscribed = userRequestTextComponent.text;

        // Watch out, we are doing a string replacement here to filter out "(AI is thinking) You said:"
        AddContent(userNumber,
                 "Player",
                "asked",
                 "\"" + userRequestTranscribed.Replace("(AI is thinking) You said:", "") + "\"",
                "true",
                "Transcription of voice input (question asked)",
                System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssK"),
                "NA",
                "NA",
                "NA");
    }

    // Ensure the StreamWriter is closed when the instance is destroyed
    private void OnDestroy()
    {
        CloseFile();
    }

    // Method to close the StreamWriter
    public void CloseFile()
    {
        if (streamWriter != null)
        {
            try
            {
                streamWriter.Close();
                streamWriter = null;
                Debug.Log("File closed successfully.");
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to close file: " + e.Message);
            }
        }
    }

}