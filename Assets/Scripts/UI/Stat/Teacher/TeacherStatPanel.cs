using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TeacherStatPanel : MonoBehaviour
{
    HttpController http;

    [SerializeField] GameObject memberPanel, oneStatPanel, groupStatPanel;
    [SerializeField] GameObject memberContent, studentCard;
    [SerializeField] StudentMyStatPanel oneStat;
    [SerializeField] TeacherGroupStatPanel groupStat;
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
            OpenMemberPanel();
        });
    }

    public void OpenMemberPanel()
    {
        memberPanel.SetActive(true);
        SetMemberList(programInfo.programName);
        oneStatPanel.SetActive(false);
        groupStatPanel.SetActive(false);
    }

    public void SetMemberList(string programName)
    {
        StartCoroutine(GetMemberList(programName));
    }

    IEnumerator GetMemberList(string programName)
    {
        yield return http.GetMethod("manage/students/program?programName=" + programName, (response) =>
        {
            UIManager.Instance().SetClear(memberContent.transform);

            List<UserInfo> studentList = JsonConvert.DeserializeObject<List<UserInfo>>(response);

            foreach (UserInfo student in studentList)
            {
                StartCoroutine(SetStudentCard(student));
            }
        });
    }

    IEnumerator SetStudentCard(UserInfo student)
    {
        string rank = "-1", total_count ="temp", most_word="temp";
        GameObject studentCardObj = Instantiate(studentCard, memberContent.transform);
        yield return http.GetMethod("statistics/rank/" + student.id, (response) =>
        {
            Debug.Log(response);
            rank = response;
        });
        yield return http.GetMethod("statistics/most-used/program/" + student.id, (response) =>
        {
            if (response == null || response == "")
            {
                most_word = "¿å¼³ »ç¿ë x";
            }
            else
            {
                most_word = response;
            }

        });
        studentCardObj.GetComponent<StudentCard>().SetText(student.name, rank, total_count, most_word);
        studentCardObj.GetComponent<Button>().onClick.AddListener(() => OpenOneStat(student.id));
    }

    public void OpenOneStat(int id)
    {
        memberPanel.SetActive(false);
        oneStatPanel.SetActive(true);
        oneStat.SetMyStat(programInfo.programName, id);
        groupStatPanel.SetActive(false);
    }

    public void OpenGroupStat()
    {
        memberPanel.SetActive(false);
        oneStatPanel.SetActive(false);
        groupStatPanel.SetActive(true);
        groupStat.SetGroupStat(programInfo.programName);
    }
}