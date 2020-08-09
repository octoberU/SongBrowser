using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using MelonLoader;
using UnityEngine;
using MelonLoader.TinyJSON;
using System.IO;
using Harmony;


namespace AudicaModding
{
    public class AudicaMod : MelonMod
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
        public DownloadHistory downloadHistory;
        private string historyPath;
        public static Vector3 DebugTextPosition = new Vector3(0f, -1f, 5f);
        public static bool shouldShowKeyboard = false;
        public static string searchString = "";
        public static bool needRefresh = false;
        public static int page = 1;

        public override void OnApplicationStart()
        {
            StartSongSearch();
            var i = HarmonyInstance.Create("Song Downloader");
            Hooks.ApplyHooks(i);

        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                ReloadSongList();
            }
            if (Input.GetKeyDown(KeyCode.F3))
            {
                MelonLogger.Log( OptionsMenu.I.scrollable.GetMaxScroll().ToString());
                MelonLogger.Log(OptionsMenu.I.scrollable.mRows.Count.ToString());
            }
        }

        public static void ReloadSongList()
        {
            needRefresh = false;
            SongList.sFirstTime = true;
            SongList.OnSongListLoaded.mDone = false;
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
            MelonLogger.Log(concatURL);
            WWW www = new WWW(concatURL);
            yield return www;
            songlist = JSON.Load(www.text).Make<APISongList>();
            if(SongDownloaderUI.songItemPanel != null)
            {
                SongDownloaderUI.AddSongItems(SongDownloaderUI.songItemMenu, songlist);
            }
        }

        IEnumerator GetAllSongs()
        {
            string url = "http://www.audica.wiki:5000/api/customsongs?pagesize=all";
            WWW www = new WWW(url);
            yield return www;
            songlist = JSON.Load(www.text).Make<APISongList>();
            MelonLogger.Log("Found " + songlist.song_count.ToString() + " songs!");
        }

        public static IEnumerator DownloadSong(string downloadUrl)
        {
            string[] splitURL = downloadUrl.Split('/');
            string audicaName = splitURL[splitURL.Length - 1];
            string path = Application.streamingAssetsPath + "\\HmxAudioAssets\\songs\\" + audicaName;
            if (!File.Exists(path))
            {
                WWW www = new WWW(downloadUrl);
                yield return www;
                byte[] results = www.bytes;
                File.WriteAllBytes(path, results);
                needRefresh = true;
            }
            yield return null;
        }

        private static void DebugText(string text)
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
            if (page > songlist.total_pages)
                page = songlist.total_pages;
            else if (page < 1)
                page = 1;
            else
                page--;
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


[Serializable]
public class DownloadHistory
{
    public List<Song> downloadedSongs;
}