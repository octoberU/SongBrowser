using System;
using System.Collections.Generic;

public struct AudicaScore
{
    public int score;
    public float maxScorePercent;
    public float difficultyRating;
    public float ratedScore;
    public DateTime date;
    int combo;
    int maxCombo;
    public AudicaScore(int score, float maxScorePercent, float difficultyRating, int combo, int maxCombo)
    {
        this.score = score;
        this.maxScorePercent = maxScorePercent;
        this.difficultyRating = difficultyRating;
        this.combo = combo;
        this.maxCombo = maxCombo;
        this.ratedScore = difficultyRating * (maxScorePercent / 100) * 30;
        date = DateTime.Now;
    }
}

public class ScoreHistory
{
    double totalRating = 0f;
    List<AudicaScore> scores = new List<AudicaScore>();

    public void AddScore(int score, float maxScorePercent, float difficultyRating, int combo, int maxCombo)
    {
        scores.Add(new AudicaScore(score, maxScorePercent, difficultyRating, combo, maxCombo));
    }

    public double CalculateTotalRating()
    {
        scores.Sort((a, b) => b.ratedScore.CompareTo(a.ratedScore));
        double TotalRating = 0f;
        for (int i = 0; i < scores.Count; i++)
        {
            totalRating += scores[i].ratedScore * Math.Pow(0.95f, i);
        }
        return totalRating;
    }
}