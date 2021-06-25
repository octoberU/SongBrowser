using System;
using System.Collections;
using System.Net;
using MelonLoader;
using UnityEngine;
using MelonLoader.TinyJSON;
using System.IO;
using System.Media;
using OggDecoder;
using System.Collections.Generic;

namespace AudicaModding
{
    public static class SongDownloader
    {
        internal static bool UseNewAPI = true;

        internal static string          apiURL              = "http://www.audica.wiki:5000/api/customsongs?pagesize=14";
        internal static string          newApiUrl           = "https://beta.maudica.com/api/maps?per_page=14";
        internal static string          downloadUrlFormat   = "https://beta.maudica.com/maps/{0}/download";
        internal static string          previewUrlFormat    = "https://beta.maudica.com/maps/{0}/preview";
        internal static APISongList     songlist;           
        internal static string          searchString        = "";
        internal static bool            needRefresh         = false;
        internal static int             page                = 1;
        internal static HashSet<string> downloadedFileNames = new HashSet<string>();
        internal static HashSet<string> failedDownloads     = new HashSet<string>();

        private static SoundPlayer player      = new SoundPlayer();
        private static string      lastPreview = "";

        /// <summary>
        /// Coroutine that searches for songs using the web API
        /// </summary>
        /// <param name="search">Query text, e.g. song name, artist or mapper (partial matches possible)</param>
        /// <param name="onSearchComplete">Called with search result once search is completed, use to process search result</param>
        /// <param name="difficulty">Only find songs with given difficulty</param>
        /// <param name="curated">Only find songs that are curated</param>
        /// <param name="sortByPlayCount">Sort result by play count</param>
        /// <param name="page">Page to return (see APISongList.total_pages after initial search (page = 1) to check if multiple pages exist)</param>
        /// <param name="total">Bypasses all query and filter limitations and just returns entire song list (or max page size)</param>
        public static IEnumerator DoSongWebSearch(string search, Action<string, APISongList> onSearchComplete, DifficultyFilter difficulty, bool curated = false, 
                                                  bool sortByPlayCount = false, int page = 1, bool total = false, bool searchIsFilename = false)
        {
            if (UseNewAPI)
            {
                if (total)
                {
                    string webSearch = "https://beta.maudica.com/api/maps?per_page=100&page={0}";

                    // initial search to initialize result list
                    WWW www = new WWW(string.Format(webSearch, 1));
                    yield return www;
                    NewAPISongList list = JSON.Load(www.text).Make<NewAPISongList>();

                    APISongList result = new APISongList();
                    result.song_count  = list.count;
                    result.page        = 1;
                    result.pagesize    = list.count;
                    result.total_pages = 1;
                    result.songs       = new Song[list.count];

                    int numPages = (int)Math.Ceiling((double)list.count / 100);
                    int currPage = 1;
                    ConvertAPIList(list, result, 0);
                    while (currPage <= numPages)
                    {
                        currPage++;
                        www = new WWW(string.Format(webSearch, currPage));
                        yield return www;
                        list = JSON.Load(www.text).Make<NewAPISongList>();
                        ConvertAPIList(list, result, 100 * (currPage - 1));
                    }
                    onSearchComplete(search, result);
                }
                else
                {
                    string webSearch;
                    string concatURL;
                    if (searchIsFilename)
                    {
                        webSearch = search == null || search == "" ? "" : "&filename=" + WebUtility.UrlEncode(search);
                        concatURL = newApiUrl + webSearch;
                    }
                    else
                    {
                        webSearch     = search == null || search == "" ? "" : "&search=" + WebUtility.UrlEncode(search);
                        string webPage       = page == 1 ? "" : "&page=" + page.ToString();
                        string webDifficulty = difficulty == DifficultyFilter.All ? "" : "&difficulties%5B%5D=" + DifficultyToNewAPIValue(difficulty);
                        string webDownloads  = sortByPlayCount ? "&sort=downloads" : "";
                        concatURL = newApiUrl + webSearch + webDifficulty + webPage + webDownloads;
                    }
                    WWW www = new WWW(concatURL);
                    yield return www;
                    NewAPISongList list = JSON.Load(www.text).Make<NewAPISongList>();

                    // convert to existing SongList format, then return
                    APISongList result = new APISongList();
                    result.song_count = list.count;
                    result.page = page;
                    result.pagesize = 14;
                    result.total_pages = (int)Math.Ceiling(result.song_count / (double)result.pagesize);
                    result.songs = new Song[list.maps.Length];
                    ConvertAPIList(list, result, 0);
                    onSearchComplete(search, result);
                }
            }
            else
            {
                string webSearch = search == null || search == "" ? "" : "&search=" + WebUtility.UrlEncode(search);
                string webPage = page == 1 ? "" : "&page=" + page.ToString();
                string diff = difficulty.ToString();
                string webDifficulty = diff == "All" ? "" : "&" + diff.ToLower() + "=true";
                string webCurated = curated ? "&curated=true" : "";
                string webPlaycount = sortByPlayCount ? "&sort=leaderboards" : "";
                string concatURL = !total ? apiURL + webSearch + webDifficulty + webPage + webCurated + webPlaycount : "http://www.audica.wiki:5000/api/customsongs?pagesize=all";
                WWW www = new WWW(concatURL);
                yield return www;
                onSearchComplete(search, JSON.Load(www.text).Make<APISongList>());
            }
        }

