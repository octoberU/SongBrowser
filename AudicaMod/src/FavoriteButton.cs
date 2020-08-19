using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace AudicaModding
{
    internal static class FavoriteButtonButton
    {
        private static GameObject favoriteButtonButton;
        public static bool exists => favoriteButtonButton != null;
        public static void CreateFavoriteButtonButton()
        {
            if (favoriteButtonButton != null)
            {
                favoriteButtonButton.SetActive(true);
                return;
            }
            var backButton = GameObject.Find("menu/ShellPage_Launch/page/backParent/back");
            favoriteButtonButton = GameObject.Instantiate(backButton, backButton.transform.parent.transform);
            GameObject.Destroy(favoriteButtonButton.GetComponentInChildren<Localizer>());
            TextMeshPro buttonText = favoriteButtonButton.GetComponentInChildren<TextMeshPro>();
            buttonText.text = "Favorite";
            GunButton button = favoriteButtonButton.GetComponentInChildren<GunButton>();
            button.destroyOnShot = false;
            button.doMeshExplosion = false;
            button.doParticles = false;
            button.onHitEvent = new UnityEvent();
            button.onHitEvent.AddListener(new Action(() => { OnFavoriteButtonButtonShot(); }));
            favoriteButtonButton.transform.localPosition = new Vector3(-9.73f, -0.68f, -3.12f);
            favoriteButtonButton.transform.Rotate(0f, -51.978f, 0f);
        }
        private static void OnFavoriteButtonButtonShot()
        {
            var song = SongDataHolder.I.songData;
            FilterPanel.AddFavorite(song.songID);
            GameObject.FindObjectOfType<LaunchPanel>().Back();
        }
    }
}
