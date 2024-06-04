using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Dates;
using UnityEngine;
using UnityEngine.UI;

public class ProgramSettingScript : MonoBehaviour
{
    HttpController http;
    [SerializeField] GameObject programPanel;
    [SerializeField] InfoPanel infoPanel;
    [SerializeField] TMP_InputField programNameIF;
    [SerializeField] DatePicker startDate, endDate;
    [SerializeField] Button infoClickBtn;

    void OnEnable()
    {
        http = HttpController.Instance();
        StartCoroutine(CheckProgram());
    }


    IEnumerator CheckProgram()
    {
        yield return http.GetMethod("manage/program/get/current", (response) =>
        {
            ProgramInfo programInfo = http.GetJsonData<ProgramInfo>(response);
            infoPanel.SetInfoPanel(programInfo);
            if (programInfo != null)
            {
                if(UIManager.Instance().role == "STUDENT")
                {
                    AudioRecorderController.Instance().StartRecording();
                }
                programPanel.SetActive(true);
                gameObject.SetActive(false);
                infoClickBtn.interactable = true;
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
            programNameIF.text = "";
        });
    }

    IEnumerator AttendProgram()
    {
        JObject temp = new JObject();
        temp["programName"] = programNameIF.text;
        yield return http.PostMethod("manage/program/join", temp.ToString(), (response) =>
        {
            StartCoroutine(CheckProgram());
            programNameIF.text = "";
        });
    }
}
