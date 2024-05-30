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
        rank.text = _rank;
        date.text = _date;
        word.text = _word;
    }

    public void OpenStudentStatPanel()
    {

    }
}
