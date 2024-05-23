using UnityEngine;
using UnityEngine.Profiling;

public class BackGroundUtilityService : MonoBehaviour
{
    AndroidJavaObject screenReceiver;
    AndroidJavaObject context;

    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait; // ���� ���� ����

        screenReceiver = new AndroidJavaObject("com.example.unityplugin.ScreenReceiverManager");
        // �ȵ���̵��� Context�� �����ɴϴ�.
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        screenReceiver.Call("setContext", context);
        screenReceiver.Call("registerScreenReceiver");
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // ���� ��׶���� �̵��� �� ������ �۾� ����
        }
        else
        {
            // ���� ���׶���� ������ �� ������ �۾� ����
        }
    }

    void OnApplicationQuit()
    {
        // ���� ������ ����� �� ������ �۾� ����
        screenReceiver.Call("unregisterScreenReceiver");
    }
}