        /// <summary>
        /// Coroutine that downloads a song from given download URL. Caller is responsible to call
        /// SongBrowser.ReloadSongList() once download is done
        /// </summary>
        /// <param name="songID">SongID of download target, typically Song.song_id</param>
        /// <param name="downloadUrl">Download target, typically Song.download_url</param>
        /// <param name="onDownloadComplete">Called when download has been written to disk.
        ///     First argument is the songID of the downloaded song.
        ///     Second argument is true if the download succeeded, false otherwise.</param>
        public static IEnumerator DownloadSong(string songID, string downloadUrl, Action<string, bool> onDownloadComplete = null)
        {
            string   audicaName   = songID + ".audica";
            string   path         = Path.Combine(SongBrowser.mainSongDirectory, audicaName);
            string   downloadPath = Path.Combine(SongBrowser.downloadsDirectory, audicaName);
            if (!File.Exists(path) && !File.Exists(downloadPath))
            {
                WWW www = new WWW(downloadUrl);
                yield return www;
                byte[] results = www.bytes;
                File.WriteAllBytes(downloadPath, results);
            }
            yield return null;

			SongList.SongSourceDir dir     = new SongList.SongSourceDir(Application.dataPath, SongBrowser.downloadsDirectory);
			string                 file    = downloadPath.Replace('\\', '/');
			bool                   success = SongList.I.ProcessSingleSong(dir, file, new Il2CppSystem.Collections.Generic.HashSet<string>());
            downloadedFileNames.Add(audicaName);

            if (success)
            {
                needRefresh = true;
            }
            else
            {
                failedDownloads.Add(audicaName);
                if (File.Exists(downloadPath))
                    File.Delete(downloadPath);
            }

            onDownloadComplete?.Invoke(songID, success);
        }

        /// <summary>
        /// Coroutine that plays song preview for given preview URL.
        /// If called with the url of a preview that's already playing
        /// the preview will be stopped instead.
        /// </summary>
        /// <param name="url">URL to preview, typically Song.preview_url</param>
        public static IEnumerator StreamPreviewSong(string url)
        {
            if (lastPreview == url) // let people stop previews since they're very loud
            {
                lastPreview = "";
                player.Stop();
            }
            else
            {
                lastPreview = url;
                WWW www = new WWW(url);
                yield return www;
                if (www.isDone)
                {
                    Stream stream = new MemoryStream(www.bytes);
                    player.Stream = new OggDecodeStream(stream);
                    yield return new WaitForSeconds(0.2f);
                    player.Play();
                    yield return new WaitForSeconds(15f);
                }
            }

            yield return null;
        }

        internal static void StartNewSongSearch()
        {
            page = 1;
            StartNewPageSearch();
        }

        internal static void StartNewPageSearch()
        {
            SongDownloaderUI.ResetScrollPosition();
            MelonCoroutines.Start(DoSongWebSearch(searchString, (query, result) => {
                songlist = result;
                if (SongDownloaderUI.songItemPanel != null)
                {
                    SongDownloaderUI.AddSongItems(SongDownloaderUI.songItemMenu, songlist);
                }
            }, SongDownloaderUI.difficultyFilter, 
               SongDownloaderUI.curated, SongDownloaderUI.popularity, page, false));
        }

        internal static void NextPage()
        {
            if (page > songlist.total_pages)
                page = songlist.total_pages;
            else if (page < 1)
                page = 1;
            else
                page++;
        }
        internal static void PreviousPage()
        {
            if (page == 1) return;
            if (page > songlist.total_pages)
                page = songlist.total_pages;
            else if (page < 1)
                page = 1;
            else
                page--;
        }

        internal static string DifficultyToNewAPIValue(DifficultyFilter diff)
        {
            switch (diff)
            {
                case DifficultyFilter.Beginner: return "beginner";
                case DifficultyFilter.Standard: return "moderate";
                case DifficultyFilter.Advanced: return "advanced";
                case DifficultyFilter.Expert:   return "expert";
                default: return "";
            }
        }

        internal static void ConvertAPIList(NewAPISongList from, APISongList to, int startIdx)
        {
            for (int idx = 0; idx < from.maps.Length; idx++)
            {
                Song   newSong = new Song();
                SongV2 song    = from.maps[idx];
                newSong.title  = song.title;
                newSong.artist = song.artist;
                newSong.author = song.author;
                for (int diffIdx = 0; diffIdx < song.difficulties.Length; diffIdx++)
                {
                    if (song.difficulties[diffIdx] == "beginner")
                    {
                        newSong.beginner = true;
                    }
                    else if (song.difficulties[diffIdx] == "moderate")
                    {
                        newSong.standard = true;
                    }
                    else if (song.difficulties[diffIdx] == "advanced")
                    {
                        newSong.advanced = true;
                    }
                    else if (song.difficulties[diffIdx] == "expert")
                    {
                        newSong.expert = true;
                    }
                }
                newSong.download_url = string.Format(downloadUrlFormat, song.id);
                newSong.upload_time  = song.created_at;
                newSong.update_time  = song.updated_at;
                newSong.video_url    = song.embed_url;
                newSong.filename     = song.filename;
                newSong.song_id      = song.filename.Remove(song.filename.Length - 7); // remove the .audica from the filename to get the hash-less ID
                newSong.preview_url  = string.Format(previewUrlFormat, song.id);

                to.songs[startIdx + idx] = newSong;
            }
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
internal class NewAPISongList
{
    public SongV2[] maps     = null;
    public bool     has_more = false;
    public int      count    = 0;
}

[Serializable]
internal class SongV2
{
    public int      id           = 0;
    public string   created_at   = null;
    public string   updated_at   = null;
    public string   title        = null;
    public string   artist       = null;
    public string   author       = null;
    public string[] difficulties = null;
    public string   description  = null;
    public string   embed_url    = null;
    public string   filename     = null;
}