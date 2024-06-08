using Newtonsoft.Json;
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
    [SerializeField] GameObject programPanel, preProgramPanel, makeProgramPanel, preProgramContent, preProgramCard, preProgramMemberContent, preProgramStudentCard;
    [SerializeField] InfoPanel infoPanel;
    [SerializeField] TMP_InputField programNameIF;
    [SerializeField] DatePicker startDate, endDate;
    [SerializeField] Button infoClickBtn;
    [SerializeField] TMP_Text preProgramName, preProgramDuration;


    ProgramInfo preProgram;

    void Start()
    {
        http = HttpController.Instance();
        StartCoroutine(CheckProgram());
    }


    IEnumerator CheckProgram()
    {
        yield return http.GetMethod("manage/program/get/current", (response) => // 진행 중인 프로그램이 있다면 program 창으로
        {
            ProgramInfo programInfo = http.GetJsonData<ProgramInfo>(response);
            infoPanel.SetInfoPanel(programInfo);
            if (programInfo != null)
            {
                if (UIManager.Instance().role == "STUDENT")
                {
                    AudioRecorderController.Instance().StartRecording();
                }
                programPanel.SetActive(true);
                gameObject.SetActive(false);
                infoClickBtn.interactable = true;
            }
            else // 현재 예약해둔 프로그램이 있는 지 체크
            {
                StartCoroutine(CheckPreProgram());
            }
        });
    }

    IEnumerator CheckPreProgram()
    {
        if(UIManager.Instance().role == "STUDENT")
        {
            yield return http.GetMethod("manage/program/get/me", (response) =>
            {
                List<ProgramInfo> programList = JsonConvert.DeserializeObject<List<ProgramInfo>>(response);

                if (response == null || programList.Count <= 0)
                {
                    preProgramPanel.SetActive(false);
                    StartCoroutine(GetPreProgramList());
                }
                else
                {
                    if (DateTime.Parse(programList[programList.Count - 1].startDate) > DateTime.Today)
                    {
                        preProgram = programList[programList.Count - 1];
                        preProgramPanel.SetActive(true);
                        SetPreProgramPanel(programList[programList.Count - 1]);
                    }
                }
            });
        }
        else
        {
            yield return http.GetMethod("manage/program/get/all", (response) =>
            {
                makeProgramPanel.SetActive(false);
                preProgramPanel.SetActive(false);
                List<ProgramInfo> programList = JsonConvert.DeserializeObject<List<ProgramInfo>>(response);
                if(response == null || programList.Count <= 0)
                {
                    makeProgramPanel.SetActive(true);
                }
                else if (DateTime.Parse(programList[programList.Count - 1].startDate) > DateTime.Today)
                {
                    preProgram = programList[programList.Count - 1];
                    preProgramPanel.SetActive(true);
                    SetPreProgramPanel(programList[programList.Count - 1]);
                    StartCoroutine(GetPreProgramMemeberList(programList[programList.Count - 1]));
                }
                else
                {
                    makeProgramPanel.SetActive(true);
                }
            });
        }
    }

    void SetPreProgramPanel(ProgramInfo program)
    {
        preProgramName.text = program.programName;
        preProgramDuration.text = program.startDate + " ~ " + program.endDate;
    }

    IEnumerator GetPreProgramMemeberList(ProgramInfo program)
    {
        yield return null;
        UIManager.Instance().SetClear(preProgramMemberContent.transform);
        yield return http.GetMethod("manage/students/program?programName=" + program.programName, (response) =>
        {
            List<UserInfo> students = JsonConvert.DeserializeObject<List<UserInfo>>(response);
            foreach(UserInfo student in students)
            {
                GameObject studentCard = Instantiate(preProgramStudentCard, preProgramMemberContent.transform);
                studentCard.GetComponent<StudentCard>().SetText(student.name);
            }
        });
    }

    public void CancleProgramBtn()
    {
        StartCoroutine(CancleProgram(preProgram));
    }

    IEnumerator CancleProgram(ProgramInfo program)
    {
        yield return http.DeleteMethod("manage/program/join?programName=" + program.programName, (response) =>
        {
            preProgramPanel.SetActive(false);
            preProgram = null;
            StartCoroutine(CheckProgram());
        });
    }

    IEnumerator GetPreProgramList()
    {
        yield return http.GetMethod("manage/program/get/after?date=" + DateTime.Today.AddDays(1.0f).ToString("yyyy-MM-dd"), (response) =>
        {
            UIManager.Instance().SetClear(preProgramContent.transform);
            List<ProgramInfo> programList = JsonConvert.DeserializeObject<List<ProgramInfo>>(response);
            for (int i = 0; i < programList.Count; i++)
            {
                GameObject gameObject = Instantiate(preProgramCard, preProgramContent.transform);
                gameObject.GetComponent<PreProgramCard>().SetPreProgramCard(programList[i]);
                int temp = i;
                gameObject.GetComponent<Button>().onClick.AddListener(() => AttendProgramBtn(programList[temp]));
            }
        });
    }

    public void StartProgramBtn()
    {
        StartCoroutine(StartProgram());
    }

    public void AttendProgramBtn(ProgramInfo program)
    {
        StartCoroutine(AttendProgram(program));
    }

    IEnumerator StartProgram()
    {
        if(programNameIF.text == "")
        {
            yield break;
        }

        DateTime tempStartDate = startDate.SelectedDate.Date;
        DateTime tempEndDate = endDate.SelectedDate.Date;

        if(tempStartDate <= DateTime.Today) { tempStartDate = DateTime.Today.AddDays(1.0f); }
        if(tempEndDate < tempStartDate) { tempEndDate = tempStartDate; }

        JObject programDataJson = new JObject();
        programDataJson["programName"] = programNameIF.text;
        programDataJson["startDate"] = tempStartDate.ToString("yyyy-MM-dd");
        programDataJson["endDate"] = tempEndDate.ToString("yyyy-MM-dd");

        bool durationCheck = true;
        yield return http.GetMethod("manage/program/check-date?startDate=" + tempStartDate.ToString("yyyy-MM-dd") + "&endDate=" + tempEndDate.ToString("yyyy-MM-dd"), (response) =>
        {
            if(response == "false")
            {
                durationCheck = false;
            }
        });
        if (durationCheck == false)
        {
            yield break;
        }

        yield return http.PostMethod("manage/program/create", programDataJson.ToString(), (respone) =>
        {
            StartCoroutine(CheckProgram());
            programNameIF.text = "";
        });
    }

    IEnumerator AttendProgram(ProgramInfo program)
    {
        JObject temp = new JObject
        {
            ["programName"] = program.programName
        };
        yield return http.PostMethod("manage/program/join", temp.ToString(), (response) =>
        {
            UIManager.Instance().SetClear(preProgramContent.transform);
            StartCoroutine(CheckProgram());
        });
    }
}
