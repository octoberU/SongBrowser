using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

internal static class ScoreDisplayList
{
    static GameObject scoreListDisplay;
    static TextMeshPro scoreListText;
    static RectTransform rect;
    public static void Initialize()
    {
        if (scoreListDisplay == null)
        {
            scoreListDisplay = new GameObject();
            scoreListText = scoreListDisplay.AddComponent<TextMeshPro>();
            rect = scoreListDisplay.GetComponent<RectTransform>();
            rect.transform.position = new Vector3(33.86f, 10.65f, 8.38f);
            rect.sizeDelta = new Vector2(20f, 4f);
            rect.rotation = Quaternion.Euler(new Vector3(0f, 58.395f, 0f));
            scoreListText.fontSize = 16;
            scoreListText.margin = new Vector4(0f, 0f, 0f, -18.82106f);
            scoreListText.text = "No scores found";
        }
    }

    public static void Hide()
    {
        if(scoreListDisplay != null)
        {
            scoreListDisplay.SetActive(false);
        }
    }

    public static void Show()
    {
        if (scoreListDisplay != null)
        {
            scoreListDisplay.SetActive(true);
        }
        else
        {
            Initialize();
            UpdateTextFromList();
        }
    }

    public static void UpdateTextFromList()
    {
        scoreListText.text = CreateDisplayString(ScoreHistory.calculatedScores);
    }

    public static string CreateDisplayString(List<CalculatedScoreEntry> scores)
    {
        string output = $"{((int)ScoreHistory.audicaScore).ToString()}AP <size=60%><color=#999>(Audica Points)</color></size><size=60%>\nTop scores:<size=40%>";
        for (int i = 0; i < 25; i++)
        {
            if(!(i >= scores.Count))
            {
                var song = SongList.I.GetSong(scores[i].songID);
                if(song != null)
                {
                    output += $"\n<color=green>{scores[i].audicaPointsWeighted.ToString("n2")}AP</color>" +
                    " | " +
                    $"{song.artist} - {song.title} " +
                    $"<color=#999>({scores[i].maxScorePercent.ToString("P")})</color>";
                }
            }
        }
        return output;
    }

}
