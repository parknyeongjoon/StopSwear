using UnityEngine;
using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;

public class HttpController : MonoBehaviour
{
    private static HttpController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public static HttpController Instance()
    {
        if (instance == null)
        {
            return null;
        }
        return instance;
    }

    string apiUrl = "https://www.noswear.p-e.kr:8080/";

    AndroidJavaObject httpPlugin;
    Dictionary<string, string> headers = new Dictionary<string, string>();

    private void Start()
    {
        httpPlugin = new AndroidJavaObject("com.example.unityplugin.HttpPlugin");
    }

    public void SetJwtToken(string token)
    {
        if (headers.ContainsKey("Authorization"))
        {
            headers["Authorization"] = "Bearer " + token;
        }
        else
        {
            headers.Add("Authorization", "Bearer " + token);
        }
    }

    public IEnumerator PostMethod(string query, string jsonData, Action<string> callback)
    {
        UnityWebRequest request = UnityWebRequest.Post(apiUrl + query, jsonData, "application/json");

        foreach (var header in headers)
        {
            request.SetRequestHeader(header.Key, header.Value);
        }

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            // 네트워크 오류 또는 HTTP 오류 처리
            Debug.LogError("Error: " + request.error, this);
            callback(null);
        }
        else
        {
            // 정상적인 응답 처리
            callback(request.downloadHandler.text);
        }
    }

    public IEnumerator GetMethod(string query, Action<string> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl + query);

        foreach (var header in headers)
        {
            request.SetRequestHeader(header.Key, header.Value);
        }

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            // 네트워크 오류 또는 HTTP 오류 처리
            Debug.LogError("Error: " + request.error, this);
            callback(null);
        }
        else
        {
            // 정상적인 응답 처리
            //Debug.Log("Received: " + request.downloadHandler.text);
            callback(request.downloadHandler.text);
        }
    }

    public IEnumerator DeleteMethod(string query, Action<string> callback)
    {
        using UnityWebRequest request = UnityWebRequest.Delete(apiUrl + query);
        // 필요시 헤더 추가
        foreach (var header in headers)
        {
            request.SetRequestHeader(header.Key, header.Value);
        }

        // 요청 보내기
        yield return request.SendWebRequest();

        // 에러 핸들링
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            callback(null);
        }
        else
        {
            //Debug.Log("Response: " + request.downloadHandler.text);
            callback(null);
        }
    }

    public T GetJsonData<T>(string response)
    {
        T JsonData = JsonUtility.FromJson<T>(response.ToString());
        return JsonData;
    }

    /* 사용 예시
    public void CallGetMethod()
    {
        StartCoroutine(GetDataFromServer());
    }

    // GetDataFromServer 코루틴
    private IEnumerator GetDataFromServer()
    {
        string query = ""; // 원하는 쿼리 문자열 설정

        // GetMethod 호출
        yield return StartCoroutine(GetMethod(query, (responseData) =>
        {
            // 응답 데이터 처리
            if (responseData != null)
            {
                Debug.Log("Received response: " + responseData);
                // 여기서 responseData를 이용한 추가적인 처리를 할 수 있습니다.
            }
            else
            {
                Debug.LogError("Failed to get data from server.");
                // 오류 처리
            }
        }));
    }
     */

    /*public void JavaGetMethod()
    {
        httpPlugin.Call("GetJsonMethod", "statistics");
    }
    
    public void JavaPostMethod()
    {
        httpPlugin.Call("PostAudioMethod");
    }

    void ReceiveJSONArrayFromAndroid(string jsonString)
    {
        List<Statistics> statistics_list = new List<Statistics>();
        JArray jsonArray = JArray.Parse(jsonString);
        // JSON ��ü�� ��� Ű�� ������
        foreach (var pair in jsonArray)
        {
            Statistics temp = new Statistics();
            temp.word = pair.Value<string>("word");
            temp.frequency = pair.Value<int>("frequency");
            statistics_list.Add(temp);
        }
        uiManager.SetAnalizeText(statistics_list);
    }

    byte[] fileBytes;
    // HTTP ��û�� ���� �Լ�
    public async void PostAudioFile()
    {
        try
        {
            // ������ ����Ʈ �迭�� �б�
            fileBytes = File.ReadAllBytes(audioFilePath + auidoFileName);

            // HTTP ��û ����
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
            request.Method = "POST";
            request.ContentType = "audio/mp4"; // m4a ���� ���Ŀ� ���� ������ content type�� ����

            // ������ ��û�� ����
            using (Stream requestStream = await request.GetRequestStreamAsync())
            {
                await requestStream.WriteAsync(fileBytes, 0, fileBytes.Length);
            }

            // ���� �ޱ�
            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            {
                Debug.Log("���� ���� �ڵ�: " + response.StatusCode);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("���ε� ����: " + e.Message);
        }
    }*/

    /*public async void GetMethod(string query)
    {
        try
        {
            // HTTP ��û ����
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl + query);
            request.Method = "GET";

            // ���� �ޱ�
            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            {
                // ���� ��Ʈ�� �б�
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    reader.ReadToEnd();
                }
            }
        }
        catch (Exception ex)
        {
            // ���� ó��
            Console.WriteLine("Error while making GET request: " + ex.Message);
            return;
        }
    }*/

    /*public async void GetFIle()
    {
        string filePath = "C:\\Users\\qkrsudwns\\Desktop\\example.m4a"; // ������ ���� ���� ���
        try
        {
            // ����Ʈ �迭�� ���Ͽ� ��
            File.WriteAllBytes(filePath, fileBytes);
            Debug.Log("����Ʈ �迭�� ���� ���Ϸ� ���������� ��ȯ�߽��ϴ�.");
        }
        catch (Exception e)
        {
            Debug.Log("Error converting bytes to music file: " + e.Message);
        }
    }*/
}