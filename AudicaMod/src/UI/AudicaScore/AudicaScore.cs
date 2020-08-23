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
    public float maxScorePercent;
    public float difficultyRating;
    public float ratedScore;
    public DateTime date;
    public int combo;
    public int maxCombo;
    public int version;
    public AudicaScore(string songID, int score, float maxScorePercent, float difficultyRating, int combo, int maxCombo)
    {
        this.songID = songID;
        this.score = score;
        this.maxScorePercent = maxScorePercent;
        this.difficultyRating = difficultyRating;
        this.combo = combo;
        this.maxCombo = maxCombo;
        this.ratedScore = difficultyRating * maxScorePercent * 30;
        date = DateTime.Now;
        version = 2;
    }
}

internal static class ScoreHistory
{
    static double audicaScore = 0f;
    static double lastAudicaScore = 0f;
    static int scoreVersion = 2;
    static List<AudicaScore> scores = new List<AudicaScore>();
    static string historySavePath = Application.dataPath + "/../" + "/UserData/" + "AudicaScoreHistory.dat";

    public static void AddScore(string songID, int score, float maxScorePercent, float difficultyRating, int combo, int maxCombo)
    {
        var scoreToAdd = new AudicaScore(songID, score, maxScorePercent, difficultyRating, combo, maxCombo);
        var previousScore = scores.FirstOrDefault(previous => previous.songID == songID);
        if (!previousScore.Equals(default(AudicaScore)))
        {
            if (scoreToAdd.ratedScore < previousScore.ratedScore)
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
        MelonLogger.Log($"Max possible score from song: {(scoreToAdd.difficultyRating * 30).ToString("n2")}");
        MelonLogger.Log($"Achieved score from current song: {scoreToAdd.ratedScore.ToString("n2")}");
        MelonLogger.Log($"AudicaScore: {audicaScore.ToString("n2")}");
        SongBrowser.DebugText($"AudicaScore: {audicaScore.ToString("n2")}[<color=#28bf50>+{(audicaScore - lastAudicaScore).ToString("n2")}</color>]");
        SaveHistory();
    }

    public static double CalculateTotalRating()
    {
        scores.Sort((a, b) => b.ratedScore.CompareTo(a.ratedScore));
        double totalRating = 0f;
        for (int i = 0; i < scores.Count; i++)
        {
            totalRating += scores[i].ratedScore * Math.Pow(0.95f, i);
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
                float maxScorePercent = (float)score / (float)StarThresholds.I.GetMaxRawScore(songID, difficulty);
                float difficultyRating = new DifficultyCalculator(SongList.I.GetSong(songID)).GetRatingFromKataDifficulty(difficulty);
                AddScore(songID, score, maxScorePercent, difficultyRating, ScoreKeeper.I.mStreak, ScoreKeeper.I.mMaxStreak); 
            }
            counter++;
        }
    }

    private static void SaveHistory()
    {
        FileStream fs = new FileStream(historySavePath, FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        try
        {
            formatter.Serialize(fs, scores);
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

    public static void LoadHistory()
    {
        scores = null;
        if (!File.Exists(historySavePath))
        {
            scores = new List<AudicaScore>();
            MelonLogger.Log("No history found, creating new history");
            return;

        }

        FileStream fs = new FileStream(historySavePath, FileMode.Open);
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            scores = (List<AudicaScore>)formatter.Deserialize(fs);
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
    }

}


