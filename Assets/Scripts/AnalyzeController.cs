using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Dates;
using UnityEngine;
using UnityEngine.UI;

public class Statistics
{
    public string word;
    public int frequency;
}

public class Ratio
{
    public int normal;
    public int profanity;
    public float ratio = 0;
}

public class AnalyzeController : MonoBehaviour
{
    HttpController http;
    UIManager uiManager;
    string fromDate, toDate;

    #region utility
    bool isLoading = true;

    private void Start()
    {
        http = HttpController.Instance();
        uiManager = UIManager.Instance();
    }

    public void Search()
    {
        fromDate = fromDateIF.SelectedDate.Date.ToString("yyyy-MM-dd");
        toDate = toDateIF.SelectedDate.Date.ToString("yyyy-MM-dd");
        StartCoroutine(GetData(fromDate, toDate, -1));
        SetDateText();
    }
    
    IEnumerator GetData(string fromDate, string toDate, int ID)
    {
        LoadingPanel.SetActive(true);
        StartCoroutine(RoateLoadingImg());
        yield return GetMostUsed(fromDate, toDate, ID);
        yield return GetRatio(fromDate, toDate, ID);
        LoadingPanel.SetActive(false);
        InitialPanel.SetActive(false);
    }

    IEnumerator GetMostUsed(string fromDate, string toDate, int ID)
    {
        yield return http.GetMethod("statistics/most-used?size=7&startDate=" + fromDate + "&endDate=" + toDate + "&id=" + "1", (response) =>
        {
            if (response != null)
            {
                string mostWord = response.ToString();
                if (mostWord == null)
                {
                    Debug.Log("욕설 쓴 적 없음");
                }
                else
                {
                    Debug.Log("Received most used response: " + response);

                }
            }
            else
            {
                Debug.LogError("nyeong Log Failed to get data from server.");
                // 오류 처리
            }
        });
    }

    IEnumerator GetPerWord(string fromDate, string toDate, int id)
    {
        yield return http.GetMethod("statistics/per-word?size=" + "7" + "&startDate=" + fromDate + "&endDate=" + toDate + "&id=" + "1", (response) =>
        {
            if (response != null)
            {
                string statistics = response.ToString();
                if (statistics == null)
                {
                    Debug.Log("욕설 쓴 적 없음");
                }
                else
                {
                    Debug.Log("Received per word response: " + response);

                }
            }
            else
            {
                Debug.LogError("nyeong Log Failed to get data from server.");
                // 오류 처리
            }
        });
    }

    IEnumerator GetRatio(string fromDate, string toDate, int ID)
    {
        yield return http.GetMethod("statistics/ratio?startDate=" + fromDate + "&endDate=" + toDate + "&id=" + "1", (response) =>
        {
            if (response != null)
            {
                Ratio ratio = http.GetJsonData<Ratio>(response);
                if((ratio.profanity + ratio.normal) > 0)
                {
                    ratio.ratio = (float)ratio.profanity / (ratio.normal + ratio.profanity);
                }
                else
                {
                    // 욕을 안 쓴 경우 - 뭔가 이벤트 넣어주기
                }
                SetRatioGraph(ratio);
            }
            else
            {
                Debug.LogError("nyeong Log Failed to get data from server.");
                // 오류 처리
            }
        });
    }
    #endregion

    #region UI
    [SerializeField] DatePicker fromDateIF, toDateIF;
    [SerializeField] TMP_Text dateText;
    [SerializeField] GameObject ContentPanel;
    [SerializeField] Image frontCircle;
    [SerializeField] TMP_Text ratioText, normalText, swearText;
    [SerializeField] GameObject DetailWordsPanel;
    [SerializeField] TMP_Text detailsBtnText;
    [SerializeField] Image detailsImg;
    [SerializeField] Sprite downImg, upImg;
    [SerializeField] GameObject LoadingPanel;
    [SerializeField] GameObject loadingImg;
    [SerializeField] GameObject InitialPanel;
    #endregion

    IEnumerator RoateLoadingImg()
    {
        WaitForFixedUpdate rotateWait = new WaitForFixedUpdate();
        while (isLoading)
        {
            //loadingImg.transform.Rotate(Vector3.up, -2.0f);
            loadingImg.transform.Rotate(Vector3.forward, -2.0f);
            yield return rotateWait;
        }
        yield return null;
    }

    void SetDateText()
    {
        dateText.text = fromDate.ToString() + " ~ " + toDate.ToString();
    }

    void SetRatioGraph(Ratio ratio)
    {
        frontCircle.fillAmount = ratio.ratio;
        ratioText.text = ratio.ratio.ToString("F3");
        normalText.text = "비욕설 횟수: " + ratio.normal.ToString();
        swearText.text = "욕설 횟수: " + ratio.profanity.ToString();
    }

    public void ControllDetailWordsBtn()
    {
        if (DetailWordsPanel.activeSelf)
        {
            DetailWordsPanel.SetActive(false);
            detailsBtnText.text = "show details";
            detailsImg.sprite = downImg;
        }
        else
        {
            DetailWordsPanel.SetActive(true);
            detailsBtnText.text = "close details";
            detailsImg.sprite = upImg;
        }
    }
}
