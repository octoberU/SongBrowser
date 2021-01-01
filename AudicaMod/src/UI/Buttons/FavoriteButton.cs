using System;
using UnityEngine;

namespace AudicaModding
{
    internal static class FavoriteButton
    {
        private static GameObject favoriteButton;

        private static Vector3 favButtonMenuPosition = new Vector3(-9.73f, -0.68f, -3.12f);
        private static Vector3 favButtonMenuRotation = new Vector3(0f, -51.978f, 0f);

        private static Vector3 favButtonInGameUIPosition = new Vector3(5f, -15.5f, 0f);
        private static Vector3 favButtonInGameUIRotation = new Vector3(0f, 0f, 0f);

        private static LaunchPanel panel = null;

        public static void CreateFavoriteButton(ButtonUtils.ButtonLocation location = ButtonUtils.ButtonLocation.Menu)
        {
            // can only reuse the menu button, InGameUI gets recreated each time
            if (location == ButtonUtils.ButtonLocation.Menu && favoriteButton != null)
            {
                UpdateLabel();
                favoriteButton.SetActive(true);
                return;
            }

            string  name          = "InGameUI/ShellPage_EndGameContinue/page/ShellPanel_Center/exit";
            Vector3 localPosition = favButtonInGameUIPosition;
            Vector3 rotation      = favButtonInGameUIRotation;
            Action  listener      = new Action(() => { OnInGameUIFavoriteButtonShot(); });
            if (location == ButtonUtils.ButtonLocation.Failed)
            {
                name = "InGameUI/ShellPage_Failed/page/ShellPanel_Center/exit";
            }
            else if (location == ButtonUtils.ButtonLocation.Pause)
            {
                name = "InGameUI/ShellPage_Pause/page/ShellPanel_Center/exit";
            }
            else if (location == ButtonUtils.ButtonLocation.Menu)
            {
                name          = "menu/ShellPage_Launch/page/backParent/back";
                listener      = new Action(() => { OnFavoriteButtonShot(); });
                localPosition = favButtonMenuPosition;
                rotation      = favButtonMenuRotation;
            }

            var        refButton = GameObject.Find(name);
            GameObject button    = GameObject.Instantiate(refButton, refButton.transform.parent.transform);
            if (location == ButtonUtils.ButtonLocation.Menu)
            {
                favoriteButton = button;
            }
            ButtonUtils.InitButton(button, FindLabel(), listener, localPosition, rotation);
        }

        private static void OnFavoriteButtonShot()
        {
            Favorite();
            if (panel == null)
            {
                panel = GameObject.FindObjectOfType<LaunchPanel>();
            }
            panel.Back();
        }
        private static void OnInGameUIFavoriteButtonShot()
        {
            Favorite();
            InGameUI.I.ReturnToSongList();
        }

        private static void Favorite()
        {
            var song = SongDataHolder.I.songData;
            FilterPanel.AddFavorite(song.songID);
        }

        private static void UpdateLabel()
        {
            ButtonUtils.UpdateButtonLabel(favoriteButton, FindLabel());
        }

        private static string FindLabel()
        {
            if (FilterPanel.IsFavorite(SongDataHolder.I.songData.songID))
                return "Unfavorite";
            else
                return "Favorite";
        }
    }
}
