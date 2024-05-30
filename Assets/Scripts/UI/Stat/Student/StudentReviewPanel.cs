using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class StudentReviewPanel : MonoBehaviour
{
    HttpController http;

    [SerializeField] GameObject programContent, programCard;

    class ProgramList
    {
        public List<ProgramInfo> list = new List<ProgramInfo>();
    }

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
        //yield return http.GetMethod("")
        yield return http.GetMethod("statistics/most-used/program?" + program.programName, (response) =>
        {
            most_word = response;
        });
        //yield return http.GetMethod("total_cout")
        Instantiate(programCard, programContent.transform).GetComponent<ProgramCard>().SetText(program.programName, rank, program.startDate, program.endDate, total_count, most_word);
    }
}
