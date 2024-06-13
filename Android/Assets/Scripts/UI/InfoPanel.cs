using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    HttpController http;
    UIManager uiManager;

    [SerializeField] GameObject normalPanel, clickPanel;
    [SerializeField] TMP_Text nameTxt, codeTxt, progresTxt, rankTxt;
    [SerializeField] TMP_Text durationTxt;
    [SerializeField] Button logoutBtn;

    string classCode;

    private void Start()
    {
        http = HttpController.Instance();
        uiManager = UIManager.Instance();

        logoutBtn.onClick.AddListener(uiManager.LogOut);
    }

    public void SetInfoPanel(ProgramInfo info)
    {
        progresTxt.text = "";
        rankTxt.text = "";
        durationTxt.text = "";
        codeTxt.text = "";
        StartCoroutine(GetData(info));
        if (info != null)
        {
            DateTime startDate = DateTime.Parse(info.startDate);
            DateTime endDate = DateTime.Parse(info.endDate);
            progresTxt.text = "진행률: " + (100 * DateTime.Now.Subtract(startDate) / endDate.AddDays(1.0f).Subtract(startDate)).ToString("F2") + "%";
            durationTxt.text = startDate.ToString("yyyy-MM-dd") + " ~ " + endDate.ToString("yyyy-MM-dd");
        }
    }

    IEnumerator GetData(ProgramInfo info)
    {
        yield return new WaitUntil(() => http != null);

        yield return http.GetMethod("manage/info/class", (response) =>
        {
            codeTxt.text = response;
        });
        if (uiManager.role == "TEACHER" || uiManager.role == "MANAGER")
        {
            yield return http.GetMethod("manage/code/class", (response) =>
            {
                if (response == null || response == "") { return; }
                classCode = response;
            });
        }
        yield return http.GetMethod("manage/info/user", (response) =>
        {
            UserInfo temp = http.GetJsonData<UserInfo>(response);
            nameTxt.text = temp.name;
            if(uiManager.role == "TEACHER")
            {
                nameTxt.text += " 선생님";
            }
        });
        if (info != null)
        {
            if(uiManager.role == "STUDENT")
            {
                yield return http.GetMethod("statistics/rank?programName=" + info.programName, (response) =>
                {
                    rankTxt.text = response + " 등!!";
                });
            }
            else if (uiManager.role == "TEACHER" || uiManager.role == "MANAGER")
            {
                yield return http.GetMethod("manage/students/program?programName=" + info.programName + "&sorted=true", (response) => {
                    List<UserInfo> students = JsonConvert.DeserializeObject<List<UserInfo>>(response);
                    rankTxt.text = "주의 인물: " + students[0].name;
                });
            }  
        }
    }

    public void OpenClickPanel()
    {
        normalPanel.SetActive(false);
        clickPanel.SetActive(true);
    }

    public void CloseClickPanel()
    {
        normalPanel.SetActive(true);
        clickPanel.SetActive(false);
    }

    public void CopyClassCodeClipBoard()
    {
        ClipBoarController.Instance().CopyToClipboard(classCode);
    }
}
