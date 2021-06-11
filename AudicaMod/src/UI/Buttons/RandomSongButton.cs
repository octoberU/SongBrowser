using System;
using UnityEngine;

namespace AudicaModding
{
    internal static class RandomSongButton
    {
        private static GameObject randomSongButton;

        private static Vector3 randomSongButtonPos = new Vector3(10.4f, 0f, 0f);
        private static Vector3 randomSongButtonRot = new Vector3(0f, 0f, 0f);

        public static void CreateRandomSongButton()
        {
            if (randomSongButton != null)
            {
                randomSongButton.SetActive(true);
                return;
            }

            var backButton = GameObject.Find("menu/ShellPage_Song/page/backParent/back");
            if (backButton == null)
                return;

            randomSongButton = GameObject.Instantiate(backButton, backButton.transform.parent.transform);
            ButtonUtils.InitButton(randomSongButton, "Random Song", new Action(() => { OnRandomSongButtonShot(); }),
                                   randomSongButtonPos, randomSongButtonRot);
        }
        private static void OnRandomSongButtonShot()
        {
            RandomSong.SelectRandomSong();
        }
    }
}
