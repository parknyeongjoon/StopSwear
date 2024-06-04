using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeacherGroupStatPanel : MonoBehaviour
{
    HttpController http;

    void Start()
    {
        http = HttpController.Instance();
    }

    #region Utility
    public void SetGroupStat(ProgramInfo program)
    {
        //StartCoroutine(GetMyRank(programName));
        StartCoroutine(GetGroupWords(program));
    }

    IEnumerator GetGroupWords(ProgramInfo program)
    {
        yield return new WaitUntil(() => http != null);
        yield return http.GetMethod("statistics/count/word/group?programName=" + program.programName, (response) =>
        {
            List<WordData> wordDatas = JsonConvert.DeserializeObject<List<WordData>>(response);
            detailGraph.SetDetails(wordDatas);
            circleGraph.SetCircleGraph(wordDatas);
        });
    }
    #endregion

    #region UI
    [SerializeField] DetailsPanelScript detailGraph;
    [SerializeField] CircleGraphScript circleGraph;
    #endregion
}