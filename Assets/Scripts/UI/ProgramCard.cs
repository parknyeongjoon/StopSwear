using TMPro;
using UnityEngine;

public class ProgramCard : MonoBehaviour
{
    [SerializeField] TMP_Text program_name, rank, duration, total_count, most_word, total_member_count;

    public void SetText(string _program_name, string _rank, string _from, string _to, string _total_count, string _most_word, string _total_member_count)
    {
        program_name.text = _program_name;
        if(_rank != "-1")
        {
            rank.text = _rank + "��";
        }
        duration.text = _from + " ~ " + _to;
        total_count.text = "�� ���: " + _total_count +"ȸ";
        most_word.text = "�ִ� ���: " + _most_word;
        if(_total_member_count != "")
        {
            total_member_count.text = "(" + _total_member_count + "��)";
        }
    }
}