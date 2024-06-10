using UnityEngine;

public class ClipBoarController : MonoBehaviour
{
    private AndroidJavaObject clipboardPlugin;

    private static ClipBoarController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public static ClipBoarController Instance()
    {
        if (instance == null)
        {
            return null;
        }
        return instance;
    }

    void Start()
    {
        // �ȵ���̵忡���� ClipBoarController Ŭ������ �ν��Ͻ��� �����մϴ�.
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            clipboardPlugin = new AndroidJavaObject("com.example.unityplugin.ClipBoardPlugin", activity);
        }
    }

    // Ŭ�����忡 �ؽ�Ʈ�� �����ϴ� �޼���
    public void CopyToClipboard(string text)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            clipboardPlugin.Call("copyToClipboard", text);
        }
        else
        {
            Debug.Log("Copying to clipboard is not supported on this platform.");
        }
    }
}
