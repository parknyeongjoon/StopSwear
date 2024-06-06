using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PreProgramCard : MonoBehaviour
{
    [SerializeField] TMP_Text programName, duration;

    public void SetPreProgramCard(ProgramInfo program)
    {
        programName.text = program.programName;
        duration.text = program.startDate + " ~ " + program.endDate;
    }
}
