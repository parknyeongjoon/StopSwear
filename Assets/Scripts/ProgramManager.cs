using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class ProgramManager : MonoBehaviour
{
    HttpController http;

    private void Start()
    {
        http = HttpController.Instance();
        StartCoroutine(CheckProgram());
    }

    #region Utility

    IEnumerator CheckProgram()
    {
        yield return http.GetMethod("manage/program/get/current", (response) =>
        {
            if (response != null)
            {
                runningProgressPanel.SetActive(true);
                waitingProgressPanel.SetActive(false);
            }
        });
    }

    IEnumerator GetDataByDate(DateTime date)
    {
        yield return wordsByTime.GetCountByDay(date);
        yield return wordsByDate.SetGraph(date);
    }

    #endregion

    #region UI
    [SerializeField] GameObject waitingProgressPanel, runningProgressPanel, myStatPanel, groupStatPanel;
    [SerializeField] Image[] dayToggleImg;
    [SerializeField] TimeGrpahScript wordsByTime;
    [SerializeField] WordsScript wordsByDate;

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
    }

    public void OpenGroupStat()
    {
        myStatPanel.SetActive(false);
        groupStatPanel.SetActive(true);
    }

    public void CheckProgramBtn()
    {
        StartCoroutine(CheckProgram());
    }
}
