using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Dates;
using UnityEngine;

public class ProgramInfo
{
    public string programName;
    public string startDate;
    public string endDate;
}

public class ProgramSettingScript : MonoBehaviour
{
    HttpController http;
    [SerializeField] GameObject programPanel;
    [SerializeField] InfoPanel infoPanel;
    [SerializeField] TMP_InputField programNameIF;
    [SerializeField] DatePicker startDate, endDate;

    private void Start()
    {
        http = HttpController.Instance();
        StartCoroutine(CheckProgram());
    }


    IEnumerator CheckProgram()
    {
        yield return http.GetMethod("manage/program/get/current", (response) =>
        {
            ProgramInfo programInfo = http.GetJsonData<ProgramInfo>(response);
            if(programInfo != null)
            {
                infoPanel.SetInfoPanel(programInfo);
                programPanel.SetActive(true);
                gameObject.SetActive(false);
            }
        });
    }

    public void StartProgramBtn()
    {
        StartCoroutine(StartProgram());
    }

    public void AttendProgramBtn()
    {
        StartCoroutine(AttendProgram());
    }

    IEnumerator StartProgram()
    {
        if(programNameIF.text == "")
        {
            yield break;
        }

        JObject programDataJson = new JObject();
        programDataJson["programName"] = programNameIF.text;
        programDataJson["startDate"] = startDate.SelectedDate.Date.ToString("yyyy-MM-dd");
        programDataJson["endDate"] = endDate.SelectedDate.Date.ToString("yyyy-MM-dd");

        yield return http.PostMethod("manage/program/create", programDataJson.ToString(), (respone) =>
        {
            StartCoroutine(CheckProgram());
        });
    }

    IEnumerator AttendProgram()
    {
        JObject temp = new JObject();
        temp["programName"] = programNameIF.text;
        yield return http.PostMethod("manage/program/join", temp.ToString(), (response) =>
        {
            StartCoroutine(CheckProgram());
        });
    }
}
