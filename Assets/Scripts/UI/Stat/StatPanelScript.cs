using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StatPanelScript : MonoBehaviour
{
    HttpController http;

    [SerializeField] GameObject myStatPanel, groupStatPanel;
    [SerializeField] StudentMyStatPanel myStat;
    [SerializeField] StudentGroupStatPanel groupStat;
    [SerializeField] InfoPanel infoPanel;

    ProgramInfo programInfo;

    void OnEnable()
    {
        http = HttpController.Instance();
        StartCoroutine(SetPanel());
    }

    void Start()
    {
        StartCoroutine(SetPanel());
    }

    IEnumerator SetPanel()
    {
        yield return http.GetMethod("manage/program/get/current", (response) =>
        {
            programInfo = http.GetJsonData<ProgramInfo>(response);
            infoPanel.SetInfoPanel(programInfo);
            SetStat(programInfo.programName);
        });
    }

    public void SetStat(string programName)
    {
        myStat.SetMyStat(programInfo.programName);
        groupStat.SetGroupStat(programInfo.programName);
    }

    public void OpenMyStat()
    {
        myStatPanel.SetActive(true);
        groupStatPanel.SetActive(false);
    }

    public void OpenGroupStat()
    {
        myStatPanel.SetActive(false);
        groupStatPanel.SetActive(true);
    }
}