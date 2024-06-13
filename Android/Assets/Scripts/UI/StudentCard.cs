using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StudentCard : MonoBehaviour
{
    [SerializeField] TMP_Text student_name, rank, date, word;

    public void SetText(string _student_name, string _rank, string _date, string _word)
    {
        student_name.text = _student_name;
        rank.text = _rank + "등";
        date.text = _date;
        word.text = "최다 사용: " + _word;
    }

    public void SetText(string _student_name)
    {
        student_name.text = _student_name;
    }
}
