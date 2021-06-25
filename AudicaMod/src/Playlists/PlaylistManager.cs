using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static AudicaModding.FilterPanel;

namespace AudicaModding
{
    public static class PlaylistManager
    {
        public static SortedDictionary<string, Playlist> playlists { get; private set; }
        public static Playlist selectedPlaylist { get; private set; }
        public static Playlist playlistToEdit { get; private set; }
        public static Filter playlistFilter { get; set; }
        public static PlaylistState state { get; set; }
        public static List<SongList.SongData> internalSongList { get; private set; }
        public static Song[] apiSongList { get; set; }

        private static PlaylistDownloadManager downloadManager;
        private static PlaylistIOHandler ioHandler;

        public static void OnApplicationStart()
        {
            downloadManager = new PlaylistDownloadManager();
            ioHandler = new PlaylistIOHandler();           

            LoadPlaylists();
        }
        private static void LoadPlaylists()
        {       
           // ioHandler.LoadPlaylistData();
            playlists = ioHandler.LoadPlaylists();
            SavePlaylistData();
            SongLoadingManager.AddPostProcessingCB(downloadManager.DownloadMissingSongs);
            SongLoadingManager.AddPostProcessingCB(downloadManager.EnableBackButton);
        }

        public static void MoveSongUp(string song)
        {
            if (playlistToEdit is null) return;
            playlistToEdit.MoveSongUp(song);
        }

        public static void MoveSongDown(string song)
        {
            if (playlistToEdit is null) return;
            playlistToEdit.MoveSongDown(song);
        }

        public static void OnFilterApplied()
        {
            if(selectedPlaylist is null)
            {
                PlaylistManager.state = PlaylistManager.PlaylistState.Selecting;
                MenuState.I.GoToSettingsPage();
            }
        }

        /*public static void GetAllApiSongs()
        {
            downloadManager.GetAllApiSongs();
        }*/

        public static void SavePlaylist(string playlistName, bool fromEdit)
        {
            if (fromEdit) playlistToEdit = null;
            if (!playlists.ContainsKey(playlistName))
            {
                return;
            }
            ioHandler.SavePlaylist(playlists[playlistName], true);
        }

        public static void PopulatePlaylistsSongNames()
        {
            internalSongList = new List<SongList.SongData>();
            for(int i = 0; i < SongList.I.songs.Count; i++)
            {
                internalSongList.Add(SongList.I.songs[i]);
            }

            foreach(Playlist playlist in playlists.Values)
            {
                playlist.PopulateSongNames();
            }
        }

        public static void DownloadSingleSong(string songName, bool showPopup, GunButton button, TextMeshPro label)
        {
            downloadManager.DownloadSingleSong(songName, showPopup, button, label);
        }

        public static void DownloadSongs(List<string> songs, bool showPopup, GunButton button, TextMeshPro label)
        {
            downloadManager.DownloadSongs(songs, showPopup, button, label);
        }

        public static void SavePlaylistData()
        {
            ioHandler.SavePlaylistData();
        }

        public static void SetPlaylistInitialized(string playlistName)
        {
            ioHandler.UpdatePlaylistData(playlistName);
        }
        
        public static bool IsPlaylistInitialized(string playlistName)
        {
            return ioHandler.IsPlaylistInitialized(playlistName);
        }

        public static void AddNewPlaylist(Playlist playlist, bool created = false)
        {
            playlist.PopulateSongNames();
            playlists.Add(playlist.name, playlist);
            ioHandler.AddNewPlaylistData(playlist.name, created);
        }

        public static void SetPlaylistToEdit(string playlistName)
        {
            if (!playlists.ContainsKey(playlistName))
            {
                MelonLoader.MelonLogger.Msg("Playlist " + playlistName + " couldn't be found.");
                return;
            }
            playlistToEdit = playlists[playlistName];
        }

        public static void SelectPlaylist(string playlistName)
        {
            if (!playlists.ContainsKey(playlistName))
            {
                MelonLogger.Msg("Playlist " + playlistName + " couldn't be found.");
                return;
            }
            selectedPlaylist = playlists[playlistName];
        }
        public static void AddSongToPlaylist(string playlistName, string songName)
        {
            if (!playlists.ContainsKey(playlistName))
            {
                MelonLogger.Msg("Playlist " + playlistName + " couldn't be found.");
                return;
            }
            playlists[playlistName].AddSong(songName);
            SavePlaylist(playlistName, false);
            AddPlaylistButton.songToAdd = "";
        }

        public static void RemoveSongFromPlaylist(string songName)
        {
            if(playlistToEdit is null)
            {
                MelonLoader.MelonLogger.Msg("No playlist to edit selected");
                return;
            }
            playlists[playlistToEdit.name].RemoveSong(songName);
        }

        public static void DeletePlaylist()
        {
            if (playlistToEdit is null)
            {
                MelonLogger.Msg("No playlist to edit selected");
                return;
            }
            PlaylistUtil.Popup(playlistToEdit.name + " deleted");
            ioHandler.DeletePlaylist(playlistToEdit.name);
            playlists.Remove(playlistToEdit.name);
            playlistToEdit = null;
        }
        
        public enum PlaylistState
        {
            None,
            Selecting,
            Creating,
            Adding,
            Editing,
            Endless
        }
    }
}
