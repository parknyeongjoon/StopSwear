using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Schema;
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
    public int sum;
}

public class LineGraphScript : MonoBehaviour
{
    HttpController http;

    void Start()
    {
        http = HttpController.Instance();
    }

    public IEnumerator GetWordsByDay(ProgramInfo program, int id)
    {
        yield return new WaitUntil(() => http != null);
        string query = "statistics/count/daily";
        if (id != 0)
        {
            query += "/" + id.ToString();
        }
        query += "?programName=" + program.programName;

        yield return http.GetMethod(query, (response) =>
        {
            WordsByProgram wordsByDay = http.GetJsonData<WordsByProgram>(response);

            min.text = wordsByDay.min.ToString();
            max.text = wordsByDay.max.ToString();
            avg.text = "ЦђБе: " + wordsByDay.avg.ToString("F2");

            SetGraph(wordsByDay);
        });
    }

    public IEnumerator GetRanksByDate(ProgramInfo program)
    {
        low = min.rectTransform.position.y;
        high = max.rectTransform.position.y;
        UIManager.Instance().SetClear(XContent.transform);
        min.gameObject.SetActive(true);

        yield return new WaitUntil(() => http != null);
        
        DateTime startDate = DateTime.ParseExact(program.startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        DateTime date = startDate;
        string query = "statistics/rank?programName=" + program.programName;

        List<int> intList = new List<int>();
        int maxRank = 0, minRank = int.MaxValue, count = 0;
        float total = 0.0f;
        while (date <= DateTime.Today)
        {
            query = query + "&date=" + date.ToString("yyyy-MM-dd");
            yield return http.GetMethod(query, (response) =>
            {
                int rank = int.Parse(response);
                intList.Add(rank);
                if(rank > maxRank) { maxRank = rank; }
                if(rank < minRank) { minRank = rank; }
                total += rank;
                count++;
            });
            date = date.AddDays(1);
        }

        min.text = minRank.ToString();
        max.text = maxRank.ToString();
        avg.text = "ЦђБе: " + (total/count).ToString("F2");

        for (int i = 0; i < intList.Count; i++)
        {
            GameObject elem = Instantiate(XElem, XContent.transform);
            TMP_Text tempTxt = elem.GetComponent<TMP_Text>();
            tempTxt.text = startDate.AddDays(i).ToString("MM-dd");

            GameObject temp = Instantiate(point, tempTxt.transform);
            RectTransform tempRect = temp.GetComponent<RectTransform>();

            float height;
            if (maxRank != minRank)
            {
                height = (high - low) * (intList[i] - minRank) / (maxRank - minRank);
            }
            else
            {
                height = high - low;
                min.gameObject.SetActive(false);
            }
            tempRect.anchoredPosition = new Vector2(0, 120 + height);
        }
    }

    void SetGraph(WordsByProgram datas)
    {
        ClearGraph();
        min.gameObject.SetActive(true);

        low = min.rectTransform.position.y;
        high = max.rectTransform.position.y;

        foreach (var data in datas.raw)
        {
            if(DateTime.Parse(data.date) > DateTime.Today) { continue; }

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
                height = high - low;
                min.gameObject.SetActive(false);
            }
            if (data.count == 0)
            {
                height = -60;
            }
            tempRect.anchoredPosition = new Vector2(0, 120 + height);
        }

        min.text = datas.min.ToString();
        max.text = datas.max.ToString();
        avg.text = "ЦђБе: " +  datas.avg.ToString("F2");
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
