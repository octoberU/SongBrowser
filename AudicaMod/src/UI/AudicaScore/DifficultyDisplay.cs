
using AudicaModding;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static DifficultyCalculator;

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

    private struct fillData
    {
        public List<string> easyAdditions;
        public List<string> standardAdditions;
        public List<string> advancedAdditions;
        public List<string> expertAdditions;
    }
    //could be used for other info in the future
    public static List<string> CreateDisplayStringAdditions(SongDataLoader.SongData currentSong, string tagtofind)
    {
        if (currentSong.HasCustomData())
        {
            //if adding 360 tags
            if (tagtofind == "customExpertTags" || tagtofind == "customEasyTags" || tagtofind == "customStandardTags" || tagtofind == "customAdvancedTags")
            {
                if (currentSong.SongHasCustomDataKey(tagtofind))
                {
                    List<string> customTags = currentSong.GetCustomData<List<string>>(tagtofind);

             
                    return customTags;
                }
            }

            //TODO: could do more tags here
        }

        return new List<string>();
    }

    private static fillData fillAdditions(string songID, fillData d)
    {
        if (SongDataLoader.AllSongData.ContainsKey(songID))
        {
            SongDataLoader.SongData currentSong = SongDataLoader.AllSongData[songID];
            d.easyAdditions = CreateDisplayStringAdditions(currentSong, "customEasyTags");
            d.advancedAdditions = CreateDisplayStringAdditions(currentSong, "customAdvancedTags");
            d.standardAdditions = CreateDisplayStringAdditions(currentSong, "customStandardTags");
            d.expertAdditions = CreateDisplayStringAdditions(currentSong, "customExpertTags");
        }

        return d;
    }

    public static string CreateDisplayString(List<CalculatedScoreEntry> scores)
    {
        string output = "Difficulty Rating\n";
        var songData = SongDataHolder.I.songData;

        fillData AdditionHolder = new fillData();
        AdditionHolder.easyAdditions = new List<string>();
        AdditionHolder.advancedAdditions = new List<string>();
        AdditionHolder.standardAdditions = new List<string>();
        AdditionHolder.expertAdditions = new List<string>();

        if (SongBrowser.songDataLoaderInstalled)
        {
            //fillAdditions() is separated into its own function to avoid errors if !songDataLoaderInstalled
            /*Explanation:
            When CreateDisplayString() is called the first time it loads in all the functions it needs.
            If we include anything from the Song Data Loader utility in CreateDisplayString() it will try to
            load from the dll. In the case they dont have the dll, it will throw an exception and the rest of
            CreateDisplayString() won't run. Since all mentions of Son Data Loader are in fillAdditions(), which 
            is behind a conditional we wont have that issue.
            */
           
            AdditionHolder = fillAdditions(songData.songID, AdditionHolder);
        }

        CachedCalculation easy = DifficultyCalculator.GetRating(songData.songID, KataConfig.Difficulty.Easy.ToString());
        CachedCalculation normal = DifficultyCalculator.GetRating(songData.songID, KataConfig.Difficulty.Normal.ToString());
        CachedCalculation hard = DifficultyCalculator.GetRating(songData.songID, KataConfig.Difficulty.Hard.ToString());
        CachedCalculation expert = DifficultyCalculator.GetRating(songData.songID, KataConfig.Difficulty.Expert.ToString());


        //add mine tags if it has mines

        if (songData.hasEasy && easy.hasMines) AdditionHolder.easyAdditions.Insert(0, "Mines");

        if (songData.hasNormal && normal.hasMines) AdditionHolder.standardAdditions.Insert(0, "Mines");

        if (songData.hasHard && hard.hasMines) AdditionHolder.advancedAdditions.Insert(0, "Mines");

        if (songData.hasExpert && expert.hasMines) AdditionHolder.expertAdditions.Insert(0, "Mines");

        //add 360 if it is

        if (songData.hasEasy && easy.is360) AdditionHolder.easyAdditions.Insert(0, "360");

        if (songData.hasNormal && normal.is360) AdditionHolder.standardAdditions.Insert(0, "360");

        if (songData.hasHard && hard.is360) AdditionHolder.advancedAdditions.Insert(0, "360");

        if (songData.hasExpert && expert.is360) AdditionHolder.expertAdditions.Insert(0, "360");

        AdditionHolder.expertAdditions = AdditionHolder.expertAdditions.Distinct().ToList();
        AdditionHolder.standardAdditions = AdditionHolder.standardAdditions.Distinct().ToList();
        AdditionHolder.advancedAdditions = AdditionHolder.advancedAdditions.Distinct().ToList();
        AdditionHolder.easyAdditions = AdditionHolder.easyAdditions.Distinct().ToList();


        if (expert.value != 0)
        {
            output += $"<color=#b119f7>{expert.value.ToString("n2")} ";
            output += AdditionHolder.expertAdditions.Count > 0 ? "(" + string.Join(", ", AdditionHolder.expertAdditions.ToArray()) + ")</color> " : "</color> ";
        }
        if (hard.value != 0)
        {
            output += $"<color=#f7a919>{hard.value.ToString("n2")} ";
            output += AdditionHolder.advancedAdditions.Count > 0 ? "(" + string.Join(", ", AdditionHolder.advancedAdditions.ToArray()) + ")</color> " : "</color> ";
        }
        if (normal.value != 0)
        {
            output += $"<color=#19d2f7>{normal.value.ToString("n2")} ";
            output += AdditionHolder.standardAdditions.Count > 0 ? "(" + string.Join(", ", AdditionHolder.standardAdditions.ToArray()) + ")</color> " : "</color> ";
        }
        if (easy.value != 0)
        {
            output += $"<color=#54f719>{easy.value.ToString("n2")} ";
            output += AdditionHolder.easyAdditions.Count > 0 ? "(" + string.Join(", ", AdditionHolder.easyAdditions.ToArray()) + ")</color> " : "</color> ";
        }
        return output;
    }

}
