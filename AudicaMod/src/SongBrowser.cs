using System;
using System.Collections;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using MelonLoader.TinyJSON;
using System.IO;
using Harmony;
using System.Linq;
[assembly: MelonOptionalDependencies("SongDataLoader", "ModSettings")]

namespace AudicaModding
{
    public class SongBrowser : MelonMod
    {
        public static class BuildInfo
        {
            public const string Name = "SongBrowser";  // Name of the Mod.  (MUST BE SET)
            public const string Author = "octo"; // Author of the Mod.  (Set as null if none)
            public const string Company = null; // Company that made the Mod.  (Set as null if none)
            public const string Version = "2.2.2"; // Version of the Mod.  (MUST BE SET)
            public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
        }
        public static Vector3 DebugTextPosition = new Vector3(0f, -1f, 5f);
        public static bool shouldShowKeyboard = false;
        public static string downloadsDirectory;
        public static string deletedDownloadsListPath;
        public static bool emptiedDownloadsFolder = false;
        public static bool addedCustomsDir = false;
        public static List<string> deletedSongs = new List<string>();
        public static List<string> deletedSongPaths = new List<string>();
        public static int newSongCount;
        public static int lastSongCount;

        public static HashSet<string> songIDs = new HashSet<string>();

        public static bool modSettingsInstalled = false;

        //Meeps' Stuff
        public static bool songDataLoaderInstalled = false;
        public class SongDisplayPackage
        {
            public bool hasEasy = false;
            public bool hasStandard = false;
            public bool hasAdvanced = false;
            public bool hasExpert = false;

            public List<string> customEasyTags = new List<string>();
            public List<string> customStandardTags = new List<string>();
            public List<string> customAdvancedTags = new List<string>();
            public List<string> customExpertTags = new List<string>();

            public static SongDisplayPackage FillCustomData(SongDisplayPackage songd, string songID)
            {
                if (SongDataLoader.AllSongData.ContainsKey(songID))
                {
                    SongDataLoader.SongData currentSong = SongDataLoader.AllSongData[songID];
                    if (currentSong.HasCustomData())
                    {
                        if (currentSong.SongHasCustomDataKey("customEasyTags"))
                        {
                            songd.customEasyTags = currentSong.GetCustomData<List<string>>("customEasyTags");                           
                        }
                        if (currentSong.SongHasCustomDataKey("customStandardTags"))
                        {
                            songd.customStandardTags = currentSong.GetCustomData<List<string>>("customStandardTags");
                        }
                        if (currentSong.SongHasCustomDataKey("customAdvancedTags"))
                        {
                            songd.customAdvancedTags = currentSong.GetCustomData<List<string>>("customAdvancedTags");
                        }
                        if (currentSong.SongHasCustomDataKey("customExpertTags"))
                        {
                            songd.customExpertTags = currentSong.GetCustomData<List<string>>("customExpertTags");
                        }

                    }
                }
                return songd;
            }
        }

        private void CreatePrivateConfig()
        {
            MelonPrefs.RegisterInt("RandomSong", "RandomSongBagSize", 10);
            MelonPrefs.RegisterInt("SongBrowser", "LastSongCount", 0);
        }

        private void LoadPrivateConfig()
        {
            RandomSong.LoadBagSize(MelonPrefs.GetInt("RandomSong", "RandomSongBagSize"));
            lastSongCount = MelonPrefs.GetInt("SongBrowser", "LastSongCount");
        }

        public static void SavePrivateConfig()
        {
            MelonPrefs.SetInt("RandomSong", "RandomSongBagSize", RandomSong.randomSongBagSize);
            MelonPrefs.SetInt("SongBrowser", "LastSongCount", lastSongCount);
        }

        public override void OnModSettingsApplied()
        {
            Config.OnModSettingsApplied();
        }

