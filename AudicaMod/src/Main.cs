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
            CleanDeletedSongs();
        }

        public static void EmptyDownloadsFolderFolder()
        {
            String directoryName = downloadsDirectory;
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            var dirInfo = new DirectoryInfo(directoryName);
            List<String> AudicaFiles = Directory
                               .GetFiles(downloadsDirectory, "*.*", SearchOption.TopDirectoryOnly).ToList();
            foreach (string file in AudicaFiles)
            {
                FileInfo audicaFile = new FileInfo(file);
                if (new FileInfo(dirInfo + "\\" + audicaFile.Name).Exists == false)
                {
                    audicaFile.MoveTo(dirInfo + "\\" + audicaFile.Name);
                }
                else
                {
                    File.Delete(file);
                }
            }
            emptiedDownloadsFolder = true;
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
                return;

            var song = SongList.I.GetSong(songID);
            if (File.Exists(song.zipPath))
            {
                deletedSongPaths.Add(song.zipPath);
                deletedSongs.Add(song.songID);
            }
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                ReloadSongList();
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
                yield return new WaitForSeconds(3f);
                File.WriteAllBytes(downloadPath, results);
                yield return new WaitForSeconds(5f);
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
                player.LoadAsync();
                yield return new WaitForSeconds(0.2f);
                player.Play();
                yield return new WaitForSeconds(10f);
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

        //Variables
        #region Delete Button
        static public GameObject deleteButton = null;

        static private Vector3 deleteButtonPos = new Vector3(-24.2f, -9.9f, 17.0f);
        static private Vector3 deleteButtonRot = new Vector3(0, -55f, 0);
        static private Vector3 deleteButtonScale = new Vector3(2, 2, 2);

        static public bool deleteButtonCreated = false;
        #endregion
        //Wait times to make it look a bit better with menu transitions.
        public static IEnumerator SetDeleteButtonActive(bool active, bool immediate = false)
        {
            if (!deleteButtonCreated) yield break;
            if (immediate) yield return null;
            else if (active) yield return new WaitForSeconds(.65f);
            else yield return new WaitForSeconds(.3f);

            deleteButton.gameObject.SetActive(active);
        }
        public static void CreateDeleteButton()
        {
            GameObject menuButton = GameObject.FindObjectOfType<MainMenuPanel>().buttons[1];
            deleteButton = CreateButton(GameObject.FindObjectOfType<MainMenuPanel>().buttons[1], "Delete Song", OnDeleteButtonShot, deleteButtonPos, deleteButtonRot, deleteButtonScale);
            deleteButtonCreated = true;
            SetDeleteButtonActive(false);
        }

        //Add your code to this
        private static void OnDeleteButtonShot()
        {
            var song = SongDataHolder.I.songData;
            DebugText("Deleted " + song.title);
            RemoveSong(song.songID);
            GameObject.FindObjectOfType<LaunchPanel>().Back();
        }
        private static GameObject CreateButton(GameObject buttonPrefab, string label, Action onHit, Vector3 position, Vector3 eulerRotation, Vector3 scale)
        {
            GameObject buttonObject = UnityEngine.Object.Instantiate(buttonPrefab);
            buttonObject.transform.rotation = Quaternion.Euler(eulerRotation);
            buttonObject.transform.position = position;
            buttonObject.transform.localScale = scale;

            UnityEngine.Object.Destroy(buttonObject.GetComponentInChildren<Localizer>());

            TextMeshPro buttonText = buttonObject.GetComponentInChildren<TextMeshPro>();
            buttonText.text = label;

            GunButton button = buttonObject.GetComponentInChildren<GunButton>();
            //don't comment this out, else you'll lose your reference to the button
            button.destroyOnShot = false;
            //comment out from here...
            button.doMeshExplosion = false;
            button.doParticles = false;
            //..to here if you want the explosion effect to play
            button.onHitEvent = new UnityEvent();
            button.onHitEvent.AddListener(onHit);

            return buttonObject.gameObject;
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
