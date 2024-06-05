using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] RectTransform scrollContent;

    private void Start()
    {
        http = HttpController.Instance();
    }

    public IEnumerator SetWordsGraph(DateTime date, int id)
    {
        yield return new WaitUntil(() => http != null);
        string query = "statistics/count/word";
        if(id != 0)
        {
            query += "/" + id.ToString();
        }
        query += "?date=" + date.ToString("yyyy-MM-dd");
        yield return http.GetMethod(query, (response) =>
        {
            List<WordData> wordDatas = JsonConvert.DeserializeObject<List<WordData>>(response);

            if(response == null || wordDatas.Count <= 0)
            {
                mostWord.text = "욕설 사용 안 함!! bb";
                details.SetClear();
            }
            else
            {
                mostWord.text = wordDatas[0].word + "(" + wordDatas[0].count + ")";
                details.SetDetails(wordDatas);
            }
        });
    }

    public IEnumerator SetProgramGraph(ProgramInfo program, int id)
    {
        yield return new WaitUntil(() => http != null);
        string query = "statistics/count/word/program";
        if(id != 0)
        {
            query += "/" + id.ToString();
        }
        query += "?programName=" + program.programName;
        yield return http.GetMethod(query, (response) =>
        {
            List<WordData> wordDatas = JsonConvert.DeserializeObject<List<WordData>>(response);

            if (response == null || response == "" || wordDatas.Count <= 0)
            {
                mostWord.text = "욕설 사용 안 함!! bb";
                details.SetClear();
            }
            else
            {
                mostWord.text = wordDatas[0].word + "(" + wordDatas[0].count + ")";
                details.SetDetails(wordDatas);
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
            scrollContent.sizeDelta = new Vector2(scrollContent.sizeDelta.x, scrollContent.sizeDelta.y - 900);
        }
        else
        {
            DetailWordsPanel.SetActive(true);
            detailsBtnText.text = "close details";
            detailsImg.sprite = upImg;
            scrollContent.sizeDelta = new Vector2(scrollContent.sizeDelta.x, scrollContent.sizeDelta.y + 900);
        }
    }
}
