using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace AudicaModding
{
    internal static class DeleteButton
    {
        private static GameObject deleteButton;
        public static bool exists => deleteButton != null;
        public static void CreateDeleteButton()
        {
            if (deleteButton != null)
            {
                deleteButton.SetActive(true);
                return;
            }
            var backButton = GameObject.Find("menu/ShellPage_Launch/page/backParent/back");
            deleteButton = GameObject.Instantiate(backButton, backButton.transform.parent.transform);
            GameObject.Destroy(deleteButton.GetComponentInChildren<Localizer>());
            TextMeshPro buttonText = deleteButton.GetComponentInChildren<TextMeshPro>();
            buttonText.text = "Delete";
            GunButton button = deleteButton.GetComponentInChildren<GunButton>();
            button.destroyOnShot = false;
            button.doMeshExplosion = false;
            button.doParticles = false;
            button.onHitEvent = new UnityEvent();
            button.onHitEvent.AddListener(new Action(() => { OnDeleteButtonShot(); }));
            deleteButton.transform.localPosition = new Vector3(-12.28f, -0.68f, -6.38f);
            deleteButton.transform.Rotate(0f, -51.978f, 0f);
        }
        private static void OnDeleteButtonShot()
        {
            var song = SongDataHolder.I.songData;
            SongBrowser.DebugText("Deleted " + song.title);
            SongBrowser.RemoveSong(song.songID);
            GameObject.FindObjectOfType<LaunchPanel>().Back();
        }
    }
}
