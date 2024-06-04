using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeacherReviewPanel : MonoBehaviour
{
    HttpController http;

    [SerializeField] GameObject programContent, programCard;
    [SerializeField] GameObject memberContent, studentCard;
    [SerializeField] StudentReviewScroll reviewScroll;
    [SerializeField] GameObject programPanel, memberPanel, reviewScrollPanel;

    ProgramInfo cur_program;
    
    // Start is called before the first frame update
    void Start()
    {
        http = HttpController.Instance();
        StartCoroutine(GetProgramReviewList());
    }

    IEnumerator GetProgramReviewList()
    {
        UIManager.Instance().SetClear(programContent.transform);
        yield return new WaitUntil(() => http != null);
        yield return http.GetMethod("manage/program/get/all", (response) =>
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
        int total_member_count = -1;
        yield return http.GetMethod("statistics/count/word/group?programName=" + program.programName, (response) =>
        {
            List<WordData> wordDatas = JsonConvert.DeserializeObject<List<WordData>>(response);
            if(wordDatas.Count <= 0)
            {
                most_word = "¿å¼³ »ç¿ë x";
            }
            else
            {
                most_word = wordDatas[0].word;
            }
        });
        yield return http.GetMethod("statistics/count/daily/group?programName=" + program.programName, (response) =>
        {
            WordsByProgram data = http.GetJsonData<WordsByProgram>(response);
            total_count = data.sum.ToString();
        });
        yield return http.GetMethod("manage/students/program?programName=" + program.programName, (response) =>
        {
            List<UserInfo> userInfos = JsonConvert.DeserializeObject<List<UserInfo>>(response);
            total_member_count = userInfos.Count;
        });
        GameObject cardObj = Instantiate(programCard, programContent.transform);
        cardObj.GetComponent<ProgramCard>().SetText(program.programName, rank, program.startDate, program.endDate, total_count, most_word, total_member_count.ToString());
        cardObj.GetComponent<Button>().onClick.AddListener(() => OpenMemberPanelBtn(program));
    }

    void OpenMemberPanelBtn(ProgramInfo program)
    {
        cur_program = program;

        programPanel.SetActive(false);
        memberPanel.SetActive(true);
        reviewScrollPanel.SetActive(false);

        StartCoroutine(OpenMemberPanel(program));
    }

    IEnumerator OpenMemberPanel(ProgramInfo program)
    {
        yield return http.GetMethod("manage/students/program?programName=" + program.programName, (response) =>
        {
            UIManager.Instance().SetClear(memberContent.transform);

            List<UserInfo> studentList = JsonConvert.DeserializeObject<List<UserInfo>>(response);

            foreach (UserInfo student in studentList)
            {
                StartCoroutine(SetStudentCard(program, student));
            }
        });
    }

    IEnumerator SetStudentCard(ProgramInfo program, UserInfo student)
    {
        Debug.Log(student.name);
        string rank = "-1", total_count = "temp", most_word = "temp";
        GameObject studentCardObj = Instantiate(studentCard, memberContent.transform);
        yield return http.GetMethod("statistics/rank/" + student.id + "?programName=" + program.programName, (response) =>
        {
            rank = response;
        });
        yield return http.GetMethod("statistics/most-used/program/" + student.id + "?programName=" + program.programName, (response) =>
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
        yield return http.GetMethod("statistics/count/daily/" + student.id + "?programName=" + program.programName, (response) =>
        {
            WordsByProgram data = http.GetJsonData<WordsByProgram>(response);
            total_count = data.sum.ToString() + "È¸";
        });
        studentCardObj.GetComponent<StudentCard>().SetText(student.name, rank, total_count, most_word);
        studentCardObj.GetComponent<Button>().onClick.AddListener(() => OpenReviewScroll(program, student.id));
    }

    void OpenReviewScroll(ProgramInfo info, int id)
    {
        programPanel.SetActive(false);
        memberPanel.SetActive(false);
        reviewScrollPanel.SetActive(true);
        reviewScroll.SetReviewScroll(info, id);
    }

    public void OpenProgramPanel()
    {
        programPanel.SetActive(true);
        memberPanel.SetActive(false);
        reviewScrollPanel.SetActive(false);
        StartCoroutine(GetProgramReviewList());
    }

    public void OpenMemberPanel()
    {
        programPanel.SetActive(false);
        memberPanel.SetActive(true);
        reviewScrollPanel.SetActive(false);
        OpenMemberPanelBtn(cur_program);
    }
}
