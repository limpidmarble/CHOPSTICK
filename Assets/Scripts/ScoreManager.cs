using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI firstScore;
    public TextMeshProUGUI secondScore;
    public TextMeshProUGUI thirdScore;
    public TextMeshProUGUI newRank;
    public TextMeshProUGUI newScore;


    void Start()
    {
        int i = 1;
        string prefKey;
        int lastScore = 0;
        int lastRank = 1;
        List<int> scores = new List<int>();

        while (i <= 101)
        {
            prefKey = "score" + $"{i}";
            if (PlayerPrefs.HasKey(prefKey))
            {
                scores.Add(PlayerPrefs.GetInt(prefKey));
                lastScore = PlayerPrefs.GetInt(prefKey);
                i++;
            }
            else
            {
                break;
            }
        }

        var sortedScores = scores.OrderByDescending(x => x).ToList();

        for (int j = 0; j < sortedScores.Count; j++)
        {
            if (sortedScores[j] == lastScore)
            {
                lastRank = i + 1;
                break;
            }
        }

        firstScore.text = $"{sortedScores[0]}";
        secondScore.text = $"{sortedScores[1]}";
        thirdScore.text = $"{sortedScores[2]}";
        newRank.text = $"{lastRank}" + ".";
        newScore.text = $"{lastScore}";

    }


}
