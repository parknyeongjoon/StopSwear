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

        for (int i = 0; i < wordDatas.Count; i++)
        {
            WordData wordData = wordDatas[i];

            GameObject temp = Instantiate(wordObject, content.transform);

            TMP_Text[] texts = temp.GetComponentsInChildren<TMP_Text>();
            Image image = temp.GetComponentInChildren<Image>();

            texts[0].text = (i + 1) + ". ";
            texts[1].text = wordData.word + "(" + wordData.count + ")";
            image.fillAmount = (float)wordData.count / 10;
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
