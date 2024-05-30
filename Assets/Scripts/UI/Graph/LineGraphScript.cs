using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class WordByDay
{
    public string date;
    public int count;
}

public class WordsByProgram
{
    public WordByDay[] raw;
    public int min;
    public int max;
    public double avg;
}

public class LineGraphScript : MonoBehaviour
{
    HttpController http;

    void Start()
    {
        http = HttpController.Instance();
    }

    public IEnumerator GetWordsByDay(string programName, int id)
    {
        string query = "statistics/count/daily";
        if (id != 0)
        {
            query += "/" + id.ToString();
        }
        query += "?programName=" + programName;
        yield return http.GetMethod(query, (response) =>
        {
            WordsByProgram wordsByDay = http.GetJsonData<WordsByProgram>(response);

            min.text = wordsByDay.min.ToString();
            max.text = wordsByDay.max.ToString();
            avg.text = "ЦђБе: " + wordsByDay.avg.ToString("F2");

            SetGraph(wordsByDay);
        });
    }

    void SetGraph(WordsByProgram datas)
    {
        ClearGraph();

        low = min.rectTransform.position.y;
        high = max.rectTransform.position.y;

        foreach (var data in datas.raw)
        {
            TMP_Text tempTxt = Instantiate(XElem, XContent.transform).GetComponent<TMP_Text>();
            tempTxt.text = DateTime.Parse(data.date).ToString("MM-dd");

            GameObject temp = Instantiate(point, tempTxt.transform);
            RectTransform tempRect = temp.GetComponent<RectTransform>();
            
            float height;
            if (datas.max != datas.min)
            {
                height = (high - low) * (data.count - datas.min) / (datas.max - datas.min);
            }
            else
            {
                height = (high - low) / 2;
            }
            if (data.count == 0)
            {
                height = -60;
            }
            tempRect.anchoredPosition = new Vector2(tempRect.position.x, 120 + height);
        }
    }

    void ClearGraph()
    {
        for (int i = 0; i < XContent.transform.childCount; i++)
        {
            Destroy(XContent.transform.GetChild(i).gameObject);
        }
    }

    #region UI
    [SerializeField] GameObject XElem, XContent, point;
    [SerializeField] TMP_Text min, max, avg;
    float low, high;
    #endregion
}
