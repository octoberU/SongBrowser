using System;
using UnityEngine;
using MelonLoader;
using System.Collections;

namespace AudicaModding
{
    internal static class PlaylistEndlessButton
    {
        private static GameObject playlistButton;

        private static Vector3 playlistButtonPos = new Vector3(10f, 16.6f, 0.0f);
        private static Vector3 playlistButtonRot = new Vector3(0f, 0f, 0f);

        private static bool isActive = false;

        public static void CreatePlaylistButton()
        {
            if (playlistButton != null)
            {
                ShowPlaylistButton();
                return;
            }
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
            ButtonUtils.InitButton(playlistButton, "Playlist Marathon", new Action(() => { OnPlaylistButtonShot(); }),
                                   playlistButtonPos, playlistButtonRot);
            isActive = true;
            playlistButton.SetActive(isActive);
            //ShowPlaylistButton();
        }
        private static void OnPlaylistButtonShot()
        {
            PlaylistManager.state = PlaylistManager.PlaylistState.Endless;
            MenuState.I.GoToSettingsPage();
            // moves to search page next via Hooks.PatchShowOptionsPage.Postfix()
        }

        public static void UpdatePlaylistButton()
        {
            playlistButton?.SetActive(isActive);
        }

        public static void ShowPlaylistButton()
        {
            if (playlistButton is null) return;
            isActive = true;
            playlistButton?.SetActive(true);
        }
        public static void HidePlaylistButton()
        {
            if (playlistButton is null) return;
            isActive = false;
            playlistButton?.SetActive(false);
        }
    }
}
