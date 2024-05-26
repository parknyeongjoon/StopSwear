using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

public class TimeGrpahScript : MonoBehaviour
{
    [Serializable]
    public class WordsByTime
    {
        public int hour;
        public int count;
    }

    public class WordsByTimeData
    {
        public WordsByTime[] raw;
        public int max;
        public int min;
        public double avg;
    }

    HttpController http;

    [SerializeField] TMP_Text max, avg, min;
    [SerializeField] GameObject tempTxt;
    [SerializeField] RectTransform[] points;
    [SerializeField] int high, low;

    void Start()
    {
        http = HttpController.Instance();
    }

    public IEnumerator GetCountByDay(DateTime date)
    {
        yield return http.GetMethod("statistics/count/total?date=" + date.ToString("yyyy-MM-dd"), (response) =>
        {
            WordsByTimeData temp = JsonUtility.FromJson<WordsByTimeData>(response);
            
            if (temp.max > 0)
            {
                WordsByTime[] timeDatas = temp.raw;
                for (int i = 0; i < timeDatas.Length; i++)
                {
                    if (timeDatas[i].hour >= 9 && timeDatas[i].hour <= 18)
                    {
                        float height;
                        points[timeDatas[i].hour - 9].gameObject.SetActive(true);
                        if(temp.max != temp.min)
                        {
                            height = (high - low) * (timeDatas[i].count - temp.min) / (temp.max - temp.min);
                        }
                        else
                        {
                            height = (high - low) / 2;
                        }
                        if (timeDatas[i].count == 0)
                        {
                            height = -60;
                        }
                        points[timeDatas[i].hour - 9].anchoredPosition = new Vector2(points[timeDatas[i].hour - 9].anchoredPosition.x, low + height);
                    }
                }
                max.text = "최대" + temp.max.ToString();
                min.text = "최소" + temp.min.ToString();
                avg.text = "평균: " + temp.avg.ToString("F1");
                tempTxt.SetActive(false);
            }
            else
            {
                for(int i = 0; i < points.Length; i++)
                {
                    points[i].gameObject.SetActive(false);
                }
                max.text = "-";
                min.text = "-";
                avg.text = "-";
                tempTxt.SetActive(true);
            }
        });
    }
}
