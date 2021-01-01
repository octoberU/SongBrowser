using System;
using System.Collections.Generic;
using UnityEngine;

namespace AudicaModding
{
    class SongSearch
    {
        public static List<string> searchResult = new List<string>();
        public static string query;
        public static bool   searchInProgress = false;

        private static GameObject searchButton;

        private static Vector3 searchButtonPos = new Vector3(0f, 15.1f, 0.0f);
        private static Vector3 searchButtonRot = new Vector3(0f, 0f, 0f);

        public static void CreateSearchButton()
        {

            if (searchButton != null)
            {
                searchButton.SetActive(true);
                return;
            }
            var backButton = GameObject.Find("menu/ShellPage_Song/page/backParent/back");
            searchButton = GameObject.Instantiate(backButton, backButton.transform.parent.transform);
            ButtonUtils.InitButton(searchButton, "Search", new Action(() => { OnSearchButtonShot(); }),
                                   searchButtonPos, searchButtonRot);
        }
        private static void OnSearchButtonShot()
        {
            searchInProgress = true;
            MenuState.I.GoToSettingsPage();
            // moves to search page next via Hooks.PatchShowOptionsPage.Postfix()
        }

        public static void CancelSearch()
        {
            searchInProgress = false;
            MenuState.I.GoToSongPage();
        }

        public static void Search()
        {
            searchResult.Clear();
            searchInProgress = false;

            for (int i = 0; i < SongList.I.songs.Count - 1; i++)
            {
                SongList.SongData currentSong = SongList.I.songs[i];

                if (currentSong.artist.ToLowerInvariant().Contains(query.ToLowerInvariant()) ||
                    currentSong.title.ToLowerInvariant().Contains(query.ToLowerInvariant()) ||
                    currentSong.songID.ToLowerInvariant().Contains(query.ToLowerInvariant()) ||
                    currentSong.artist.ToLowerInvariant().Replace(" ", "").Contains(query.ToLowerInvariant()) ||
                    currentSong.title.ToLowerInvariant().Replace(" ", "").Contains(query.ToLowerInvariant()))
                {
                    searchResult.Add(currentSong.songID);
                }
            }
        }
    }
}
