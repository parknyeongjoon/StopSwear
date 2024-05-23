using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToastController : MonoBehaviour
{
    private static ToastController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public static ToastController Instance()
    {
        if (instance == null)
        {
            return null;
        }
        return instance;
    }

    AndroidJavaClass toaster;
    AndroidJavaObject _instance;

    // Start is called before the first frame update
    void Start()
    {
        toaster = new AndroidJavaClass("com.example.unityplugin.ToastPlugin");
        if(toaster != null) { Debug.Log("plugin ºñ¾úÀ½"); }
        _instance = toaster.CallStatic<AndroidJavaObject>("instance");
    }

    public void showToast(string message)
    {
        if (_instance != null)
        {
            _instance.Call("showToast", message);
        }
    }
}