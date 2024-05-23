using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Dates;
using UnityEngine;
using UnityEngine.UI;

public class DateController : MonoBehaviour
{
    private static DateController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public static DateController Instance()
    {
        if (instance == null)
        {
            return null;
        }
        return instance;
    }

    DatePicker_DateRange datePicker;
    public DateTime startTime, endTime;

    public void SetStartNEndTime()
    {
        startTime = datePicker.FromDate.Date;
        if(startTime == null || startTime < DateTime.Now)
        {
            startTime = DateTime.Now;
        }
        endTime = datePicker.ToDate.Date;
        if( endTime == null || endTime < startTime)
        {
            endTime = startTime.AddDays(1);
        }
        SaveDatePlayerPrefs();
    }

    void SaveDatePlayerPrefs()
    {
        PlayerPrefs.SetString("startTime", startTime.ToString("yyyy-MM-dd"));
        PlayerPrefs.SetString("endTime", endTime.ToString("yyyy-MM-dd"));
    }

    void LoadDatePlayerPrefs()
    {
        string temp = PlayerPrefs.GetString("startTime");
        startTime = DateTime.ParseExact(temp, "yyyy-MM-dd", null);
        temp = PlayerPrefs.GetString("endTime");
        endTime = DateTime.ParseExact(temp, "yyyy-MM-dd", null);
    }

    public double CalculateProgressPercent()
    {
        LoadDatePlayerPrefs();
        DateTime curTime = DateTime.Now;
        TimeSpan elapsedTime = curTime - startTime;
        TimeSpan totalTime = endTime - startTime;

        // 진행률 계산 (시작 시간부터 현재까지의 시간 / 전체 시간 * 100)
        double progressPercentage = (elapsedTime.TotalSeconds / totalTime.TotalSeconds) * 100;

        return progressPercentage;
    }

    public bool CheckTime()
    {
        DateTime curTime = DateTime.Now;
        if(curTime > startTime && curTime < endTime )
        {
            return true;
        }
        return false;
    }
}
