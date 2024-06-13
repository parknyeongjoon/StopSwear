using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeacherGroupStatPanel : MonoBehaviour
{
    HttpController http;

    float originHeight = 2600;

    void Start()
    {
        http = HttpController.Instance();
    }

    #region Utility
    public void SetGroupStat(ProgramInfo program)
    {
        StartCoroutine(GetGroupRank());
        StartCoroutine(GetBastards(program));
        StartCoroutine(GetGroupWords(program));
    }

    IEnumerator GetGroupRank()
    {
        yield return new WaitUntil(() => http != null);
        yield return http.GetMethod("statistics/rank/class?date=" + DateTime.Today.ToString("yyyy-MM-dd"), (response) => {
            rankTxt.text = response;
        });
        yield return http.GetMethod("manage/class/program/count?date=" + DateTime.Today.ToString("yyyy-MM-dd"), (response) =>
        {
            rankTxt.text = rankTxt.text + " / " + response.ToString();
        });
    }

    IEnumerator GetBastards(ProgramInfo program)
    {
        yield return new WaitUntil(() => http != null);
        yield return http.GetMethod("manage/students/program?programName=" + program.programName + "&sorted=true", (response) =>
        {
            List<UserInfo> students = JsonConvert.DeserializeObject<List<UserInfo>>(response);
            int size = 3;
            if(size > students.Count)
            {
                size = students.Count;
            }

            groupRect.sizeDelta = new Vector2(groupRect.sizeDelta.x, originHeight + 250 * size);

            for(int i = 0; i < size; i++)
            {
                bastards[i].gameObject.SetActive(true);
                StartCoroutine(SetStudentCard(program, students[i], i));
            }

            for(int i = size; i < 3; i++)
            {
                bastards[i].gameObject.SetActive(false);
            }
        });
    }

    IEnumerator SetStudentCard(ProgramInfo program, UserInfo student, int index)
    {
        string rank = "-1", total_count = "temp", most_word = "temp";
        yield return http.GetMethod("statistics/rank/" + student.id + "?programName=" + program.programName, (response) =>
        {
            rank = response;
        });
        yield return http.GetMethod("statistics/most-used/program/" + student.id + "?programName=" + program.programName, (response) =>
        {
            if (response == null || response == "")
            {
                most_word = "¿å¼³ »ç¿ë x";
            }
            else
            {
                most_word = response;
            }
        });
        yield return http.GetMethod("statistics/count/daily/" + student.id + "?programName=" + program.programName, (response) =>
        {
            WordsByProgram data = http.GetJsonData<WordsByProgram>(response);
            total_count = data.sum.ToString() + "È¸";
        });
        bastards[index].SetText(student.name, rank, total_count, most_word);
    }

    IEnumerator GetGroupWords(ProgramInfo program)
    {
        yield return new WaitUntil(() => http != null);
        yield return http.GetMethod("statistics/count/word/group?programName=" + program.programName, (response) =>
        {
            List<WordData> wordDatas = JsonConvert.DeserializeObject<List<WordData>>(response);
            detailGraph.SetDetails(wordDatas);
            circleGraph.SetCircleGraph(wordDatas);
        });
    }
    #endregion

    #region UI
    [SerializeField] TMP_Text rankTxt;
    [SerializeField] DetailsPanelScript detailGraph;
    [SerializeField] CircleGraphScript circleGraph;
    [SerializeField] StudentCard[] bastards;
    [SerializeField] GameObject studentCard;
    [SerializeField] RectTransform groupRect;
    #endregion
}