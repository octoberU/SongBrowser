
using csvorbis;
using System.Collections.Generic;
using System.IO;
using MelonLoader.TinyJSON;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace AudicaModding
{
    internal static class FilterPanel
    {
        static GameObject glass;
        static GameObject highlights;
        static GameObject filterButton;
        static GameObject favoritesButton;
        
        static GameObject notificationPanel;
        static TextMeshPro notificationText;

        public static GameObject favoritesButtonSelectedIndicator;

        public static Favorites favorites;

        public static bool firstTime = true;

        public static bool filteringFavorites = false;

        static string favoritesPath = Application.dataPath + "/../" + "/UserData/"+ "SongBrowserFavorites.json";

        public static void Initialize()
        {
            if (firstTime)
            {
                GetReferences();
                firstTime = false;
                notificationPanel.transform.localPosition = new Vector3(0f, -15.07f, 0f);
                glass.transform.localScale = new Vector3(10.9f, 16.52f, 3);
                glass.transform.localPosition = new Vector3(0f, -2.27f, 0.15f);
                highlights.transform.localScale = new Vector3(1f, 1.2172f, 1f);
                highlights.transform.localPosition = new Vector3(0f, -12.36f, 0f);
                PrepareFavoritesButton();
                favoritesButtonSelectedIndicator = favoritesButton.transform.GetChild(3).gameObject;
                favoritesButtonSelectedIndicator.SetActive(false);
                filterButton.GetComponentInChildren<GunButton>().onHitEvent.AddListener(new Action(() => { FilterFavorites(); }));
            }
        }

        private static void PrepareFavoritesButton()
        {
            favoritesButton = GameObject.Instantiate(filterButton, filterButton.transform.parent);
            favoritesButton.transform.localPosition = new Vector3(0f, -8.09f, 0f);
            GameObject.Destroy(favoritesButton.GetComponentInChildren<Localizer>());
            GunButton button = favoritesButton.GetComponentInChildren<GunButton>();
            button.onHitEvent = new UnityEvent();
            button.onHitEvent.AddListener(new Action(() => { FilterFavorites(); }));
            favoritesButton.GetComponentInChildren<TextMeshPro>().text = "favorites";
        }

        private static void DisableFavoritesFilter()
        {
            filteringFavorites = false;
            favoritesButtonSelectedIndicator.SetActive(false);
            GameObject.FindObjectOfType<SongSelect>().ShowSongList();
        }

        public static void FilterFavorites()
        {
            GameObject.FindObjectOfType<SongListControls>().FilterExtras(); // this seems to fix duplicated songs;
            if (!filteringFavorites)
            {
                filteringFavorites = true;
                favoritesButtonSelectedIndicator.SetActive(true);
            }
            else
            {
                filteringFavorites = false;
                favoritesButtonSelectedIndicator.SetActive(false);
            }
            GameObject.FindObjectOfType<SongSelect>().ShowSongList();
        }

        public static void GetReferences()
        {
            glass = GameObject.Find("menu/ShellPage_Song/page/ShellPanel_Left/Glass");
            highlights = GameObject.Find("menu/ShellPage_Song/page/ShellPanel_Left/PanelFrame/highlights");
            filterButton = GameObject.Find("menu/ShellPage_Song/page/ShellPanel_Left/FilterExtras");
            notificationPanel = GameObject.Find("menu/ShellPage_Song/page/ShellPanel_Left/ShellPanel_SongListNotification");
            notificationText = notificationPanel.GetComponentInChildren<TextMeshPro>();
        }

        public static void SetNotificationText(string text)
        {
            if (notificationText != null)
            {
                notificationText.text = text; 
            }
        }

        public static void LoadFavorites()
        {
            if (File.Exists(favoritesPath))
            {
                string text = System.IO.File.ReadAllText(favoritesPath);
                favorites = JSON.Load(text).Make<Favorites>();
            }
            else
            {
                favorites = new Favorites();
                favorites.songIDs = new List<string>();
            }
        }

        public static void SaveFavorites()
        {
            string text = JSON.Dump(favorites);
            File.WriteAllText(favoritesPath, text);
        }

        public static void AddFavorite(string songID)
        {
            var song = SongList.I.GetSong(songID);
            if (!song.extrasSong) return;
            if (favorites.songIDs.Contains(songID))
            {
                favorites.songIDs.Remove(songID);
                SongBrowser.DebugText($"Removed {song.title} from favorites!");
                SaveFavorites();
            }
            else
            {
                favorites.songIDs.Add(songID);
                SongBrowser.DebugText($"Added {song.title} to favorites!");
                SaveFavorites();
            }
        }
    }
}

[Serializable]
class Favorites
{
    public List<string> songIDs;
}