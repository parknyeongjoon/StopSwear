using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public static UIManager Instance()
    {
        if (instance == null)
        {
            return null;
        }
        return instance;
    }

    [SerializeField] GameObject StudentInfoPanel, TeacherInfoPanel;
    [SerializeField] GameObject StudentSettingPanel, StudentStatPanel;
    [SerializeField] GameObject TeacherSettingPanel, TeacherStatPanel;
    [SerializeField] GameObject LoginTab;
    [SerializeField] GameObject settingPanel;
    /*[SerializeField] TMP_InputField addSwearInput;
    //[SerializeField] TMP_Text additionalSwearText;
    //[SerializeField] GameObject progressPanel;
    //[SerializeField] Image FrontCircle;
    //[SerializeField] TMP_Text percentText;
    //[SerializeField] GameObject analyzePanel;
    //[SerializeField] TMP_Text analyzeText;*/

    public void SetInfoPanel(string role)
    {
        if (role == "STUDENT")
        {
            TeacherInfoPanel.SetActive(false);
            StudentInfoPanel.SetActive(true);
        }
        else if (role == "TEACHER" || role == "MANAGER")
        {
            StudentInfoPanel.SetActive(false);
            TeacherInfoPanel.SetActive(true);
        }
        else
        {
            StudentInfoPanel.SetActive(false);
            TeacherInfoPanel.SetActive(false);
        }
    }

    public void OpenLoginTab()
    {
        StudentInfoPanel.SetActive(false);
        StudentSettingPanel.SetActive(true);
        StudentStatPanel.SetActive(false);
        TeacherInfoPanel.SetActive(false);
        TeacherSettingPanel.SetActive(true);
        TeacherStatPanel.SetActive(false);
        LoginTab.SetActive(true);
    }
    
    public void CloseLoginTab()
    {
        LoginTab.SetActive(false);
    }

    /*

    public void PanelControll()
    {
        int curState = PlayerPrefs.GetInt("isRecord");
        if(curState == 0)
        {
            OpenSettingPanel();
        }
        else if(curState == 1)
        {
            OpenProgressPanel();
        }
    }

    public void Update()
    {
        if(progressPanel.activeSelf == true)
        {
            SetPercentage();
        }
    }

    public void OpenSettingPanel()
    {
        CloseAllPanel();
        settingPanel.SetActive(true);
        SetAdditionalSwear();
    }

    void SetAdditionalSwear()
    {
        string temp = PlayerPrefs.GetString("addSwear");
        if (temp == null)
        {
            additionalSwearText.text = "X";
        }
        else
        {
            additionalSwearText.text = PlayerPrefs.GetString("addSwear");
        }
    }

    public void AddAdditionalSwear()
    {
        string temp = PlayerPrefs.GetString("addSwear");
        if (addSwearInput.text != "")
        {
            temp += "\n" + addSwearInput.text;
            PlayerPrefs.SetString("addSwear", temp);
            additionalSwearText.text = temp;
        }
    }

    public void OpenProgressPanel()
    {
        CloseAllPanel();
        progressPanel.SetActive(true);
    }

    void SetPercentage()
    {
        // double percent = dateController.CalculateProgressPercent();
        // percentText.text = dateController.startTime.ToString("yyyy-MM-dd") + "~" + dateController.endTime.ToString("yyyy-MM-dd") + "\n" + percent.ToString("F2") + "%";
        // FrontCircle.fillAmount = (float)percent / 100;
    }

    public void OpenAnalizePanel()
    {
        CloseAllPanel();
        analyzePanel.SetActive(true);

    }

    public void SetAnalizeText(List<Statistics> analizeData)
    {
        analyzeText.text = "»ç¿ë È½¼ö\n";
        for (int i = 0; i < analizeData.Count; i++)
        {
            string temp = analizeData[i].word + " " + analizeData[i].frequency + " È¸\n";
            analyzeText.text += temp;
        }
    }

    public void CloseAllPanel()
    {
        settingPanel.SetActive(false);
        progressPanel.SetActive(false);
        analyzePanel.SetActive(false);
    }
    */
}