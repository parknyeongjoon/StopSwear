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
        // 안드로이드에서는 ClipBoarController 클래스의 인스턴스를 생성합니다.
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            clipboardPlugin = new AndroidJavaObject("com.example.unityplugin.ClipBoardPlugin", activity);
        }
    }

    // 클립보드에 텍스트를 복사하는 메서드
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
