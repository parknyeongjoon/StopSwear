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
            rank.text = _rank + "등";
        }
        duration.text = _from + " ~ " + _to;
        total_count.text = "총 사용: " + _total_count +"회";
        most_word.text = "최다 사용: " + _most_word;
        if(_total_member_count != "")
        {
            total_member_count.text = "(" + _total_member_count + "명)";
        }
    }
}