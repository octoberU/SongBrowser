using System;
using UnityEngine;

namespace AudicaModding
{
    internal static class SongSearchButton
    {
        private static GameObject searchButton;

        private static Vector3 searchButtonPos = new Vector3(0f, 15.1f, 0.0f);
        private static Vector3 searchButtonRot = new Vector3(0f, 0f, 0f);

        private static bool isActive = false;

        public static void CreateSearchButton()
        {
            if (searchButton != null)
                return;

            var backButton = GameObject.Find("menu/ShellPage_Song/page/backParent/back");
            if (backButton == null)
                return;

            searchButton = GameObject.Instantiate(backButton, backButton.transform.parent.transform);
            ButtonUtils.InitButton(searchButton, "Search", new Action(() => { OnSearchButtonShot(); }),
                                   searchButtonPos, searchButtonRot);
            searchButton.SetActive(isActive);
        }
        private static void OnSearchButtonShot()
        {
            SongSearch.searchInProgress = true;
            MenuState.I.GoToSettingsPage();
            // moves to search page next via Hooks.PatchShowOptionsPage.Postfix()
        }

        public static void UpdateSearchButton()
        {
            searchButton?.SetActive(isActive);
        }

        public static void ShowSearchButton()
        {
            isActive = true;
            searchButton?.SetActive(true);
        }
        public static void HideSearchButton()
        {
            isActive = false;
            searchButton?.SetActive(false);
        }
    }
}
