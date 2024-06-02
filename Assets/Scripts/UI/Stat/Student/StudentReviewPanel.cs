using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class StudentReviewPanel : MonoBehaviour
{
    HttpController http;

    [SerializeField] GameObject programContent, programCard;
    [SerializeField] StudentReviewScroll reviewScroll;
    [SerializeField] GameObject reviewPanel, reviewScrollPanel;

    private void Start()
    {
        http = HttpController.Instance();
        StartCoroutine(GetProgramReviewList());
    }

    IEnumerator GetProgramReviewList()
    {
        yield return http.GetMethod("manage/program/get/me", (response) =>
        {
            List<ProgramInfo> programList = JsonConvert.DeserializeObject<List<ProgramInfo>>(response);

            foreach (ProgramInfo info in programList)
            {
                StartCoroutine(SetProgramCard(info));
            }
        });
    }

    IEnumerator SetProgramCard(ProgramInfo program)
    {
        string rank = "-1", total_count = "total", most_word = "most";
        //yield return http.GetMethod("rank")
        yield return http.GetMethod("statistics/most-used/program?programName=" + program.programName, (response) =>
        {
            most_word = response;
        });
        //yield return http.GetMethod("total_cout")
        GameObject cardObj = Instantiate(programCard, programContent.transform);
        cardObj.GetComponent<ProgramCard>().SetText(program.programName, rank, program.startDate, program.endDate, total_count, most_word);
        cardObj.GetComponent<Button>().onClick.AddListener(() => OpenReviewScroll(program));
    }

    void OpenReviewScroll(ProgramInfo info)
    {
        reviewPanel.SetActive(false);
        reviewScrollPanel.SetActive(true);
        reviewScroll.SetReviewScroll(info, 0);
    }

    public void OpenReviewPanel()
    {
        reviewScrollPanel.SetActive(false);
        reviewPanel.SetActive(true);
    }
}
