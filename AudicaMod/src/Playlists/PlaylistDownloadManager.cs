using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UnityEngine;

namespace AudicaModding
{
    internal class PlaylistDownloadManager
    {
        public Song[] songList { get; private set; }

        private int activeDownloads = 0;
        private bool missingSongsDownloaded = false;
        private bool apiSongsLoaded = false;
        private bool missingSongsFound = false;
        private List<string> missingSongs = new List<string>();
        private bool fullReload = false;
        private GunButton backButton;
        private bool playlistsPopulated = false;

        public IEnumerator DownloadMissingSongs()
        {
            if (missingSongsDownloaded) yield break;
            missingSongsDownloaded = true;
            yield return new WaitForSecondsRealtime(.5f);
            foreach (string song in missingSongs)
            {
                Song match = songList.First(s => s.filename == song);
                activeDownloads++;
                MelonCoroutines.Start(SongDownloader.DownloadSong(match.song_id, match.download_url, OnDownloadComplete));
                yield return null;
            }
            fullReload = true;
        }

        public void DownloadSong(string songName, GunButton button = null)
        {
            backButton = button;
            Song match = songList.DefaultIfEmpty(null).FirstOrDefault(s => s.filename == songName + ".audica");
            if(match is null)
            {
                MelonLogger.Log("Couldn't find " + songName);
                return;
            }
            MelonCoroutines.Start(SongDownloader.DownloadSong(match.song_id, match.download_url, OnDownloadComplete));
        }

        public void GetAllApiSongs()
        {
            if (apiSongsLoaded) return;
            apiSongsLoaded = true;
            MelonCoroutines.Start(SongDownloader.DoSongWebSearch("", OnWebSearchDone, DifficultyFilter.All, false, false, 1, true));
        }

        public void FindMissingSongs()
        {
            if (missingSongsFound) return;
            missingSongsFound = true;

            foreach (Playlist playlist in PlaylistManager.playlists.Values)
            {
                if (PlaylistManager.IsPlaylistInitialized(playlist.name)) continue;
                PlaylistManager.SetPlaylistInitialized(playlist.name);
                foreach (string song in playlist.songs)
                {
                    if (!SongLoadingManager.songDictionary.ContainsKey(song + ".audica"))
                    {
                        missingSongs.Add(song + ".audica");
                    }
                }
            }
        }

        public void OnWebSearchDone(string search, APISongList result)
        {
            PlaylistManager.apiSongList = result.songs;
            songList = result.songs;
            if (!playlistsPopulated)
            {
                playlistsPopulated = true;
                PlaylistManager.PopulatePlaylistsSongNames();
            }
        }

        private void OnDownloadComplete(string search, bool success)
        {
            activeDownloads -= 1;
            if (!success) MelonLogger.LogWarning("Download of " + search + " failed");
            if (activeDownloads > 0) return;
            if (fullReload)
            {
                SongBrowser.ReloadSongList();
                fullReload = false;
            }
            PlaylistManager.SavePlaylistData();
            if(backButton != null)
            {
                backButton.SetInteractable(true);
                backButton = null;
            }           
        }
    }
}
