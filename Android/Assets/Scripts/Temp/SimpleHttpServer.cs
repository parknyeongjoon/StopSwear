using UnityEngine;
using System.Net;
using System.IO;
using System.Text;

public class SimpleHttpServer : MonoBehaviour
{
    private HttpListener httpListener;
    private bool isRunning = false;

    // 포트 번호
    public int port = 8080;

    private void Start()
    {
        StartServer();
    }

    // 서버 시작
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
        Debug.Log("서버 시작됨");

        // 요청 대기
        WaitForRequest();
    }

    // 서버 종료
    public void StopServer()
    {
        if (isRunning)
        {
            httpListener.Stop();
            isRunning = false;
            Debug.Log("서버 종료됨");
        }
    }

    // 요청 대기
    private async void WaitForRequest()
    {
        while (isRunning)
        {
            HttpListenerContext context = await httpListener.GetContextAsync();
            HandleRequest(context);
        }
    }

    // 요청 처리
    private void HandleRequest(HttpListenerContext context)
    {
        HttpListenerRequest request = context.Request;
        Debug.Log("요청 도착: " + request.HttpMethod + " " + request.Url);

        // 요청 본문 읽기
        StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding);
        string requestBody = reader.ReadToEnd();
        Debug.Log("요청 본문: " + requestBody);

        // 응답 보내기
        string responseString = "요청이 도착했습니다: " + request.HttpMethod + " " + request.Url + "\n요청 본문: " + requestBody;
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