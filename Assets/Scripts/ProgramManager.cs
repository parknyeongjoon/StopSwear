using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgramManager : MonoBehaviour
{
    HttpController http;

    ProgramInfo programInfo;

    private void Start()
    {
        http = HttpController.Instance();
    }

    #region Utility

    IEnumerator GetDataByDate(DateTime date)
    {
        yield return wordsByTime.GetCountByDay(date);
        yield return wordsByDate.SetGraph(date);
    }

    IEnumerator SetMyStat()
    {
        if (programInfo == null)
        {
            yield return http.GetMethod("manage/program/get/current", (response) =>
            {
                programInfo = http.GetJsonData<ProgramInfo>(response);
            });
        }
        yield return mostWordByProgram.SetProgramGraph(programInfo.programName);
        wordsByProgram.SetWordsByDay(programInfo.programName);
    }

    IEnumerator SetGroupStat()
    {
        if(programInfo == null)
        {
            yield return http.GetMethod("manage/program/get/current", (response) =>
            {
                programInfo = http.GetJsonData<ProgramInfo>(response);
            });
        }
        studentGroupStat.SetPanel(programInfo.programName);
    }

    #endregion

    #region UI
    [SerializeField] GameObject waitingProgressPanel, runningProgressPanel, myStatPanel, groupStatPanel;
    [SerializeField] InfoPanel infoPanel;
    [SerializeField] Image[] dayToggleImg;
    [SerializeField] TimeGrpahScript wordsByTime;
    [SerializeField] WordsScript wordsByDate;
    [SerializeField] LineGraphScript wordsByProgram;
    [SerializeField] WordsScript mostWordByProgram;
    [SerializeField] StudentGroupStatPanel studentGroupStat;

    public void DayBtnAnim(int day) // 월:0, 화:1, 수:2, 목:3, 금:4
    {
        for(int i = 0; i < 5; i++)
        {
            if(i == day)
            {
                dayToggleImg[i].color = new Color(100, 100, 100);
                DateTime today = DateTime.Today;
                DateTime date = today.AddDays(day - (int)today.DayOfWeek + 1);
                StartCoroutine(GetDataByDate(date));
            }
            else
            {
                dayToggleImg[i].color = new Color(255, 255, 255);
            }
        }
    }
    #endregion

    public void OpenMyStat()
    {
        myStatPanel.SetActive(true);
        groupStatPanel.SetActive(false);
        StartCoroutine(SetMyStat());
    }

    public void OpenGroupStat()
    {
        myStatPanel.SetActive(false);
        groupStatPanel.SetActive(true);
        StartCoroutine(SetGroupStat());
    }
}
