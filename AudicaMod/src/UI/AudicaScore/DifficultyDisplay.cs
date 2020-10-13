
using AudicaModding;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

internal static class DifficultyDisplay
{
    static GameObject difficultyDisplay;
    static TextMeshPro difficultyText;
    static RectTransform rect;
    public static void Initialize()
    {
        if (difficultyDisplay == null)
        {
            difficultyDisplay = new GameObject();
            difficultyText = difficultyDisplay.AddComponent<TextMeshPro>();
            rect = difficultyDisplay.GetComponent<RectTransform>();
            rect.transform.position = new Vector3(0f, -11.5f, 23.85f);
            rect.sizeDelta = new Vector2(20f, 5f);
            rect.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            difficultyText.fontSize = 18;
            difficultyText.margin = new Vector4(0f, 0f, 0f, -0.7521667f);
            difficultyText.alignment = TextAlignmentOptions.Center;
            difficultyText.text = "Difficulty Display Error";
            difficultyDisplay.layer = 5;
            difficultyText.font = GameObject.Find("menu/ShellPage_Launch/page/ShellPanel_Center/play/Label").GetComponent<TextMeshPro>().font;
        }
    }

    public static void Hide()
    {
        if(difficultyDisplay != null)
        {
            difficultyDisplay.SetActive(false);
            
        }
    }

    public static void Show()
    {
        if (difficultyDisplay != null)
        {
            difficultyDisplay.SetActive(true);
            UpdateTextFromList();
        }
        else
        {
            Initialize();
            UpdateTextFromList();
        }
    }

    public static void UpdateTextFromList()
    {
        difficultyText.text = CreateDisplayString(ScoreHistory.calculatedScores);
    }

    public static string CreateDisplayString(List<CalculatedScoreEntry> scores)
    {
        string output = "Difficulty Rating\n";
        var songData = SongDataHolder.I.songData;
        var calc = new DifficultyCalculator(songData);

        bool hasCustomData = false;
        SongDataLoader.SongData currentSong = SongDataLoader.AllSongData[songData.songID];
        if (currentSong.HasCustomData())
            hasCustomData = true;

        if (calc.expert != null)
        {
            output += $"<color=#b119f7>{calc.expert.difficultyRating.ToString("n2")}</color>  ";
            if (hasCustomData && currentSong.SongHasCustomDataKey("easy360"))
                output += " <color=#b119f7> (360) </color>";
        }
        if (calc.advanced != null)
        {
            output += $"<color=#f7a919>{calc.advanced.difficultyRating.ToString("n2")}</color>  ";
            if (hasCustomData && currentSong.SongHasCustomDataKey("advanced360"))
                output += " <color=#f7a919> (360) </color>";
        }
        if (calc.standard != null)
        {
            output += $"<color=#19d2f7>{calc.standard.difficultyRating.ToString("n2")}</color>  ";
            if (hasCustomData && currentSong.SongHasCustomDataKey("standard360"))
                output += " <color=#19d2f7>(360) </color>";
        }
        if (calc.beginner != null)
        {
            output += $"<color=#54f719>{calc.beginner.difficultyRating.ToString("n2")}</color>  ";
            if (hasCustomData && currentSong.SongHasCustomDataKey("expert360"))
                output += " <color=#54f719> (360)</color>";
        }
        return output;
    }

}