        public override void OnLevelWasLoaded(int level)
        {

            if (!MelonPrefs.HasKey("RandomSong", "RandomSongBagSize") || !MelonPrefs.HasKey("SongBrowser", "LastSongCount"))
            {
                CreatePrivateConfig();
            }
            else
            {
                LoadPrivateConfig();
            }
        }

        public override void OnApplicationStart()
        {
            Config.RegisterConfig();
            downloadsDirectory       = Application.dataPath.Replace("Audica_Data", "Downloads");
            deletedDownloadsListPath = Path.Combine(downloadsDirectory, "SongBrowserDownload_DeletedFiles");
            CheckFolderDirectories();
            SongDownloader.StartNewSongSearch();
            var i = HarmonyInstance.Create("Song Downloader");
            FilterPanel.LoadFavorites();

            if (MelonHandler.Mods.Any(it => it.Info.SystemType.Name == nameof(SongDataLoader)))
            {
                songDataLoaderInstalled = true;
                MelonLogger.Log("Song Data Loader is installed. Enabling integration");
            }
            else
                MelonLogger.LogWarning("Song Data Loader is not installed. Consider downloading it for the best experience :3");

            if (MelonHandler.Mods.Any(it => it.Info.SystemType.Name == nameof(ModSettings)))
            {
                modSettingsInstalled = true;
            }
        }

        private void CheckFolderDirectories()
        {
            if (!Directory.Exists(downloadsDirectory))
            {
                Directory.CreateDirectory(downloadsDirectory);
            }
        }

        //public override void OnGUI()
        //{
        //    if (GUI.Button(new Rect(10, 10, 150, 100), "Show scores"))
        //    {
        //        ScoreDisplayList.Initialize();
        //    }
        //    if (GUI.Button(new Rect(10, 110, 150, 100), "Update"))
        //    {
        //        ScoreDisplayList.UpdateTextFromList();
        //    }
        //}

        public override void OnApplicationQuit()
        {
            FilterPanel.SaveFavorites();
            CleanDeletedSongs();
        }

        public static void RestoreDeletedSongs()
        {
            deletedSongPaths = new List<string>();
            deletedSongs = new List<string>();
            DebugText("Restored songs");
        }

        public static void CleanDeletedSongs()
        {
            List<string> downloadsMarkedForDeletion = new List<string>();
            foreach (var songPath in deletedSongPaths)
            {
                if (File.Exists(songPath))
                {
                    File.Delete(songPath);
                }
                else
                {
                    // could be a fresh download
                    string fileName     = Path.GetFileName(songPath);
                    string downloadPath = Path.Combine(downloadsDirectory, fileName);
                    if (File.Exists(downloadPath))
                    {
                        // can't delete while game is still running since it has a write-lock
                        downloadsMarkedForDeletion.Add(fileName);
                    }
                }
            }
            if (downloadsMarkedForDeletion.Count > 0)
            {
                // store these so they can be deleted on next launch
                string text = JSON.Dump(downloadsMarkedForDeletion);
                File.WriteAllText(deletedDownloadsListPath, text);
            }
        }

        public static void RemoveSong(string songID)
        {
            if (deletedSongs.Contains(songID))
            {
                return;
            }
            var song = SongList.I.GetSong(songID);
            deletedSongPaths.Add(song.searchRoot + "/" + song.zipPath);
            deletedSongs.Add(song.songID);
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                ReloadSongList();
            }
            //if (Input.GetKeyDown(KeyCode.F3))
            //{
            //    FilterPanel.GetReferences();
            //    FilterPanel.SetNotificationText("There are 3 new songs available in the song downloader.");
            //}
        }

        public static void ReloadSongList()
        {
            SongDownloader.needRefresh = false;
            SongList.sFirstTime = true;
            SongList.OnSongListLoaded.mDone = false;
            SongList.SongSourceDirs = new Il2CppSystem.Collections.Generic.List<SongList.SongSourceDir>();
            SongList.AddSongSearchDir(Application.dataPath, downloadsDirectory);
            SongList.I.StartAssembleSongList();

            SongLoadingManager.StartSongListUpdate();

            DebugText("Reloading Songs");
        }

