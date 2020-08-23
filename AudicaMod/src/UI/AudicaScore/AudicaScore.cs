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
        this.audicaPointsRaw = DifficultyCalculator.GetRating(songID, localScore.difficultyString) * maxScorePercent * 30;
    }
}

internal static class ScoreHistory
{
    static double audicaScore = 0f;
    static double lastAudicaScore = 0f;
    static int scoreVersion = 4;
    static List<AudicaScore> scores = new List<AudicaScore>();
    static string historySavePath = Application.dataPath + "/../" + "/UserData/" + "AudicaScoreHistory.dat";
    static List<CalculatedScoreEntry> calculatedScores;

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
        audicaScore = CalculateTotalRating();
        SaveHistory();
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
                float maxScorePercent = (float)score / (float)StarThresholds.I.GetMaxRawScore(songID, difficulty);
                float difficultyRating = new DifficultyCalculator(SongList.I.GetSong(songID)).GetRatingFromKataDifficulty(difficulty);
                AddScore(songID, score, maxScorePercent, difficultyRating, ScoreKeeper.I.mStreak, ScoreKeeper.I.mMaxStreak, difficulty); 
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


