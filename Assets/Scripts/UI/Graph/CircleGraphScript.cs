using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CircleGraphScript : MonoBehaviour
{
    [SerializeField] GameObject circleObject;

    public void SetCircleGraph(List<WordData> wordDatas)
    {
        SetClear();

        int size = wordDatas.Count;
        int count = 0;
        float rotate = 0;
        foreach (WordData wordData in wordDatas)
        {
            count += wordData.count;
        }
        if(size == 0)
        {
            GameObject temp = Instantiate(circleObject, transform);
            temp.GetComponentInChildren<TMP_Text>().text = "욕설 사용 안 함";
            Image circleImg = temp.GetComponent<Image>();
            circleImg.color = new Color(1, 1, 1);
        }
        else if(size <= 5)
        {
            for(int i = 0; i < size; i++)
            {
                GameObject temp = Instantiate(circleObject, transform);
                TMP_Text circleTxt = temp.GetComponentInChildren<TMP_Text>();
                circleTxt.text = wordDatas[i].word + " (" + ((float)wordDatas[i].count / count * 100).ToString("F0") + "%)";
                circleTxt.rectTransform.rotation = Quaternion.Euler(0, 0, -(360 - rotate));
                Image circleImg = temp.GetComponent<Image>();
                circleImg.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                circleImg.fillAmount = (float)wordDatas[i].count / count;
                temp.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -rotate);
                rotate += (360.0f / count) * wordDatas[i].count;
            }
        }
        else
        {
            int word_count = 0;
            for(int i = 0; i < 4; i++)
            {
                GameObject temp = Instantiate(circleObject, transform);
                TMP_Text circleTxt = temp.GetComponentInChildren<TMP_Text>();
                circleTxt.text = wordDatas[i].word + " (" + ((float)wordDatas[i].count / count * 100).ToString("F0") + "%)";
                circleTxt.rectTransform.rotation = Quaternion.Euler(0, 0, -(360 - rotate));
                Image circleImg = temp.GetComponent<Image>();
                circleImg.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                circleImg.fillAmount = (float)wordDatas[i].count / count;
                word_count += wordDatas[i].count;
                temp.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -rotate);
                rotate += 360.0f / count * wordDatas[i].count;
            }

            GameObject last = Instantiate(circleObject, transform);
            TMP_Text lastTxt = last.GetComponentInChildren<TMP_Text>();
            lastTxt.text = "기타: (" + ((float)(count - word_count) / count * 100).ToString("F0") + "%)";
            lastTxt.rectTransform.rotation = Quaternion.Euler(0, 0, -(360 - rotate));
            Image lastImg = last.GetComponent<Image>();
            lastImg.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            lastImg.fillAmount = (float)(count - word_count) / count;
            last.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -rotate);
        }
    }

    public void SetClear()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
