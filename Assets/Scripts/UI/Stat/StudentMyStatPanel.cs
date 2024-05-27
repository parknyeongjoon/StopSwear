using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;

public class StudentMyStatPanel : MonoBehaviour
{
    #region Utility
    public void SetMyStat(string programName)
    {
        StartCoroutine(mostWordByProgram.SetProgramGraph(programName));
        StartCoroutine(wordsByProgram.GetWordsByDay(programName));
    }

    IEnumerator GetDataByDate(DateTime date)
    {
        yield return wordsByTime.GetCountByDay(date);
        yield return wordsByDate.SetWordsGraph(date);
    }

    public void DayBtnAnim(int day) // 월:0, 화:1, 수:2, 목:3, 금:4
    {
        for (int i = 0; i < 5; i++)
        {
            if (i == day)
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

    #region UI
    [SerializeField] Image[] dayToggleImg;
    [SerializeField] TimeGrpahScript wordsByTime;
    [SerializeField] WordsScript wordsByDate;
    [SerializeField] WordsScript mostWordByProgram;
    [SerializeField] LineGraphScript wordsByProgram;

    #endregion

}
