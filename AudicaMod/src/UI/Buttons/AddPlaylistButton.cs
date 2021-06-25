using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace AudicaModding
{
    internal static class AddPlaylistButton
    {
        private static GameObject playlistButton;

        private static Vector3 playlistButtonMenuPosition = new Vector3(-9.73f, 9.5f, -3.12f); //-0.68
        private static Vector3 playlistButtonMenuRotation = new Vector3(0f, -51.978f, 0f);

        private static Vector3 playlistButtonInGameUIPosition = new Vector3(5f, 17f, 0f); //13.5
        private static Vector3 playlistButtonInGameUIRotation = new Vector3(0f, 0f, 0f);

        public static string songToAdd;
        public static void CreatePlaylistButton(ButtonUtils.ButtonLocation location = ButtonUtils.ButtonLocation.Menu)
        {
            if (PlaylistManager.state == PlaylistManager.PlaylistState.Endless) return;
            // can only reuse the menu button, InGameUI gets recreated each time
            if (location == ButtonUtils.ButtonLocation.Menu && playlistButton != null)
            {
                UpdateLabel();
                playlistButton.SetActive(true);
                return;
            }

            string name = "InGameUI/ShellPage_EndGameContinue/page/ShellPanel_Center/exit";
            Vector3 localPosition = playlistButtonInGameUIPosition;
            Vector3 rotation = playlistButtonInGameUIRotation;
            Action listener = new Action(() => { OnIngamePlaylistButtonShot(); });
            if (location == ButtonUtils.ButtonLocation.Failed)
            {
                name = "InGameUI/ShellPage_Failed/page/ShellPanel_Center/exit";
            }
            else if (location == ButtonUtils.ButtonLocation.Pause)
            {
                name = "InGameUI/ShellPage_Pause/page/ShellPanel_Center/exit";
            }
            else if (location == ButtonUtils.ButtonLocation.PracticeModeOver)
            {
                name = "InGameUI/ShellPage_PracticeModeOver/page/ShellPanel_Center/exit";
            }
            else if (location == ButtonUtils.ButtonLocation.Menu)
            {
                name = "menu/ShellPage_Launch/page/backParent/back";
                listener = new Action(() => { OnPlaylistButtonShot(); });
                localPosition = playlistButtonMenuPosition;
                rotation = playlistButtonMenuRotation;
            }

            var refButton = GameObject.Find(name);
            GameObject button = GameObject.Instantiate(refButton, refButton.transform.parent.transform);
            if (location == ButtonUtils.ButtonLocation.Menu)
            {
                playlistButton = button;
            }
            ButtonUtils.InitButton(button, "Add to Playlist", listener, localPosition, rotation);
        }

        private static void OnPlaylistButtonShot()
        {
            SelectPlaylist();
        }
        private static void OnIngamePlaylistButtonShot()
        {
            MelonLoader.MelonCoroutines.Start(GoToSelectPlaylist());
            //InGameUI.I.ReturnToSongList();
            //SelectPlaylist();
        }

        private static IEnumerator GoToSelectPlaylist()
        {
            InGameUI.I.ReturnToSongList();
            yield return new WaitForSecondsRealtime(.5f);
            SelectPlaylist();
        }

        private static void SelectPlaylist()
        {
            songToAdd = Path.GetFileName(SongDataHolder.I.songData.zipPath);
            songToAdd = songToAdd.Substring(0, songToAdd.Length - 7);
            PlaylistManager.state = PlaylistManager.PlaylistState.Adding;
            MenuState.I.GoToSettingsPage();
        }

        private static void UpdateLabel()
        {
            ButtonUtils.UpdateButtonLabel(playlistButton, "Add to Playlist");
        }
    }
}
