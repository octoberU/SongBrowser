using System;
using Il2CppSystem.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Harmony;
using MelonLoader;

namespace AudicaModding
{
    internal static class RefreshButton
    {

        private static GameObject refreshButton;
        public static bool exists => refreshButton != null;

        private static Vector3 randomButtonPos = new Vector3(10.2f, -15.4f, 24.2f);
        private static Vector3 randomButtonRot = new Vector3(0f, 0f, 0f);
        private static SongSelect songSelect = null;


        public static void CreateRefreshButton()
        {
            if (refreshButton != null)
            {
                refreshButton.SetActive(true);
                return;
            }
            var backButton = GameObject.Find("menu/ShellPage_Song/page/backParent/back");
            refreshButton = GameObject.Instantiate(backButton, backButton.transform.parent.transform);
            GameObject.Destroy(refreshButton.GetComponentInChildren<Localizer>());
            TextMeshPro buttonText = refreshButton.GetComponentInChildren<TextMeshPro>();
            buttonText.text = "Refresh Songs";
            GunButton button = refreshButton.GetComponentInChildren<GunButton>();
            button.destroyOnShot = true;
            button.doMeshExplosion = true;
            button.doParticles = true;
            button.onHitEvent = new UnityEvent();
            button.onHitEvent.AddListener(new Action(() => { OnRefreshButtonShot(); }));
            refreshButton.transform.position = randomButtonPos;
            refreshButton.transform.rotation = Quaternion.Euler(randomButtonRot);
        }
        private static void OnRefreshButtonShot()
        {
            SongBrowser.ReloadSongList();
            refreshButton = null;
            CreateRefreshButton();
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
