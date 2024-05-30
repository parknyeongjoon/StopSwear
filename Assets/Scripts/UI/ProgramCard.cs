using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgramCard : MonoBehaviour
{
    [SerializeField] TMP_Text program_name, rank, duration, total_count, most_word;

    public void SetText(string _program_name, string _rank, string _from, string _to, string _total_count, string _most_word)
    {
        program_name.text = _program_name;
        rank.text = _rank + "��";
        duration.text = _from + " ~ " + _to;
        total_count.text = "�� ���: " + _total_count +"ȸ";
        most_word.text = "�ִ� ���: " + _most_word;
    }
}