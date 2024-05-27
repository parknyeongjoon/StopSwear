using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StudentGroupStatPanel : MonoBehaviour
{
    HttpController http;

    void Start()
    {
        http = HttpController.Instance();
    }

    #region Utility
    public void SetGroupStat(string programName)
    {
        StartCoroutine(GetMyRank());
        StartCoroutine(GetGroupWords(programName));
    }

    IEnumerator GetMyRank()
    {
        yield return new WaitUntil(() => http != null);
        yield return http.GetMethod("statistics/rank", (response) =>
        {
            groupRankTxt.text = response + " / n µî";
        });
    }

    IEnumerator GetGroupWords(string programName)
    {
        yield return new WaitUntil(() => http != null);
        yield return http.GetMethod("statistics/count/word/group?programName=" + programName, (response) =>
        {
            List<WordData> wordDatas = JsonConvert.DeserializeObject<List<WordData>>(response);
            detailGraph.SetDetails(wordDatas);
        });
    }
    #endregion

    #region UI
    [SerializeField] TMP_Text groupRankTxt;
    [SerializeField] LineGraphScript rankGraph;
    [SerializeField] CircleGraphScript groupWordsCircleGraph;
    [SerializeField] DetailsPanelScript detailGraph;
    #endregion
}
