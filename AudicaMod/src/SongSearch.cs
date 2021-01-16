using System.Collections.Generic;

namespace AudicaModding
{
    internal static class SongSearch
    {
        public static List<string> searchResult = new List<string>();
        public static string       query;
        public static bool         searchInProgress = false;

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

            if (string.IsNullOrEmpty(query))
                return;

            for (int i = 0; i < SongList.I.songs.Count - 1; i++)
            {
                SongList.SongData currentSong = SongList.I.songs[i];

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
    }
}
