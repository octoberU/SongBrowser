using System;
using UnityEngine;
using MelonLoader;
using System.Collections;

namespace AudicaModding
{
    internal static class SelectPlaylistButton
    {
        private static GameObject playlistButton;

        private static Vector3 playlistButtonPos = new Vector3(0f, 15.1f, 0.0f);
        private static Vector3 playlistButtonRot = new Vector3(0f, 0f, 0f);

        private static bool isActive = false;

        public static void CreatePlaylistButton()
        {
            if (playlistButton != null)
                return;
            MelonCoroutines.Start(CreateButton());
            
        }

        private static IEnumerator CreateButton()
        {
            while (EnvironmentLoader.I.IsSwitching())
            {
                yield return new WaitForSecondsRealtime(.5f);
            }
            var backButton = GameObject.Find("menu/ShellPage_Song/page/backParent/back");
            if (backButton == null) yield break;
            playlistButton = GameObject.Instantiate(backButton, backButton.transform.parent.transform);
            ButtonUtils.InitButton(playlistButton, "Select Playlist", new Action(() => { OnPlaylistButtonShot(); }),
                                   playlistButtonPos, playlistButtonRot);
            playlistButton.SetActive(isActive);
        }
        private static void OnPlaylistButtonShot()
        {
            PlaylistManager.state = PlaylistManager.PlaylistState.Selecting;
            MenuState.I.GoToSettingsPage();
            // moves to search page next via Hooks.PatchShowOptionsPage.Postfix()
        }

        public static void UpdatePlaylistButton()
        {
            playlistButton?.SetActive(isActive);
        }

        public static void ShowPlaylistButton()
        {
            isActive = true;
            playlistButton?.SetActive(true);
        }
        public static void HidePlaylistButton()
        {
            isActive = false;
            playlistButton?.SetActive(false);
        }
    }
}
