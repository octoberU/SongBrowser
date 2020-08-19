using UnityEngine;
using AudicaModding;
using MelonLoader;
internal static class DiffCalculatorUtil
{
    public static void ExportDifficultyCalculation(bool extras)
    {
        var songIDs = GameObject.FindObjectOfType<SongSelect>().GetSongIDs(extras);
        using (System.IO.StreamWriter file =
        new System.IO.StreamWriter(Application.dataPath + "/../" + "difficultyCalculatorOutput.txt"))
        {
            file.WriteLine("Song Name, Difficulty, Difficulty Rating, BPM, Author");
            for (int i = 0; i < songIDs.Count; i++)
            {
                var songData = SongList.I.GetSong(songIDs[i]);
                var calc = new DifficultyCalculator(songData);
                if (calc.expert != null) file.WriteLine($"{SongBrowser.RemoveFormatting(songData.artist).Replace(",", "")} - {SongBrowser.RemoveFormatting(songData.title).Replace(",", "")}," +
                    "Expert," +
                    $"{calc.expert.difficultyRating.ToString("n2")}," +
                    $"{songData.tempos[0].tempo.ToString("n2")}," +
                    $"{songData.author}");
                if (calc.advanced != null) file.WriteLine($"{SongBrowser.RemoveFormatting(songData.artist).Replace(",", "")} - {SongBrowser.RemoveFormatting(songData.title).Replace(",", "")}," +
                    "Advanced," +
                    $"{calc.advanced.difficultyRating.ToString("n2")}," +
                    $"{songData.tempos[0].tempo.ToString("n2")}," +
                    $"{songData.author}");
                if (calc.standard != null) file.WriteLine($"{SongBrowser.RemoveFormatting(songData.artist).Replace(",", "")} - {SongBrowser.RemoveFormatting(songData.title).Replace(",", "")}," +
                    "Standard," +
                    $"{calc.standard.difficultyRating.ToString("n2")}," +
                    $"{songData.tempos[0].tempo.ToString("n2")}," +
                    $"{songData.author}");
                if (calc.beginner != null) file.WriteLine($"{SongBrowser.RemoveFormatting(songData.artist).Replace(",", "")} - {SongBrowser.RemoveFormatting(songData.title).Replace(",", "")}," +
                    "Beginner," +
                    $"{calc.beginner.difficultyRating.ToString("n2")}," +
                    $"{songData.tempos[0].tempo.ToString("n2")}," +
                    $"{songData.author}");
            }
        }
    }

    public static void LogCurrentSongDifficulty()
    {
        var calc = new DifficultyCalculator(SongDataHolder.I.songData);
        MelonLogger.Log("\n" + calc.songID);
        if (calc.expert != null) MelonLogger.Log("\nExpert: " + calc.expert.difficultyRating.ToString());
        if (calc.advanced != null) MelonLogger.Log("\nAdvanced: " + calc.advanced.difficultyRating.ToString());
        if (calc.standard != null) MelonLogger.Log("\nStandard: " + calc.standard.difficultyRating.ToString());
        if (calc.beginner != null) MelonLogger.Log("\nBeginner: " + calc.beginner.difficultyRating.ToString());
    }
}