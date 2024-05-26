using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoPanel : MonoBehaviour
{
    HttpController http;

    [SerializeField] GameObject normalPanel, clickPanel;
    [SerializeField] TMP_Text nameTxt, codeTxt, progresTxt, rankTxt;
    [SerializeField] TMP_Text durationTxt;

    class UserInfo
    {
        public string name;
        public string email;
    }

    private void Start()
    {
        http = HttpController.Instance();
    }

    public void SetInfoPanel(ProgramInfo info)
    {
        progresTxt.text = "";
        rankTxt.text = "";
        durationTxt.text = "";
        StartCoroutine(GetData(info));
        if (info != null)
        {
            //codeTxt.text = info.programName;
            DateTime startDate = DateTime.Parse(info.startDate);
            DateTime endDate = DateTime.Parse(info.endDate);
            progresTxt.text = "ÁøÇà·ü: " + (100 * DateTime.Now.Subtract(startDate) / endDate.Subtract(startDate)).ToString("F2") + "%";
            durationTxt.text = startDate.ToString("yyyy-MM-dd") + " ~ " + endDate.ToString("yyyy-MM-dd");
        }
    }

    IEnumerator GetData(ProgramInfo info)
    {
        yield return new WaitUntil(() => http != null);

        yield return http.GetMethod("manage/info/class", (response) =>
        {
            codeTxt.text = response;
        });
        yield return http.GetMethod("manage/code/class", (response) =>
        {
            codeTxt.text += " - " + response;
        });
        yield return http.GetMethod("manage/info/user", (response) =>
        {
            UserInfo temp = http.GetJsonData<UserInfo>(response);
            nameTxt.text = temp.name;
        });
        if (info != null)
        {
            yield return http.GetMethod("statistics/rank?programName=" + info.programName, (response) =>
            {
                rankTxt.text = response + " µî!!";
            });
        }
    }

    public void OpenClickPanel()
    {
        normalPanel.SetActive(false);
        clickPanel.SetActive(true);
    }

    public void CloseClickPanel()
    {
        normalPanel.SetActive(true);
        clickPanel.SetActive(false);
    }
}
