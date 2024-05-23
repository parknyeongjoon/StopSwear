using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Dates;
using UnityEngine;

public class ProgramSettingScript : MonoBehaviour
{
    HttpController http;
    [SerializeField] GameObject programPanel;
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
            if(response != null)
            {
                programPanel.SetActive(true);
                gameObject.SetActive(false);
            }
        });
    }

    public void StartProgramBtn()
    {
        StartCoroutine(StartProgram());
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
            Debug.Log("post ½ÇÇà");
            programPanel.SetActive(true);
            gameObject.SetActive(false);
        });
    }
}
