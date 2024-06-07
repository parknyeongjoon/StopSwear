using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StudentReviewScroll : MonoBehaviour
{
    HttpController http;

    private void Start()
    {
        http = HttpController.Instance();
    }

    #region Utility

    public void SetReviewScroll(ProgramInfo info, int id)
    {
        SetProgramInfo(info);

        StartCoroutine(mostWordByProgram.SetProgramGraph(info, id));
        StartCoroutine(wordsByProgram.GetWordsByDay(info, id));

        StartCoroutine(GetRank(info.programName, id));
        StartCoroutine(GetGroupWords(info.programName));
    }

    void SetProgramInfo(ProgramInfo info)
    {
        programInfo.text = info.programName + "\n" + info.startDate + " ~ " + info.endDate;
    }

    IEnumerator GetRank(string programName, int id)
    {
        yield return new WaitUntil(() => http != null);
        string query = "statistics/rank";
        if(id != 0)
        {
            query +="/" + id.ToString();
        }
        query += "?programName=" + programName;
        yield return http.GetMethod(query, (response) =>
        {
            myRank.text = response;
        });
        yield return http.GetMethod("manage/students/program/count?programName=" + programName, (response) =>
        {
            myRank.text = myRank.text + " / " + response.ToString();
        });
    }

    IEnumerator GetGroupWords(string programName)
    {
        yield return new WaitUntil(() => http != null);
        yield return http.GetMethod("statistics/count/word/group?programName=" + programName, (response) =>
        {
            List<WordData> wordDatas = JsonConvert.DeserializeObject<List<WordData>>(response);
            detailGraph.SetDetails(wordDatas);
            groupWordsCircleGraph.SetCircleGraph(wordDatas);
        });
    }

    #endregion

    #region UI

    [SerializeField] TMP_Text programInfo;
    [SerializeField] WordsScript mostWordByProgram;
    [SerializeField] LineGraphScript wordsByProgram;
    [SerializeField] TMP_Text myRank;
    [SerializeField] CircleGraphScript groupWordsCircleGraph;
    [SerializeField] DetailsPanelScript detailGraph;

    #endregion
}
