using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AudicaModding
{
    internal static class SongSearch
    {
        public static List<string> searchResult = new List<string>();
        public static string       query;
        public static MapType      mapType = MapType.All;
        public static bool         searchInProgress = false;

        public enum MapType
        {
            All,
            OfficialOnly,
            CustomsOnly
        }

        public static void CancelSearch()
        {
            searchInProgress = false;
            MenuState.I.GoToSongPage();
            SongSearchButton.UpdateSearchButton();
        }

        public static void Search()
        {
            searchResult.Clear();
            searchInProgress = false;

            if (query == null)
                return;

            for (int i = 0; i < SongList.I.songs.Count; i++)
            {
                SongList.SongData currentSong = SongList.I.songs[i];
                bool              isCustom    = IsCustomSong(currentSong.songID);

                if ((mapType == MapType.CustomsOnly  && !isCustom) ||
                    (mapType == MapType.OfficialOnly && isCustom))
                    continue;

                if (currentSong.songID == "tutorial")
                    continue; // never return the tutorial as result

                if (currentSong.artist.ToLowerInvariant().Contains(query.ToLowerInvariant()) ||
                    currentSong.title.ToLowerInvariant().Contains(query.ToLowerInvariant()) ||
                    currentSong.songID.ToLowerInvariant().Contains(query.ToLowerInvariant()) ||
                    currentSong.author != null && currentSong.author.ToLowerInvariant().Contains(query.ToLowerInvariant()) ||
                    currentSong.artist.ToLowerInvariant().Replace(" ", "").Contains(query.ToLowerInvariant()) ||
                    currentSong.title.ToLowerInvariant().Replace(" ", "").Contains(query.ToLowerInvariant()))
                {
                    searchResult.Add(currentSong.songID);
                }
            }
        }

        public static void OnNewUserSearch()
        {
            Search();
            FilterPanel.ResetFilterState();
            MenuState.I.GoToSongPage();
            SongSearchButton.UpdateSearchButton();
        }

        private static bool IsCustomSong(string songID)
        {
            string[] components = songID.Split('_');
            if (components.Length == 1) // only official songs don't have a hash in their ID
                return false;

            string potentialHash = components[components.Length - 1];

            // hash is always 32 characters long and only contains (lowercase) hex characters
            if (potentialHash.Length == 32 && Regex.IsMatch(potentialHash, @"^[0-9a-f]+$"))
                return true;

            return false;
        }
    }
}
