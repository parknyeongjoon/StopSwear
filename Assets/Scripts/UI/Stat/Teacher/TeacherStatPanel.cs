using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] TMP_Text sortBtnTxt;
    [SerializeField] RectTransform memberRect;

    ProgramInfo programInfo;
    bool isSortByRank = false;
    float originHeight;

    void Start()
    {
        http = HttpController.Instance();
        StartCoroutine(SetPanel());
        originHeight = 200;
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
        StartCoroutine(GetMemberList(programName, isSortByRank));
    }

    IEnumerator GetMemberList(string programName, bool isSortByRank)
    {
        string query = "manage/students/program?programName=" + programName;
        if (isSortByRank)
        {
            query = query + "&sorted=" + isSortByRank.ToString();
        }
        yield return http.GetMethod(query, (response) =>
        {
            UIManager.Instance().SetClear(memberContent.transform);

            List<UserInfo> studentList = JsonConvert.DeserializeObject<List<UserInfo>>(response);
            int count = studentList.Count;
            memberRect.sizeDelta = new Vector2(memberRect.sizeDelta.x, originHeight + 250 * count);
            foreach (UserInfo student in studentList)
            {
                StartCoroutine(SetStudentCard(programName, student));
            }
        });
    }

    IEnumerator SetStudentCard(string programName, UserInfo student)
    {
        string rank = "-1", total_count ="temp", most_word="temp";
        GameObject studentCardObj = Instantiate(studentCard, memberContent.transform);
        yield return http.GetMethod("statistics/rank/" + student.id + "?programName=" + programName, (response) =>
        {
            rank = response;
        });
        yield return http.GetMethod("statistics/most-used/program/" + student.id + "?programName=" + programName, (response) =>
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
        yield return http.GetMethod("statistics/count/daily/" + student.id + "?programName=" + programName, (response) =>
        {
            WordsByProgram data = http.GetJsonData<WordsByProgram>(response);
            total_count = data.sum.ToString();
        });
        studentCardObj.GetComponent<StudentCard>().SetText(student.name, rank, total_count, most_word);
        studentCardObj.GetComponent<Button>().onClick.AddListener(() => OpenOneStat(student.id));
    }

    public void ToggleSortBtn()
    {
        isSortByRank = !isSortByRank;
        if (isSortByRank) { sortBtnTxt.text = "·©Å© ¼ø"; }
        else { sortBtnTxt.text = "ÀÌ¸§ ¼ø"; }
        SetMemberList(programInfo.programName);
    }

    public void OpenOneStat(int id)
    {
        memberPanel.SetActive(false);
        oneStatPanel.SetActive(true);
        oneStat.SetMyStat(programInfo, id);
        groupStatPanel.SetActive(false);
    }

    public void OpenGroupStat()
    {
        memberPanel.SetActive(false);
        oneStatPanel.SetActive(false);
        groupStatPanel.SetActive(true);
        groupStat.SetGroupStat(programInfo);
    }
}