using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetailsPanelScript : MonoBehaviour
{
    [SerializeField] GameObject content, wordObject;


    public void SetDetails(List<WordData> wordDatas)
    {
        SetClear();

        int max = 0;
        foreach(var wordData in wordDatas)
        {
            if(max < wordData.count) { max = wordData.count; }
        }
        int size = 8;
        if(size > wordDatas.Count)
        {
            size = wordDatas.Count;
        }

        for (int i = 0; i < size; i++)
        {
            WordData wordData = wordDatas[i];

            GameObject temp = Instantiate(wordObject, content.transform);

            TMP_Text[] texts = temp.GetComponentsInChildren<TMP_Text>();
            Image image = temp.GetComponentInChildren<Image>();

            texts[0].text = (i + 1) + ". ";
            texts[1].text = wordData.word + "(" + wordData.count + ")";
            image.fillAmount = (float)wordData.count / max;
            image.color = new Color(Random.Range(0.0f,1.0f), Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));
        }
    }

    public void SetClear()
    {
        for(int i=0;i<content.transform.childCount;i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
    }
}
