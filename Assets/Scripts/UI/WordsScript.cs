using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;
using static TimeGrpahScript;
//asdfasdf
public class WordData
{
    public string word;
    public int count;
}

public class WordsScript : MonoBehaviour
{
    HttpController http;

    [SerializeField] TMP_Text detailsBtnText, mostWord;
    [SerializeField] Image detailsImg;
    [SerializeField] Sprite upImg, downImg;
    [SerializeField] GameObject DetailWordsPanel;
    [SerializeField] DetailsPanelScript details;

    private void Start()
    {
        http = HttpController.Instance();
    }

    public IEnumerator SetGraph(DateTime date)
    {
        yield return http.GetMethod("statistics/count/word?date=" + date.ToString("yyyy-MM-dd"), (response) =>
        {
            List<WordData> wordDatas = JsonConvert.DeserializeObject<List<WordData>>(response);

            if(wordDatas.Count > 0)
            {
                mostWord.text = wordDatas[0].word + "(" + wordDatas[0].count + ")";
                details.SetDetails(wordDatas);
            }
            else
            {
                mostWord.text = "욕설 사용 안 함!! bb";
                details.SetClear();
            }
        });
    }

    public void ToggleDetailsPanel()
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
