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

    public IEnumerator GetCountByDay(DateTime date, int id)
    {
        string query = "statistics/count/total";
        if(id != 0)
        {
            query += "/" + id.ToString();
        }
        query += "?date=" + date.ToString("yyyy-MM-dd");
        yield return http.GetMethod(query, (response) =>
        {
            min.gameObject.SetActive(true);

            WordsByTimeData temp = JsonUtility.FromJson<WordsByTimeData>(response);
            
            if (response == null || temp.max <= 0)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    points[i].gameObject.SetActive(false);
                }
                max.text = "-";
                min.text = "-";
                avg.text = "-";
                tempTxt.SetActive(true);
            }
            else
            {
                WordsByTime[] timeDatas = temp.raw;
                for (int i = 0; i < timeDatas.Length; i++)
                {
                    if (timeDatas[i].hour >= 9 && timeDatas[i].hour <= 18)
                    {
                        float height;
                        points[timeDatas[i].hour - 9].gameObject.SetActive(true);
                        if (temp.max != temp.min)
                        {
                            height = (high - low) * (timeDatas[i].count - temp.min) / (temp.max - temp.min);
                        }
                        else
                        {
                            height = high - low;
                            min.gameObject.SetActive(false);
                        }
                        if (timeDatas[i].count == 0)
                        {
                            height = -60;
                        }
                        points[timeDatas[i].hour - 9].anchoredPosition = new Vector2(points[timeDatas[i].hour - 9].anchoredPosition.x, low + height);
                    }
                }
                max.text = "�ִ�" + temp.max.ToString();
                min.text = "�ּ�" + temp.min.ToString();
                avg.text = "���: " + temp.avg.ToString("F1");
                tempTxt.SetActive(false);
            }
        });
    }
}
