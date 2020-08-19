using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using MelonLoader;
using UnityEngine;
using MelonLoader.TinyJSON;
using System.IO;
using Harmony;
using System.Media;
using OggDecoder;
using System.Linq;
using TMPro;
using UnityEngine.Events;

namespace AudicaModding
{
    public class SongBrowser : MelonMod
    {
        public static class BuildInfo
        {
            public const string Name = "SongBrowser";  // Name of the Mod.  (MUST BE SET)
            public const string Author = "octo"; // Author of the Mod.  (Set as null if none)
            public const string Company = null; // Company that made the Mod.  (Set as null if none)
            public const string Version = "0.1.0"; // Version of the Mod.  (MUST BE SET)
            public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
        }
        public static string apiURL = "http://www.audica.wiki:5000/api/customsongs?pagesize=14";
        public string downloadPath = null;
        public static APISongList songlist;
        public APISongList fullSongList;
        public static Vector3 DebugTextPosition = new Vector3(0f, -1f, 5f);
        public static bool shouldShowKeyboard = false;
        public static string searchString = "";
        public static bool needRefresh = false;
        public static int page = 1;
        public static string customSongDirectory;
        public static string downloadsDirectory;
        public static bool emptiedDownloadsFolder = false;
        public static bool addedCustomsDir = false;
        public static List<string> deletedSongs = new List<string>();
        public static List<string> deletedSongPaths = new List<string>();


        public override void OnApplicationStart()
        {
            downloadsDirectory = Application.dataPath.Replace("Audica_Data", "Downloads");
            customSongDirectory = Application.dataPath.Replace("Audica_Data", "CustomSongs");
            CheckFolderDirectories();
            StartSongSearch();
            var i = HarmonyInstance.Create("Song Downloader");
            Hooks.ApplyHooks(i);
            FilterPanel.LoadFavorites();
            ScoreHistory.LoadHistory();
        }

        private void CheckFolderDirectories()
        {
            if (!Directory.Exists(downloadsDirectory))
            {
                Directory.CreateDirectory(downloadsDirectory);
            }
            if (!Directory.Exists(customSongDirectory))
            {
                Directory.CreateDirectory(customSongDirectory);
            }
        }

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
            foreach (var songPath in deletedSongPaths)
            {
                if (File.Exists(songPath))
                {
                    File.Delete(songPath);
                }
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
            if (Input.GetKeyDown(KeyCode.F3))
            {
                FilterPanel.GetReferences();
                FilterPanel.SetNotificationText("There are 3 new songs available in the song downloader.");
            }
        }

        IEnumerator PlayOggCoroutine(string oggFilename)
        {
            using (var file = new FileStream(oggFilename, FileMode.Open, FileAccess.Read))
            {
                var player = new SoundPlayer(new OggDecodeStream(file));
                player.Play();
                yield return new WaitForSeconds(10f);
            }
            yield return null;
        }


        public static void ReloadSongList()
        {
            needRefresh = false;
            SongList.sFirstTime = true;
            SongList.OnSongListLoaded.mDone = false;
            SongList.SongSourceDirs = new Il2CppSystem.Collections.Generic.List<SongList.SongSourceDir>();
            SongList.AddSongSearchDir(Application.dataPath, downloadsDirectory);
            SongList.I.StartAssembleSongList();
            SongSelect songSelect = GameObject.FindObjectOfType<SongSelect>();
            if (songSelect != null)
            {
                SongList.OnSongListLoaded.On(new Action(() => { songSelect.ShowSongList(); }));
            }
            DebugText("Reloading Songs");
        }

        public static void StartSongSearch()
        {
            MelonCoroutines.Start(StartSongSearchCoroutine(searchString, SongDownloaderUI.difficultyFilter.ToString(), page, false));
        }

        public static IEnumerator StartSongSearchCoroutine(string search, string difficulty = null, int page = 1, bool total = false)
        {
            string webSearch = search == null || search == "" ? "" : "&search=" + WebUtility.UrlEncode(search);
            string webPage = page == 1 ? "" : "&page=" + page.ToString();
            string webDifficulty = difficulty == "All" || difficulty == "" ? "" : "&" + difficulty.ToLower() + "=true";
            string webCurated = SongDownloaderUI.curated ? "&curated=true" : "";
            string concatURL = !total ? apiURL + webSearch + webDifficulty + webPage + webCurated : "http://www.audica.wiki:5000/api/customsongs?pagesize=all";
            WWW www = new WWW(concatURL);
            yield return www;
            songlist = JSON.Load(www.text).Make<APISongList>();
            if(SongDownloaderUI.songItemPanel != null)
            {
                SongDownloaderUI.AddSongItems(SongDownloaderUI.songItemMenu, songlist);
            }
        }


        public static IEnumerator DownloadSong(string downloadUrl)
        {
            string[] splitURL = downloadUrl.Split('/');
            string audicaName = splitURL[splitURL.Length - 1];
            string path = Application.streamingAssetsPath + "\\HmxAudioAssets\\songs\\" + audicaName;
            string customSongsPath = customSongDirectory + "\\" + audicaName;
            string downloadPath = downloadsDirectory + "\\" + audicaName;
            if (!File.Exists(path) && !File.Exists(downloadPath) && !File.Exists(downloadPath))
            {
                WWW www = new WWW(downloadUrl);
                yield return www;
                byte[] results = www.bytes;
                File.WriteAllBytes(downloadPath, results);
                needRefresh = true;
            }
            yield return null;
        }


        public static IEnumerator StreamPreviewSong(string url)
        {
            WWW www = new WWW(url);
            yield return www;
            if (www.isDone)
            {
                Stream stream = new MemoryStream(www.bytes);
                var player = new SoundPlayer(new OggDecodeStream(stream));
                //player.LoadAsync();
                yield return new WaitForSeconds(0.2f);
                player.Play();
                yield return new WaitForSeconds(15f);
            }

            yield return null;
        }


        public static void DebugText(string text)
        {
            KataConfig.I.CreateDebugText(text, DebugTextPosition, 5f, null, false, 0.2f);
        }

        public static void NextPage()
        {
            if (page > songlist.total_pages)
                page = songlist.total_pages;
            else if (page < 1)
                page = 1;
            else
                page++;
        }
        public static void PreviousPage()
        {
            if (page == 1) return;
            if (page > songlist.total_pages)
                page = songlist.total_pages;
            else if (page < 1)
                page = 1;
            else
                page--;
        }


        public static string GetDifficultyString(bool hasEasy, bool hasStandard, bool hasAdvanced, bool hasExpert)
        {
            return "[" +
                (hasEasy ? "<color=#54f719>B</color>" : "") +
                (hasStandard ? "<color=#19d2f7>S</color>" : "") +
                (hasAdvanced ? "<color=#f7a919>A</color>" : "") +
                (hasExpert ? "<color=#b119f7>E</color>" : "") +
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
public class APISongList
{
    public int total_pages;
    public int song_count;
    public Song[] songs;
    public int pagesize;
    public int page;
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
