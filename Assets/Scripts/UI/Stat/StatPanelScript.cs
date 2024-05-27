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

    void Start()
    {
        http = HttpController.Instance();
        StartCoroutine(SetPanel());
    }

    IEnumerator SetPanel()
    {
        yield return http.GetMethod("manage/program/get/current", (response) =>
        {
            programInfo = http.GetJsonData<ProgramInfo>(response);
            infoPanel.SetInfoPanel(programInfo);
            OpenMyStat();
        });
    }

    public void OpenMyStat()
    {
        myStatPanel.SetActive(true);
        myStat.SetMyStat(programInfo.programName);
        groupStatPanel.SetActive(false);
    }

    public void OpenGroupStat()
    {
        myStatPanel.SetActive(false);
        groupStatPanel.SetActive(true);
        groupStat.SetGroupStat(programInfo.programName);
    }
}