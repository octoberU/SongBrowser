using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using Newtonsoft.Json;

namespace AudicaModding
{
    [Serializable]
    public class Playlist
    {
        public string name { get; set; }
        public List<string> songs { get; set; }
        [JsonIgnore]
        public Dictionary<string, string> songNames { get; set; }

        public Playlist(string name, List<string> songs)
        {
            this.name = name;
            this.songs = songs;
            songNames = new Dictionary<string, string>();
        }

        public void AddSong(string song)
        {
            if(songs.Contains(song))
            {
                PlaylistUtil.Popup($"{song} already exists in playlist");
                return;
            }
            songs.Add(song);
            songNames.Add(song, GetSongName(song));
        }

        public void RemoveSong(string song)
        {
            if (!songs.Contains(song))
            {
                MelonLogger.Log($"{song} doesn't exist in playlist");
                return;
            }
            songs.Remove(song);
            songNames.Remove(song);
        }

        public void MoveSongUp(string song)
        {
            if (!songs.Contains(song)) return;
            int i = songs.IndexOf(song);
            songs.RemoveAt(i);
            songs.Insert(i - 1, song);
            PopulateSongNames();
        }

        public void MoveSongDown(string song)
        {
            if (!songs.Contains(song)) return;
            int i = songs.IndexOf(song);
            songs.RemoveAt(i);
            songs.Insert(i + 1, song);
            PopulateSongNames();
        }

        public void PopulateSongNames()
        {
            songNames = new Dictionary<string, string>();
            if (songs is null) return;
            foreach(string song in songs)
            {
                songNames.Add(song, GetSongName(song));
            }
        }

        private string GetSongName(string song)
        {
            string fileName = song + ".audica";
            if (PlaylistManager.apiSongList.Any(s => s.filename == fileName))
            {
                Song _song = PlaylistManager.apiSongList.First(s => s.filename == fileName);
                return _song.title + " - " + _song.author;
            }
            else if (PlaylistManager.internalSongList.Any(s => Path.GetFileName(s.zipPath) == fileName))
            {
                SongList.SongData _song = PlaylistManager.internalSongList.First(s => Path.GetFileName(s.zipPath) == fileName);
                return _song.title + " - " + _song.author;
            }
            else return song;
            
        }
    }
}
