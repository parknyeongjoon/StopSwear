using UnityEngine;
using System.Net;
using System.IO;
using System.Text;

public class SimpleHttpServer : MonoBehaviour
{
    private HttpListener httpListener;
    private bool isRunning = false;

    // ��Ʈ ��ȣ
    public int port = 8080;

    private void Start()
    {
        StartServer();
    }

    // ���� ����
    public void StartServer()
    {
        if (!HttpListener.IsSupported)
        {
            Debug.LogError("HTTP Listener is not supported on this platform.");
            return;
        }

        httpListener = new HttpListener();
        httpListener.Prefixes.Add("http://localhost:" + port + "/");
        httpListener.Start();
        isRunning = true;
        Debug.Log("���� ���۵�");

        // ��û ���
        WaitForRequest();
    }

    // ���� ����
    public void StopServer()
    {
        if (isRunning)
        {
            httpListener.Stop();
            isRunning = false;
            Debug.Log("���� �����");
        }
    }

    // ��û ���
    private async void WaitForRequest()
    {
        while (isRunning)
        {
            HttpListenerContext context = await httpListener.GetContextAsync();
            HandleRequest(context);
        }
    }

    // ��û ó��
    private void HandleRequest(HttpListenerContext context)
    {
        HttpListenerRequest request = context.Request;
        Debug.Log("��û ����: " + request.HttpMethod + " " + request.Url);

        // ��û ���� �б�
        StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding);
        string requestBody = reader.ReadToEnd();
        Debug.Log("��û ����: " + requestBody);

        // ���� ������
        string responseString = "��û�� �����߽��ϴ�: " + request.HttpMethod + " " + request.Url + "\n��û ����: " + requestBody;
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        context.Response.ContentLength64 = buffer.Length;
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        context.Response.OutputStream.Close();
    }

    private void OnApplicationQuit()
    {
        StopServer();
    }
}