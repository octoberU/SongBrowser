using System;
using Il2CppSystem.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Harmony;
using MelonLoader;

namespace AudicaModding
{
    internal static class RandomSong
    {

        private static GameObject randomSongButton;
        public static bool exists => randomSongButton != null;

        private static Vector3 randomButtonPos = new Vector3(10.2f, -9.4f, 24.2f);
        private static Vector3 randomButtonRot = new Vector3(0f, 0f, 0f);

        public static int randomSongBagSize = 10;
        private static int mainSongCount = 33;
        private static bool availableSongListsSetup = false;
        private static List<int> availableMainSongs = new List<int>();
        private static List<int> availableExtrasSongs = new List<int>();
        private static System.Collections.Generic.List<string> availableFavouritesSongs = new System.Collections.Generic.List<string>();
        private static System.Collections.Generic.List<string> lastPickedFavouritesSongs = new System.Collections.Generic.List<string>();
        private static List<int> lastPickedSongs = new List<int>();
        private static List<int> availableSongs = new List<int>();

        private static SongSelect songSelect = null;

        private static Il2CppSystem.Collections.Generic.List<SongSelectItem> songs = new Il2CppSystem.Collections.Generic.List<SongSelectItem>();
      

        public static void LoadBagSize(int size)
        {
            if (size > mainSongCount) randomSongBagSize = mainSongCount;
            else randomSongBagSize = size;
        }

        public static void CreateRandomSongButton()
        {
            if (randomSongButton != null)
            {
                randomSongButton.SetActive(true);
                return;
            }
            var backButton = GameObject.Find("menu/ShellPage_Song/page/backParent/back");
            randomSongButton = GameObject.Instantiate(backButton, backButton.transform.parent.transform);
            GameObject.Destroy(randomSongButton.GetComponentInChildren<Localizer>());
            TextMeshPro buttonText = randomSongButton.GetComponentInChildren<TextMeshPro>();
            buttonText.text = "Random Song";
            GunButton button = randomSongButton.GetComponentInChildren<GunButton>();
            button.destroyOnShot = false;
            button.doMeshExplosion = false;
            button.doParticles = false;
            button.onHitEvent = new UnityEvent();
            button.onHitEvent.AddListener(new Action(() => { OnRandomSongButtonShot(); }));
            randomSongButton.transform.position = randomButtonPos;
            randomSongButton.transform.rotation = Quaternion.Euler(randomButtonRot);
        }
        private static void OnRandomSongButtonShot()
        {
            songSelect = GameObject.FindObjectOfType<SongSelect>();
            songs = songSelect.songSelectItems.mItems;
            int maxLength = songs.Count;
            if (!availableSongListsSetup)
            {
                availableSongListsSetup = true;

                for (int i = 0; i < mainSongCount; i++)
                {
                    availableMainSongs.Add(i);
                }

                for (int i = mainSongCount; i < maxLength; i++)
                {
                    availableExtrasSongs.Add(i);
                }

                for (int i = 0; i < maxLength; i++)
                {
                    availableSongs.Add(i);
                }

                foreach(string song in FilterPanel.favorites.songIDs)
                {
                    availableFavouritesSongs.Add(song);
                }
                
            }

            SongSelect.Filter filter = songSelect.GetListFilter();

            var rand = new System.Random();
            int index;
            if (FilterPanel.filteringFavorites && availableFavouritesSongs.Count > 0)
            {
                string id = availableFavouritesSongs[rand.Next(0, availableFavouritesSongs.Count)];  
                for(int i = 0; i < songs.Count; i++)
                {
                    if(id == songs[i].mSongData.songID)
                    {
                        lastPickedFavouritesSongs.Add(id);
                        availableFavouritesSongs.Remove(id);
                        songs[i].OnSelect();
                        break;
                    }
                }

                if (availableFavouritesSongs.Count < 1)
                {
                    foreach (string s in lastPickedFavouritesSongs) availableFavouritesSongs.Add(s);
                    lastPickedFavouritesSongs = new System.Collections.Generic.List<string>();
                }
                return;               
            }
            else if (filter == SongSelect.Filter.All)
            {
                index = availableSongs[rand.Next(0, availableSongs.Count)];
            }
            else if (filter == SongSelect.Filter.Main)
            {
                index = availableMainSongs[rand.Next(0, availableMainSongs.Count)];
                if (availableMainSongs.Count > 0) availableMainSongs.Remove(index);
            }
            else
            {
                if(availableExtrasSongs.Count < 1)
                {
                    foreach (int i in lastPickedSongs)
                    {
                        availableSongs.Add(i);
                        if (i < 33) availableMainSongs.Add(i);
                        else availableExtrasSongs.Add(i);
                    }
                    lastPickedSongs.Clear();
                }

                index = availableExtrasSongs[rand.Next(0, availableExtrasSongs.Count)];
                availableExtrasSongs.Remove(index);                                 
            }

            songs[index].OnSelect();
            lastPickedSongs.Add(index);
            if (availableSongs.Count > 0) availableSongs.Remove(index);


            if (lastPickedSongs.Count > randomSongBagSize)
            {
                int oldestIndex = lastPickedSongs[0];
                lastPickedSongs.Remove(oldestIndex);
                availableSongs.Add(oldestIndex);
                if (oldestIndex < 33) availableMainSongs.Add(oldestIndex); //was index before but why did i do that?
                else availableExtrasSongs.Add(oldestIndex); //was index before but why did i do that?
            }
        }
        public static void FavouritesChanged(string id, bool add)
        {
            if (add)
            {
                availableFavouritesSongs.Add(id);
            }
            else
            {
                if (lastPickedFavouritesSongs.Contains(id)) lastPickedFavouritesSongs.Remove(id);                 
                else if (availableFavouritesSongs.Contains(id)) availableFavouritesSongs.Remove(id);
            }
        }

        [HarmonyPatch(typeof(MenuState), "SetState", new Type[] { typeof(MenuState.State) })]
        private static class RandomSongPatchMenuState
        {
            private static void Postfix(MenuState __instance, ref MenuState.State state)
            {               
                if (state == MenuState.State.SongPage) CreateRandomSongButton();
            }
        }

    }
}
