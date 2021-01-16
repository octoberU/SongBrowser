
using System.Collections.Generic;
using System.IO;
using MelonLoader.TinyJSON;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace AudicaModding
{
    // Public so other mods can add buttons to the panel 
    // by using RegisterFilter() during application startup
    public static class FilterPanel
    {
        internal static GameObject notificationPanel;
        private static TextMeshPro notificationText;

        internal static Favorites favorites;

        internal static bool firstTime = true;

        private static string favoritesPath = Application.dataPath + "/../" + "/UserData/"+ "SongBrowserFavorites.json";

        private static Dictionary<string, Filter> filters;

        private static SongSelect       songSelect       = null;
        private static SongListControls songListControls = null;

        private static bool filterStateChanged = false;

        private static GameObject panel;
        private static GameObject glass;
        private static GameObject highlights;
        private static GameObject filterButton;

        public class Filter
        {
            public string      FilterID                { get; internal set; }
            public bool        IsActive                { get; internal set; }
            public string      DefaultButtonText       { get; internal set; }
            public string      SongListText            { get; set; }
            public GameObject  Button                  { get; internal set; }
            public TextMeshPro ButtonText              { get; internal set; }

            internal GameObject  ButtonSelectedIndicator = null;

            internal Action OnHit     = null;
            internal Action OnDisable = null;
            internal Func<Il2CppSystem.Collections.Generic.List<string>, bool> Apply = null;

            internal Filter() { }
        }

        /// <summary>
        /// Allows adding a filter (including its button) to the filter panel.
        /// </summary>
        /// <param name="defaultButtonText">Text shown on filter button by default, also acts as filterID</param>
        /// <param name="songListText">Text shown as first element in the filtered list (e.g. "Extras")</param>
        /// <param name="onHitListener">Called when the button is shot</param>
        /// <param name="onDisable">Called when the filter is disabled</param>
        /// <param name="applyFilter">Called to apply the filter to the song list. Must return true if filter was applied successfully, 
        ///     false otherwise.</param>
        /// <returns>Function that can be used to access the filter - null if filter with same ID already exists</returns>
        public static Func<Filter> RegisterFilter(string defaultButtonText, string songListText, Action onHitListener,
                                                  Action onDisable,
                                                  Func<Il2CppSystem.Collections.Generic.List<string>, bool> applyFilter)
        {
            if (filters == null)
            {
                filters = new Dictionary<string, Filter>();
            }

            if (filters.ContainsKey(defaultButtonText))
                return null;

            Filter filter            = new Filter();
            filter.FilterID          = defaultButtonText;
            filter.IsActive          = false;
            filter.SongListText      = songListText;
            filter.DefaultButtonText = defaultButtonText;
            filter.OnHit             = onHitListener;
            filter.OnDisable         = onDisable;
            filter.Apply             = applyFilter;

            filters.Add(defaultButtonText, filter);

            return new Func<Filter>(() => { return filter; });
        }

        /// <summary>
        /// Returns true if filter with given ID is currently active (i.e. in use), false otherwise
        /// </summary>
        public static bool IsFiltering(string filterID)
        {
            return filters.ContainsKey(filterID) ? filters[filterID].IsActive : false;
        }

        internal static void OnApplicationStart()
        {
            LoadFavorites();

            RegisterFilter("search", "Search Results", SongSearchButton.ShowSearchButton, SongSearchButton.HideSearchButton, 
                (result) =>
                {
                    if (SongSearch.searchResult != null)
                    {
                        result.Clear();
                        for (int i = 0; i < SongSearch.searchResult.Count; i++)
                        {
                            result.Add(SongSearch.searchResult[i]);
                        }
                        return true;
                    }
                    return false;
                });

            RegisterFilter("favorites", "Favorites", () => {}, () => {}, 
                (result) =>
                {
                    if (favorites != null)
                    {
                        result.Clear();
                        string id;
                        for (int i = 0; i < favorites.songIDs.Count; i++)
                        {
                            id = favorites.songIDs[i];
                            if (SongBrowser.songIDs.Contains(id))
                                result.Add(id);
                        }
                        return true;
                    }
                    return false;
                });
        }

        internal static void Initialize()
        {
            if (firstTime)
            {
                songSelect       = GameObject.FindObjectOfType<SongSelect>();
                songListControls = GameObject.FindObjectOfType<SongListControls>();
                GetReferences();
                firstTime = false;

                foreach (string filterKey in filters.Keys)
                {
                    PrepareFilterButton(filters[filterKey]);
                }

                SetFilterUIGeometry();

                filterButton.GetComponentInChildren<GunButton>().onHitEvent.AddListener(new Action(() => 
                { 
                    DisableCustomFilters();
                    songSelect.ShowSongList();
                }));
            }
        }

        internal static void ResetFilterState()
        {
            filterStateChanged = true;
        }

        internal static void ApplyFilter(SongSelect __instance, ref bool extras, ref Il2CppSystem.Collections.Generic.List<string> __result)
        {
            foreach (Filter filter in filters.Values)
            {
                if (filter.IsActive)
                {
                    extras = true;
                    if (filter.Apply(__result))
                    {
                        __instance.songSelectHeaderItems.mItems[0].titleLabel.text = filter.SongListText;
                    }
                    UpdateScrollPosition(__instance.scroller);
                    break; // only can have one active at a time
                }
            }
        }

        internal static void DisableCustomFilters(string exceptFilterId = "")
        {
            filterStateChanged = true;
            foreach (Filter filter in filters.Values)
            {
                if (filter.FilterID != exceptFilterId)
                {
                    filter.IsActive = false;
                    filter.ButtonSelectedIndicator.SetActive(false);
                    filter.OnDisable();
                }
            }
        }

        internal static void SetNotificationText(string text)
        {
            if (notificationText != null)
            {
                notificationText.text = text;
            }
        }

        private static void OnFilterHit(string filterId)
        {
            songListControls.FilterExtras(); // this seems to fix duplicated songs
            Filter filter = filters[filterId];
            if (!filter.IsActive)
            {
                filter.IsActive    = true;
                filterStateChanged = true;
                filter.ButtonSelectedIndicator.SetActive(true);
                filter.OnHit();

                DisableCustomFilters(filterId);
            }
            else
            {
                DisableCustomFilters();
            }
            songSelect.ShowSongList();
        }

        private static void UpdateScrollPosition(ShellScrollable scroller)
        {
            // only reset scroller if the user switched filters
            // not e.g. when re-entering the song list from a song
            if (filterStateChanged)
            {
                scroller.SnapTo(0, true);
                filterStateChanged = false;
            }
        }

        private static void PrepareFilterButton(Filter filter)
        {
            filter.Button = GameObject.Instantiate(filterButton, filterButton.transform.parent);
            GameObject.Destroy(filter.Button.GetComponentInChildren<Localizer>());
            GunButton button = filter.Button.GetComponentInChildren<GunButton>();
            button.onHitEvent = new UnityEvent();
            button.onHitEvent.AddListener(new Action(() => { OnFilterHit(filter.FilterID); }));
            filter.ButtonText = filter.Button.GetComponentInChildren<TextMeshPro>();
            filter.ButtonText.text = filter.DefaultButtonText;
            filter.ButtonSelectedIndicator = filter.Button.transform.GetChild(3).gameObject;
            filter.ButtonSelectedIndicator.SetActive(filter.IsActive);
        }

        private static void SetFilterUIGeometry()
        {
            var filterKeys = filters.Keys;
            int filterCount = filterKeys.Count;

            // following is default (only search and favorites as custom filters)
            Vector3 panelLocalPositionOffset  = new Vector3( 0.0f,   1.5f,  0f);
            Vector3 notificationLocalPosition = new Vector3( 0.0f, -17.85f,  0f);
            Vector3 glassLocalScale           = new Vector3(11.0f,  20.0f,  3f);
            Vector3 glassLocalPosition        = new Vector3( 0.0f,  -3.75f, 0.15f);
            Vector3 highlightsLocalScale      = new Vector3( 1.0f,   1.4f,  1f);
            Vector3 highlightsLocalPosition   = new Vector3( 0.0f, -14.0f,  0f);

            if (filterCount > 2) // there are additional filters registered by other mods
            {
                // make room for additional button(s)
                for (int i = 0; i < filterCount - 2; i++)
                {
                    panelLocalPositionOffset  += new Vector3(0f,  1.5f,   0f);
                    notificationLocalPosition += new Vector3(0f, -3.5f,   0f);
                    glassLocalScale           += new Vector3(0f,  3.15f,  0f);
                    glassLocalPosition        += new Vector3(0f, -1.875f, 0f);
                    highlightsLocalScale      += new Vector3(0f,  0.25f,  0f);
                    highlightsLocalPosition   += new Vector3(0f, -1.85f,  0f);
                }
            }

            // apply geometry changes
            panel.transform.localPosition             += panelLocalPositionOffset;
            notificationPanel.transform.localPosition = notificationLocalPosition;
            glass.transform.localScale                = glassLocalScale;
            glass.transform.localPosition             = glassLocalPosition;
            highlights.transform.localScale           = highlightsLocalScale;
            highlights.transform.localPosition        = highlightsLocalPosition;

            Vector3 localPos = new Vector3(0f, -8f, 0f); // local position (relative to extras button) of first custom filter button
            foreach (string key in filterKeys)
            {
                filters[key].Button.transform.localPosition = localPos;
                localPos += new Vector3(0f, -3.5f, 0f);
            }
        }

        private static void GetReferences()
        {
            panel             = GameObject.Find("menu/ShellPage_Song/page/ShellPanel_Left");
            glass             = GameObject.Find("menu/ShellPage_Song/page/ShellPanel_Left/Glass");
            highlights        = GameObject.Find("menu/ShellPage_Song/page/ShellPanel_Left/PanelFrame/highlights");
            filterButton      = GameObject.Find("menu/ShellPage_Song/page/ShellPanel_Left/FilterExtras");
            notificationPanel = GameObject.Find("menu/ShellPage_Song/page/ShellPanel_Left/ShellPanel_SongListNotification");
            notificationText  = notificationPanel.GetComponentInChildren<TextMeshPro>();
        }

        #region Favorites
        public static void SaveFavorites()
        {
            string text = JSON.Dump(favorites);
            File.WriteAllText(favoritesPath, text);
        }

        public static bool IsFavorite(string songID)
        {
            return favorites.songIDs.Contains(songID);
        }

        public static void AddFavorite(string songID)
        {
            var song = SongList.I.GetSong(songID);
            if (!song.extrasSong) return;
            if (favorites.songIDs.Contains(songID))
            {
                RandomSong.FavouritesChanged(songID, false);
                favorites.songIDs.Remove(songID);
                SongBrowser.DebugText($"Removed {song.title} from favorites!");
                SaveFavorites();
            }
            else
            {
                RandomSong.FavouritesChanged(songID, true);
                favorites.songIDs.Add(songID);
                SongBrowser.DebugText($"Added {song.title} to favorites!");
                SaveFavorites();
            }
        }

        private static void LoadFavorites()
        {
            if (File.Exists(favoritesPath))
            {
                string text = File.ReadAllText(favoritesPath);
                favorites = JSON.Load(text).Make<Favorites>();
            }
            else
            {
                favorites = new Favorites();
                favorites.songIDs = new List<string>();
            }
        }
        #endregion
    }
}

[Serializable]
class Favorites
{
    public List<string> songIDs;
}