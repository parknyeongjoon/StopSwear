using UnityEngine;
using UnityEngine.Profiling;

public class BackGroundUtilityService : MonoBehaviour
{
    AndroidJavaObject screenReceiver;
    AndroidJavaObject context;

    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait; // 세로 모드로 고정

        screenReceiver = new AndroidJavaObject("com.example.unityplugin.ScreenReceiverManager");
        // 안드로이드의 Context를 가져옵니다.
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        screenReceiver.Call("setContext", context);
        screenReceiver.Call("registerScreenReceiver");
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // 앱이 백그라운드로 이동할 때 실행할 작업 수행
        }
        else
        {
            // 앱이 포그라운드로 복귀할 때 실행할 작업 수행
        }
    }

    void OnApplicationQuit()
    {
        // 앱이 완전히 종료될 때 실행할 작업 수행
        screenReceiver.Call("unregisterScreenReceiver");
    }
}