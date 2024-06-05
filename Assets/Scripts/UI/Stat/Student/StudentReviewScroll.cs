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

        StartCoroutine(GetRank(info.programName));
        StartCoroutine(GetGroupWords(info.programName));
    }

    void SetProgramInfo(ProgramInfo info)
    {
        programInfo.text = info.programName + "\n" + info.startDate + " ~ " + info.endDate;
    }

    IEnumerator GetRank(string programName)
    {
        yield return new WaitUntil(() => http != null);
        yield return http.GetMethod("statistics/rank/class?date=" + DateTime.Today.ToString("yyyy-MM-dd"), (response) => {
            classRank.text = response;
        });
        yield return http.GetMethod("manage/class/program/count?date=" + DateTime.Today.ToString("yyyy-MM-dd"), (response) =>
        {
            classRank.text = classRank.text + " / " + response.ToString();
        });
        yield return http.GetMethod("statistics/rank?programName=" + programName, (response) =>
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
    [SerializeField] TMP_Text classRank, myRank;
    [SerializeField] CircleGraphScript groupWordsCircleGraph;
    [SerializeField] DetailsPanelScript detailGraph;

    #endregion
}
