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

    ProgramInfo programInfo;

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
        StartCoroutine(GetData());
        codeTxt.text = info.programName;
        DateTime startDate = DateTime.Parse(info.startDate);
        DateTime endDate = DateTime.Parse(info.endDate);
        progresTxt.text = "ÁøÇà·ü: " + (100 * DateTime.Now.Subtract(startDate) / endDate.Subtract(startDate)).ToString("F2") + "%";
        durationTxt.text = startDate.ToString("yyyy-MM-dd") + " ~ " + endDate.ToString("yyyy-MM-dd");
    }

    IEnumerator GetData()
    {
        yield return http.GetMethod("manage/program/get/current", (response) =>
        {
            programInfo = http.GetJsonData<ProgramInfo>(response);
        });
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
        yield return http.GetMethod("statistics/rank?programName=" + programInfo.programName, (response) =>
        {
            rankTxt.text = response + " µî!!";
        });
    }

    public void OpenClickPanel()
    {
        normalPanel.SetActive(false);
        clickPanel.SetActive(true);
    }

    public void CloseClickPanel()
    {
        normalPanel?.SetActive(true);
        clickPanel?.SetActive(false);
    }
}
