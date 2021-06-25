using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace AudicaModding
{
    internal static class RandomSong
    {
        public static void SelectRandomSong()
        {
            SongSelect select = GameObject.FindObjectOfType<SongSelect>();
            if (select == null)
            {
                return;
            }
            List<SongSelectItem> songs = select.GetSongButtons();
            if (songs.Count == 0)
            {
                return;
            }

            int               songCount = songs.Count;
            System.Random     rand      = new System.Random();
            int               idx       = rand.Next(0, songCount);
            SongList.SongData data      = songs[idx].mSongData;
            if (data != null)
            {
                SongDataHolder.I.songData = data;
                MenuState.I.GoToLaunchPage();
            }
        }
    }
}
