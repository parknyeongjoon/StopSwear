using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoPanel : MonoBehaviour
{
    HttpController http;

    [SerializeField] GameObject normalPanel, clickPanel;
    [SerializeField] TMP_Text nameTxt, codeTxt, progresTxt, rankTxt;

    class UserInfo
    {
        public string name;
        public string email;
    }

    private void Awake()
    {
        http = HttpController.Instance();
    }

    private void OnEnable()
    {
        StartCoroutine(GetData());
    }

    IEnumerator GetData()
    {
        yield return http.GetMethod("manage/info/class", (response) =>
        {
            codeTxt.text = response + " - ";
        });
        yield return http.GetMethod("manage/code/class", (response) =>
        {
            codeTxt.text += response;
        });
        yield return http.GetMethod("manage/info/user", (response) =>
        {
            UserInfo temp = http.GetJsonData<UserInfo>(response);
            nameTxt.text = temp.name;
        });
        yield return http.GetMethod("statistics/rank", (response) =>
        {
            rankTxt.text = response + " µî!!";
        });
    }

    public void OpenClickPanel()
    {
        normalPanel.SetActive(false);
        clickPanel.SetActive(true);
    }

    public void CloseClickPanel()
    {
        normalPanel?.SetActive(true);
        clickPanel?.SetActive(false);
    }
}
