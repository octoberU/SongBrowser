using System;
using UnityEngine;

namespace AudicaModding
{
    internal static class DeleteButton
    {
        private static GameObject deleteButton;

        private static Vector3 delButtonMenuPosition = new Vector3(-12.28f, -0.68f, -6.38f);
        private static Vector3 delButtonMenuRotation = new Vector3(0f, -51.978f, 0f);

        private static Vector3 delButtonFailedPosition   = new Vector3(-5f, -15.5f, 0f);
        private static Vector3 delButtonResultsPosition  = new Vector3(-295f, -183f, 0f);
        private static Vector3 delButtonInGameUIRotation = new Vector3(0f, 0f, 0f);

        public static void CreateDeleteButton(ButtonUtils.ButtonLocation location = ButtonUtils.ButtonLocation.Menu)
        {
            // can only reuse the menu button, InGameUI gets recreated each time
            if (location == ButtonUtils.ButtonLocation.Menu && deleteButton != null)
            {
                deleteButton.SetActive(true);
                return;
            }

            string  name          = "InGameUI/ShellPage_Results/page/ShellPanel_Center/continue";
            Vector3 localPosition = delButtonResultsPosition;
            Vector3 rotation      = delButtonInGameUIRotation;
            Action  listener      = new Action(() => { OnInGameUIDeleteButtonShot(); });
            if (location == ButtonUtils.ButtonLocation.Failed)
            {
                name          = "InGameUI/ShellPage_Failed/page/ShellPanel_Center/exit";
                localPosition = delButtonFailedPosition;
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
            GameObject.FindObjectOfType<LaunchPanel>().Back();
        }
        private static void OnInGameUIDeleteButtonShot()
        {
            Delete();
            GameObject.FindObjectOfType<InGameUI>().ReturnToSongList();
        }

        private static void Delete()
        {
            var song = SongDataHolder.I.songData;
            SongBrowser.DebugText("Deleted " + song.title);
            SongBrowser.RemoveSong(song.songID);
        }
    }
}
