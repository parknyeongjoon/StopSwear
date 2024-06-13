using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgramManager : MonoBehaviour
{
    HttpController http;

    ProgramInfo programInfo;

    private void Start()
    {
        http = HttpController.Instance();
        StartCoroutine(GetProgramInfo());
    }

    #region Utility
    IEnumerator GetProgramInfo()
    {
        yield return http.GetMethod("manage/program/get/current", (response) =>
        {
            programInfo = http.GetJsonData<ProgramInfo>(response);
            //set info panel
            infoPanel.SetInfoPanel(programInfo);
            if (response != null) {// attend some program
                //SetStatPanel();
                waitingProgressPanel.SetActive(false);
                runningProgressPanel.SetActive(true);
            }
            else {// not attend program
                infoPanel.SetInfoPanel(null);
            }
        });
    }

    //public void SetStatPanel()
    //{   //set stats
    //    infoPanel.SetInfoPanel(programInfo);
    //    StartCoroutine(SetMyStat(programInfo.programName));
    //    StartCoroutine(SetGroupStat(programInfo.programName));
    //}


    //IEnumerator SetGroupStat(string programName)
    //{
    //    yield return studentGroupStat.SetPanel(programName);
    //}
    #endregion

    #region UI
    [SerializeField] GameObject waitingProgressPanel, runningProgressPanel;
    [SerializeField] InfoPanel infoPanel;
    #endregion
}