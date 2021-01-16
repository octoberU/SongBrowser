using System;
using UnityEngine;

namespace AudicaModding
{
    internal static class DeleteButton
    {
        private static GameObject deleteButton;

        private static Vector3 delButtonMenuPosition = new Vector3(-12.28f, -0.68f, -6.38f);
        private static Vector3 delButtonMenuRotation = new Vector3(0f, -51.978f, 0f);

        private static Vector3 delButtonInGameUIPosition = new Vector3(-5f, 13.5f, 0f);
        private static Vector3 delButtonInGameUIRotation = new Vector3(0f, 0f, 0f);

        private static LaunchPanel panel = null;

        public static void CreateDeleteButton(ButtonUtils.ButtonLocation location = ButtonUtils.ButtonLocation.Menu)
        {
            // can only reuse the menu button, InGameUI gets recreated each time
            if (location == ButtonUtils.ButtonLocation.Menu && deleteButton != null)
            {
                deleteButton.SetActive(true);
                return;
            }

            string  name          = "InGameUI/ShellPage_EndGameContinue/page/ShellPanel_Center/exit";
            Vector3 localPosition = delButtonInGameUIPosition;
            Vector3 rotation      = delButtonInGameUIRotation;
            Action  listener      = new Action(() => { OnInGameUIDeleteButtonShot(); });
            if (location == ButtonUtils.ButtonLocation.Failed)
            {
                name          = "InGameUI/ShellPage_Failed/page/ShellPanel_Center/exit";
            }
            else if (location == ButtonUtils.ButtonLocation.Pause)
            {
                name = "InGameUI/ShellPage_Pause/page/ShellPanel_Center/exit";
            }
            else if (location == ButtonUtils.ButtonLocation.Menu)
            {
                name          = "menu/ShellPage_Launch/page/backParent/back";
                listener      = new Action(() => { OnDeleteButtonShot(); });
                localPosition = delButtonMenuPosition;
                rotation      = delButtonMenuRotation;
            }

            var        refButton = GameObject.Find(name);
            GameObject button    = GameObject.Instantiate(refButton, refButton.transform.parent.transform);
            if (location == ButtonUtils.ButtonLocation.Menu)
            {
                deleteButton = button;
            }
            ButtonUtils.InitButton(button, "Delete", listener, localPosition, rotation);
        }

        private static void OnDeleteButtonShot()
        {
            Delete();

            if (panel == null)
            {
                panel = GameObject.FindObjectOfType<LaunchPanel>();
            }
            panel.Back();
        }
        private static void OnInGameUIDeleteButtonShot()
        {
            Delete();
            InGameUI.I.ReturnToSongList();
        }

        private static void Delete()
        {
            var song = SongDataHolder.I.songData;
            SongBrowser.DebugText("Deleted " + song.title);
            SongBrowser.RemoveSong(song.songID);
        }
    }
}
