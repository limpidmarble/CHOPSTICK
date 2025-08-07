using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

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
        List<int> scores = new List<int>();

        while (i <= 101)
        {
            prefKey = "score" + $"{i}";
            if (PlayerPrefs.HasKey(prefKey))
            {
                scores.Add(PlayerPrefs.GetInt(prefKey));
                i++;
            }
            else
            {
                break;
            }
        }

        
        

    }


}
