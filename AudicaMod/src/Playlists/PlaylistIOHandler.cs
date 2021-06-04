using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace AudicaModding
{
    internal class PlaylistIOHandler
    {
        public string playlistDirectory = Application.dataPath.Replace("Audica_Data", "Playlists");
        public string playlistDataFile;
        public List<PlaylistData> playlistData;
        /// <summary>
        /// Returns a list of all decoded playlist files in Playlist folder
        /// </summary>
        /// <returns></returns>
        /// 
        public PlaylistIOHandler()
        {
            if (!Directory.Exists(playlistDirectory))
            {
                Directory.CreateDirectory(playlistDirectory);
            }
        }
        public SortedDictionary<string, Playlist> LoadPlaylists()
        {
            LoadPlaylistData();

            SortedDictionary<string, Playlist> playlists = new SortedDictionary<string, Playlist>();
            string[] files = Directory.GetFiles(playlistDirectory);
            for(int i = 0; i < files.Length; i++)
            {
                if (Path.GetExtension(files[i]) != ".playlist") continue;
                KeyValuePair<string, Playlist> entry = DecodePlaylist(files[i]);
                CheckDuplicate(ref entry, ref playlists);
                playlists.Add(entry.Key, entry.Value);
                AddNewPlaylistData(entry.Value.name);
            }
            return playlists;
        }

        private void CheckDuplicate(ref KeyValuePair<string, Playlist> entry, ref SortedDictionary<string, Playlist> playlists)
        {
            int i = 2;
            if (playlists.ContainsKey(entry.Key))
            {
                entry.Value.name += $" [{i}]";
                entry = new KeyValuePair<string, Playlist>(entry.Value.name, entry.Value);
            }
            while (playlists.ContainsKey(entry.Key))
            {
                i++;
                entry.Value.name = entry.Value.name.Substring(0, entry.Value.name.Length - 2);
                entry.Value.name += $"{i}]";
                entry = new KeyValuePair<string, Playlist>(entry.Value.name, entry.Value);
            }
        }

        private void LoadPlaylistData()
        {
            playlistDataFile = Path.Combine(playlistDirectory, "playlistData.dat");
            if (!File.Exists(playlistDataFile))
            {
                var stream = new FileStream(playlistDataFile, FileMode.Create);
                stream.Dispose();
                playlistData = new List<PlaylistData>();
                return;
            }

            using (StreamReader reader = new StreamReader(playlistDataFile))
            {
                string json = reader.ReadToEnd();
                playlistData = JsonConvert.DeserializeObject<List<PlaylistData>>(json);
            }

            
        }

        public void UpdatePlaylistData(string playlistName)
        {
            PlaylistData playlist = playlistData.First(p => p.playlistName == playlistName);
            playlist.initialized = true;
        }

        public void AddNewPlaylistData(string playlistName, bool created = false)
        {
            if (!playlistData.Any(p => p.playlistName == playlistName))
            {
                playlistData.Add(new PlaylistData(playlistName, created));
            }
            
        }

        public void SavePlaylistData()
        {    
            string json = JsonConvert.SerializeObject(playlistData, Formatting.Indented);
            File.WriteAllText(playlistDataFile, json);
        }
        /// <summary>
        /// Decodes individual playlist json files
        /// </summary>
        /// <param name="playlistJson"></param>
        /// <returns></returns>
        private KeyValuePair<string, Playlist> DecodePlaylist(string playlistJson)
        {
            using (StreamReader reader = new StreamReader(playlistJson))
            {
                string json = reader.ReadToEnd();
                Playlist playlist = JsonConvert.DeserializeObject<Playlist>(json);
                return new KeyValuePair<string, Playlist>(playlist.name, playlist);
            }          
        }

        /// <summary>
        /// Saves playlist to Playlist folder
        /// </summary>
        /// <param name="playlist"></param>
        public void SavePlaylist(Playlist playlist, bool update)
        {
            //first check if a playlist with that name alredy exists
            string fileName = GetPlaylistPath(playlist);
            if (!update)
            {
                if (File.Exists(fileName))
                {
                    //if it does, keep incrementing i by 1 until a free name is found. Rename the playlist accordingly
                    int i = 1;
                    playlist.name += i.ToString();
                    fileName = GetPlaylistPath(playlist);
                    while (File.Exists(fileName))
                    {
                        playlist.name = playlist.name.Substring(0, playlist.name.Length - i.ToString().Length);
                        playlist.name += i.ToString();
                        fileName = GetPlaylistPath(playlist);
                    }
                }
            }
            //serialize the playlist to a new file          
            string json = JsonConvert.SerializeObject(playlist, Formatting.Indented);
            File.WriteAllText(fileName, json);
        }

        public void DeletePlaylist(string name)
        {
            string fileName = GetPlaylistPath(name);
            PlaylistData data = playlistData.First(p => p.playlistName == name);
            playlistData.Remove(data);
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            SavePlaylistData();
        }

        public bool IsPlaylistInitialized(string playlistName)
        {
            return playlistData.FirstOrDefault(p => p.playlistName == playlistName).initialized;
        }

        private string GetPlaylistPath(Playlist playlist)
        {
            return GetPlaylistPath(playlist.name);
        }

        private string GetPlaylistPath(string name)
        {
            return Path.Combine(playlistDirectory, name + ".playlist");
        }

        [Serializable]
        internal class PlaylistData
        {
            public string playlistName;
            public bool initialized = false;

            public PlaylistData(string playlistName, bool initialized)
            {
                this.playlistName = playlistName;
                this.initialized = initialized;
            }
        }
    }
}
