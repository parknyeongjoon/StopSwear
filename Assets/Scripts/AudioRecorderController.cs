using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Profiling;

public class AudioRecorderController : MonoBehaviour
{
    private static AudioRecorderController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public static AudioRecorderController Instance()
    {
        if (instance == null)
        {
            return null;
        }
        return instance;
    }

    private AndroidJavaObject recorder;
    UIManager uiManager;

    public bool isRecord = false;

    private void Start()
    {
        recorder = new AndroidJavaObject("com.example.unityplugin.RecordPlugin");
        uiManager = UIManager.Instance();
    }

    public void StartRecording()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }

        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }

        if (!isRecord)
        {
            // 녹음 시작을 호출하는 코드
            recorder.Call("startRecording");
            isRecord = true;
        }
    }

    public void RecordVoiceCheck()
    {
        recorder.Call("startVoiceCheck");
    }

    public void SendVoiceCheck()
    {
        recorder.Call("sendVoiceCheck");
    }

    public void StopRecording()
    {
        if (isRecord)
        {
            recorder.Call("endRecording");
            isRecord = false;
        }
    }

    ///
    public void Set_JWT_TOKEN(string token)
    {
        recorder.Call("Set_JWT_TOKEN", token);
    }
}