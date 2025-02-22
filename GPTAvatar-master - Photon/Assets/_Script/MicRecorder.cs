using UnityEngine;
using System;

#if unusedjunk
using NAudio.Wave;

public class MicRecorder : MonoBehaviour
{
    private WaveInEvent waveIn;
    private MemoryStream memoryStream;
    private WaveFileWriter writer;

    bool isRecording = false;
    string outputFileName = "output.wav";

    void Start()
    {
    }

    void StartRecording()
    {
        Debug.Log("Starting recording system");
        waveIn = new WaveInEvent();
        waveIn.DeviceNumber = 0; // Use the default microphone
        waveIn.WaveFormat = new WaveFormat(44100, 16, 1); // 44100Hz, Mono
        
        writer = new WaveFileWriter(outputFileName, waveIn.WaveFormat);

        waveIn.DataAvailable += (sender, e) =>
        {
            if (isRecording)
            {
                writer.Write(e.Buffer, 0, e.BytesRecorded);
            }
        };

        // Start recording audio from the microphone
        waveIn.StartRecording();
        isRecording = true;

        Console.WriteLine("Recording audio. Press S to stop recording...");
    }

    private void Update()
    {
        if (!isRecording && Input.GetKey(KeyCode.M))
        {
            Debug.Log("Recording started");
            StartRecording();
        }
        
        if (isRecording && !Input.GetKey(KeyCode.M))
        {
            Debug.Log("Recording stopped");
            waveIn.StopRecording();
            writer.Close();
            isRecording = false;

            AIManager aiScript = GetComponent<AIManager>();
            //OPTIMIZE: Pass the .wav bytes directly instead of writing/reading from an actual file?
            aiScript.ProcessMicAudioByFileName(outputFileName); 
        }
    }
}
#else

public class MicRecorder : MonoBehaviour
{
    private AudioClip audioClip;
    bool isRecording = false;

    void Start()
    {
    }

    public void StartRecording()
    {
        Debug.Log("Starting recording system");
        // Start recording audio from the microphone
        audioClip = Microphone.Start(null, false, 80, 22050);
        isRecording = true;
        Console.WriteLine("Recording audio...");
    }

    public void Destroy()
    {
        if (IsRecording())
        {
            Debug.Log("Recording stopped");
            Microphone.End(null);
            isRecording = false;
        }
    }

    public void StopRecordingAndProcess(string outputFileName, string playerName)
    {
        if (!isRecording)
            return;

        Debug.Log("Recording stopped");
        Microphone.End(null);
        isRecording = false;

        // Correctly calculate the length of the audio data
        int length = Mathf.RoundToInt(audioClip.length * audioClip.frequency) * audioClip.channels;
        float[] audioData = new float[length];
        audioClip.GetData(audioData, 0);

        // Convert AudioClip data to a WAV file and save it
        SavWav.Save(outputFileName, audioClip, true);

        AIManager aiScript = GetComponent<AIManager>();
        aiScript.ProcessMicAudioByFileName(outputFileName, playerName); 
    }

    public bool IsRecording() { return isRecording; }

    private void Update()
    {
        /*
        if (!isRecording && Input.GetKey(KeyCode.M))
        {
            Debug.Log("Recording started");
            StartRecording();
        }

        if (isRecording && !Input.GetKey(KeyCode.M))
        {
           StopRecordingAndProcess();
        }
        */
    }
}
#endif

