using System;
using UnityEngine;

namespace AudicaModding
{
    internal static class RefreshButton
    {
        private static GameObject refreshButton;

        private static Vector3 refreshButtonPos = new Vector3(10.4f, -1.5f, 0.0f);
        private static Vector3 refreshButtonRot = new Vector3(0f, 0f, 0f);

        public static void CreateRefreshButton()
        {
            if (refreshButton != null)
            {
                refreshButton.SetActive(true);
                return;
            }
            var backButton = GameObject.Find("menu/ShellPage_Song/page/backParent/back");
            refreshButton  = GameObject.Instantiate(backButton, backButton.transform.parent.transform);
            ButtonUtils.InitButton(refreshButton, "Refresh Songs", new Action(() => { OnRefreshButtonShot(); }), 
                                   refreshButtonPos, refreshButtonRot);
        }
        private static void OnRefreshButtonShot()
        {
            MenuState.I.GoToMainPage();
            SongBrowser.ReloadSongList();
        }
    }
}
