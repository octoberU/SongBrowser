using System;
using UnityEngine;
using Harmony;

namespace AudicaModding
{
    internal static class RefreshButton
    {
        private static GameObject refreshButton;

        private static Vector3 randomButtonPos = new Vector3(10.4f, -1.5f, 0.0f);
        private static Vector3 randomButtonRot = new Vector3(0f, 0f, 0f);

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
                                   randomButtonPos, randomButtonRot);
        }
        private static void OnRefreshButtonShot()
        {
            GameObject.FindObjectOfType<MenuState>().GoToMainPage();
            SongBrowser.ReloadSongList();
        }
       

        [HarmonyPatch(typeof(MenuState), "SetState", new Type[] { typeof(MenuState.State) })]
        private static class RefreshPatchMenuState
        {
            private static void Postfix(MenuState __instance, ref MenuState.State state)
            {               
                if (state == MenuState.State.SongPage) CreateRefreshButton();
            }
        }

    }
}
