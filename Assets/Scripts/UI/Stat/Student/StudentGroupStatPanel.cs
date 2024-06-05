using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StudentGroupStatPanel : MonoBehaviour
{
    HttpController http;

    void Start()
    {
        http = HttpController.Instance();
    }

    #region Utility
    public void SetGroupStat(ProgramInfo program)
    {
        StartCoroutine(GetMyRank(program));
        StartCoroutine(rankGraph.GetRanksByDate(program));
        StartCoroutine(GetGroupWords(program));
    }

    IEnumerator GetMyRank(ProgramInfo program)
    {
        yield return new WaitUntil(() => http != null);
        yield return http.GetMethod("statistics/rank/class?date=" + DateTime.Today.ToString("yyyy-MM-dd"), (response) => {
            groupRankTxt.text = response;
        });
        yield return http.GetMethod("manage/class/program/count?date=" + DateTime.Today.ToString("yyyy-MM-dd"), (response) =>
        {
            groupRankTxt.text = groupRankTxt.text + " / " + response.ToString();
        });
        yield return http.GetMethod("statistics/rank?programName=" + program.programName, (response) =>
        {
            myRankTxt.text = response;
        });
        yield return http.GetMethod("manage/students/program/count?programName=" + program.programName, (response) =>
        {
            myRankTxt.text = myRankTxt.text + " / " + response.ToString();
        });
    }

    IEnumerator GetGroupWords(ProgramInfo program)
    {
        yield return new WaitUntil(() => http != null);
        yield return http.GetMethod("statistics/count/word/group?programName=" + program.programName, (response) =>
        {
            List<WordData> wordDatas = JsonConvert.DeserializeObject<List<WordData>>(response);
            detailGraph.SetDetails(wordDatas);
            groupWordsCircleGraph.SetCircleGraph(wordDatas);
        });
    }
    #endregion

    #region UI
    [SerializeField] TMP_Text groupRankTxt, myRankTxt;
    [SerializeField] LineGraphScript rankGraph;
    [SerializeField] CircleGraphScript groupWordsCircleGraph;
    [SerializeField] DetailsPanelScript detailGraph;
    #endregion
}
