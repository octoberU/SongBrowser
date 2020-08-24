using AudicaModding;
using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public struct AudicaScore
{
    public string songID;
    public int score;
    public string difficultyString;
    public float maxScorePercent;
    public DateTime date;
    public int combo;
    public int maxCombo;
    public int version;
    public AudicaScore(string songID, int score, float maxScorePercent, KataConfig.Difficulty difficulty, int combo, int maxCombo)
    {
        this.songID = songID;
        this.score = score;
        this.maxScorePercent = maxScorePercent;
        this.difficultyString = difficulty.ToString("g");
        this.combo = combo;
        this.maxCombo = maxCombo;
        date = DateTime.Now;
        version = 4;
    }
}

public class CalculatedScoreEntry
{
    public string songID;
    public double audicaPointsRaw;
    public double audicaPointsWeighted;
    public float maxScorePercent;
    public AudicaScore localScore;
    public CalculatedScoreEntry(AudicaScore localScore)
    {
        this.localScore = localScore;
        this.songID = localScore.songID;
        this.maxScorePercent = localScore.maxScorePercent;
        this.audicaPointsWeighted = 0f;
        this.audicaPointsRaw = DifficultyCalculator.GetRating(songID, localScore.difficultyString) * maxScorePercent * 30;
    }
}
[Serializable]
public struct LocalPlayHistory
{
    public List<AudicaScore> scores;
    public string leaderboardID;

    public LocalPlayHistory(List<AudicaScore> scores, string leaderboardID)
    {
        this.scores = scores;
        this.leaderboardID = leaderboardID;
    }
}
internal static class ScoreHistory
{
    public static double audicaScore = 0f;
    static double lastAudicaScore = 0f;
    static int scoreVersion = 4;
    static List<AudicaScore> scores = new List<AudicaScore>();
    static string historySavePath = Application.dataPath + "/../" + "/UserData/";
    public static List<CalculatedScoreEntry> calculatedScores = new List<CalculatedScoreEntry>();
    static bool historyLoaded = false;

    public static void AddScore(string songID, int score, float maxScorePercent, float difficultyRating, int combo, int maxCombo, KataConfig.Difficulty difficulty)
    {
        var scoreToAdd = new AudicaScore(songID, score, maxScorePercent, difficulty, combo, maxCombo);
        var previousScore = scores.FirstOrDefault(previous => previous.songID == songID);
        if (!previousScore.Equals(default(AudicaScore)))
        {
            if (scoreToAdd.score < previousScore.score)
            {
                return;
            }
            else
            {
                scores.Add(scoreToAdd);
                scores.Remove(previousScore);
            }
        }
        else
        {
            scores.Add(scoreToAdd);
        }
        lastAudicaScore = audicaScore;
        audicaScore = CalculateTotalRating();
        SongBrowser.DebugText($"<color=green>+{(audicaScore - lastAudicaScore).ToString("n2")}</color>");
        SaveHistory(PlatformChooser.I.GetLeaderboardID());
    }

    public static double CalculateTotalRating()
    {
        //scores.Sort((a, b) => b.ratedScore.CompareTo(a.ratedScore));
        var mCalculatedScores = new List<CalculatedScoreEntry>();
        for (int i = 0; i < scores.Count; i++)
        {
            mCalculatedScores.Add(new CalculatedScoreEntry(scores[i]));   
        }
        calculatedScores = mCalculatedScores.OrderByDescending(x => x.audicaPointsRaw).ToList();
        double totalRating = 0f;
        for (int i = 0; i < calculatedScores.Count; i++)
        {
            calculatedScores[i].audicaPointsWeighted = calculatedScores[i].audicaPointsRaw * Math.Pow(0.95f, i);
            totalRating += calculatedScores[i].audicaPointsWeighted;
            //totalRating += scores[i].ratedScore * Math.Pow(0.95f, i);
        }
        return totalRating;
    }

    [HarmonyPatch(typeof(SongPlayHistory), "RecordHistory", new Type[] { typeof(string), typeof(int), typeof(KataConfig.Difficulty), typeof(float), typeof(bool), typeof(bool)})]
    private static class AddAudicaScore
    {
        private static int counter = 2;
        private static void Postfix(SongPlayHistory __instance, string songID, int score, KataConfig.Difficulty difficulty, float percent, bool fullCombo, bool noFail)
        {
            if (counter % 2 == 0)
            {
                MelonLogger.Log("Recorded new score!");
                if (noFail) return;
                if (percent < 30f) return;
                float maxScorePercent = (float)score / (float)StarThresholds.I.GetMaxRawScore(songID, difficulty);
                float difficultyRating = new DifficultyCalculator(SongList.I.GetSong(songID)).GetRatingFromKataDifficulty(difficulty);
                AddScore(songID, score, maxScorePercent, difficultyRating, ScoreKeeper.I.mStreak, ScoreKeeper.I.mMaxStreak, difficulty); 
            }
            counter++;
        }
    }

    private static void SaveHistory(string leaderboardID)
    {
        if (leaderboardID == "")
            return;
        FileStream fs = new FileStream(historySavePath + leaderboardID, FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        try
        {
            formatter.Serialize(fs, new LocalPlayHistory(scores, leaderboardID));
        }
        catch (SerializationException e)
        {
            Console.WriteLine("Failed to serialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
            MelonLogger.Log("Saved play history!");
        }
    }

    public static void LoadHistory(string leaderboardID)
    {
        if (historyLoaded) return;
        if (leaderboardID == "")
            return;
        historyLoaded = true;
        scores = null;
        LocalPlayHistory localhistory;
        if (!File.Exists(historySavePath + leaderboardID))
        {
            scores = new List<AudicaScore>();
            MelonLogger.Log("No history found, creating new history");
            return;

        }

        FileStream fs = new FileStream(historySavePath + leaderboardID, FileMode.Open);
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            localhistory = (LocalPlayHistory)formatter.Deserialize(fs);
            if (localhistory.leaderboardID == leaderboardID)
            {
                scores = localhistory.scores;
            }
            else
            {
                scores = new List<AudicaScore>();
                MelonLogger.Log("Wrong user, creating new history instead");
                return;
            }
                
        }
        catch (SerializationException e)
        {
            Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
        }
        MelonLogger.Log($"Loaded {scores.Count.ToString()} scores");
        audicaScore = lastAudicaScore = CalculateTotalRating();
        ScoreDisplayList.UpdateTextFromList();
    }

}


