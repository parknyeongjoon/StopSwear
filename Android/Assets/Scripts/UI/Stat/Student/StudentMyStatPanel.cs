using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StudentMyStatPanel : MonoBehaviour
{
    int id = 0;
    #region Utility
    // 0 ������ �ڱ� �ڽ��� ����, id �� ������ �� id ������ ����
    public void SetMyStat(ProgramInfo program, int _id)
    {
        if(_id != 0) { id = _id;}
        StartCoroutine(mostWordByProgram.SetProgramGraph(program, id));
        StartCoroutine(wordsByProgram.GetWordsByDay(program, id));
        DayBtnAnim((int)DateTime.Today.DayOfWeek-1);
    }

    IEnumerator GetDataByDate(DateTime date)
    {
        yield return wordsByTime.GetCountByDay(date, id);
        yield return wordsByDate.SetWordsGraph(date, id);
    }

    public void DayBtnAnim(int day) // ��:0, ȭ:1, ��:2, ��:3, ��:4
    {
        for (int i = 0; i < 5; i++)
        {
            if (i == day)
            {
                dayToggleImg[i].color = new Color(0, 0, 1);
                DateTime today = DateTime.Today;
                DateTime date = today.AddDays(day - (int)today.DayOfWeek + 1);
                StartCoroutine(GetDataByDate(date));
            }
            else
            {
                dayToggleImg[i].color = new Color(41.0f/255, 45.0f/255, 160.0f/255);
            }
        }
    }
    #endregion

    #region UI
    [SerializeField] Image[] dayToggleImg;
    [SerializeField] TimeGrpahScript wordsByTime;
    [SerializeField] WordsScript wordsByDate;
    [SerializeField] WordsScript mostWordByProgram;
    [SerializeField] LineGraphScript wordsByProgram;

    #endregion

}