        public static void UpdateSongCaches()
        {
            songIDs.Clear();
            for (int i = 0; i < SongList.I.songs.Count; i++)
            {
                string songID = SongList.I.songs[i].songID;
                songIDs.Add(songID);

                DifficultyCalculator.GetRating(songID, KataConfig.Difficulty.Easy.ToString());
                DifficultyCalculator.GetRating(songID, KataConfig.Difficulty.Normal.ToString());
                DifficultyCalculator.GetRating(songID, KataConfig.Difficulty.Hard.ToString());
                DifficultyCalculator.GetRating(songID, KataConfig.Difficulty.Expert.ToString());
            }
        }

        public static IEnumerator UpdateLastSongCount()
        {
            string URL = "http://www.audica.wiki:5000/api/customsongstotal";
            WWW www = new WWW(URL);
            yield return www;
            var songcount = JSON.Load(www.text).Make<TotalSongs>();
            newSongCount = songcount.song_count;
            if (FilterPanel.notificationPanel != null)
            {
                if (lastSongCount == newSongCount) FilterPanel.SetNotificationText("There are no new songs available");
                else
                {
                    int _count = newSongCount - lastSongCount;
                    bool isSingular = (newSongCount - lastSongCount) == 1;
                    string preSongtxt = isSingular ? "is " : "are ";
                    string songtxt = isSingular ? "song" : "songs";
                    FilterPanel.SetNotificationText("There " + preSongtxt + _count.ToString() + " new " + songtxt + " available");
                }
                    
            }
        }

        public static void DebugText(string text)
        {
            KataConfig.I.CreateDebugText(text, DebugTextPosition, 5f, null, false, 0.2f);
        }

        public static string GetDifficultyString(SongDisplayPackage songD)
        {
            return "[" +
                (songD.hasEasy ? "<color=#54f719>B</color>" : "") + (songD.hasEasy && (songD.customEasyTags.Count) > 0 ? "<color=#54f719>(" + songD.customEasyTags.ToArray<string>().Join() + ") </color>" : "") +
                (songD.hasStandard ? "<color=#19d2f7>S</color>" : "") + (songD.hasStandard && (songD.customStandardTags.Count) > 0 ? "<color=#19d2f7> (" + songD.customStandardTags.ToArray<string>().Join() + ") </color>" : "") +
                (songD.hasAdvanced ? "<color=#f7a919>A</color>" : "") + (songD.hasAdvanced && (songD.customAdvancedTags.Count) > 0 ? "<color=#f7a919> (" + songD.customAdvancedTags.ToArray<string>().Join() + ") </color>" : "") +
                (songD.hasExpert ? "<color=#b119f7>E</color>" : "") + (songD.hasExpert && (songD.customExpertTags.Count) > 0 ? "<color=#b119f7> (" + songD.customExpertTags.ToArray<string>().Join() + ")</color>" : "") +
                "]";
        }

        public static string RemoveFormatting(string input)
        {
            System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex("<[^>]*>");
            return rx.Replace(input, "");
        }

    }
}

[Serializable]
public class Song
{
    public string song_id;
    public string author;
    public string title;
    public string artist;
    public bool beginner;
    public bool standard;
    public bool advanced;
    public bool expert;
    public string download_url;
    public string preview_url;
    public string upload_time;
    public int leaderboard_scores;
    public string video_url;
    public string filename;
    public DateTime GetDate()
    {
        string[] day = this.upload_time.Split(new char[] { ' ', '-' });
        string[] time = this.upload_time.Split(new char[] { ' ', ':', '.' });
        return new DateTime(Int32.Parse(day[0]),
            Int32.Parse(day[1]),
            Int32.Parse(day[2]),
            Int32.Parse(time[1]),
            Int32.Parse(time[2]),
            Int32.Parse(time[3]));
    }
}

[Serializable]
public class TotalSongs
{
    public int song_count;
    public int author_count;
}