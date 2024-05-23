using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StudentGroupStatPanel : MonoBehaviour
{
    HttpController http;

    private void Awake()
    {
        http = HttpController.Instance();
    }

    private void OnEnable()
    {
        DateTime tempStart = DateTime.Now;
        DateTime tempEnd = DateTime.Now;
        GetData(tempStart, tempEnd);
    }

    #region Utility
    void GetData(DateTime startTime, DateTime endTime)
    {
        StartCoroutine(GetMyRank(startTime, endTime));
        StartCoroutine(GetGroupWords(startTime, endTime));
    }

    IEnumerator GetMyRank(DateTime startTime, DateTime endTime)
    {
        yield return http.GetMethod("statistics/rank", (response) =>
        {
            Debug.Log("my rank" + response);
            //myRankTxt.text = response;
        });
    }

    IEnumerator GetGroupWords(DateTime startTime, DateTime endTime)
    {
        yield return http.GetMethod("statistics/count/word/group", (response) =>
        {
            Debug.Log("group stat" + response);
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
