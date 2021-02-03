using MelonLoader;
using UnityEngine;
using TMPro;
using System.Collections;
using Harmony;
using System;
using System.Linq;
using System.Collections.Generic;

namespace AudicaModding
{
    public class CustomDifficultyNames : MelonMod
    {

        private static Dictionary<string, string> difficultyTagToName = new Dictionary<string, string>()
        {
            { "customExpert", "Expert" },
            { "customAdvanced", "Advanced" },
            { "customModerate", "Moderate" },
            { "customBeginner", "Beginner" },
        };

        private static Dictionary<string, string> difficultyNameToTag = new Dictionary<string, string>()
        {
            { "Expert"  ,"customExpert" },
            { "Advanced","customAdvanced" },
            { "Moderate","customModerate" },
            { "Beginner","customBeginner" },
        };

        public static IEnumerator ChangeNamesLP(LaunchPanel __instance)
        {
            yield return new WaitForSeconds(0.05f);

            SongList.SongData song = SongDataHolder.I.songData;

            ChangeSpecificDifficulty(ref song, __instance.expert.GetComponentInChildren<TextMeshPro>(), difficultyNameToTag["Expert"]);
            ChangeSpecificDifficulty(ref song, __instance.hard.GetComponentInChildren<TextMeshPro>(), difficultyNameToTag["Advanced"]);
            ChangeSpecificDifficulty(ref song, __instance.normal.GetComponentInChildren<TextMeshPro>(), difficultyNameToTag["Moderate"]);
            ChangeSpecificDifficulty(ref song, __instance.easy.GetComponentInChildren<TextMeshPro>(), difficultyNameToTag["Beginner"]);
        }

        public static IEnumerator ChangeNamesDS(DifficultySelect __instance)
        {
            yield return new WaitForSeconds(0.05f);

            SongList.SongData song = SongDataHolder.I.songData;

            ChangeSpecificDifficulty(ref song, __instance.expert.label, difficultyNameToTag["Expert"]);
            ChangeSpecificDifficulty(ref song, __instance.hard.label, difficultyNameToTag["Advanced"]);
            ChangeSpecificDifficulty(ref song, __instance.normal.label, difficultyNameToTag["Moderate"]);
            ChangeSpecificDifficulty(ref song, __instance.easy.label, difficultyNameToTag["Beginner"]);
        }

        private static void ChangeSpecificDifficulty(ref SongList.SongData song,TextMeshPro label, string difficultyTag)
        {
            bool hasCustom = SongDataLoader.AllSongData[song.songID].HasCustomData();

            if (hasCustom && SongDataLoader.AllSongData[song.songID].SongHasCustomDataKey(difficultyTag))
            {
                string text = SongDataLoader.AllSongData[song.songID].GetCustomData<string>(difficultyTag);
                if (text.Length > 0)
                {
                    label.SetText(text);
                }
            }
            else
            {
                label.SetText(difficultyTagToName[difficultyTag]);               
            }
        }

        [HarmonyPatch(typeof(LaunchPanel), "OnEnable", new Type[0])]
        private static class DisplayCustomNameLP
        {
            private static void Prefix(LaunchPanel __instance)
            {
                if (SongBrowser.songDataLoaderInstalled)
                {
                    IEnumerator coroutine = ChangeNamesLP(__instance);
                    MelonCoroutines.Start(coroutine);
                }

            }

        }

        [HarmonyPatch(typeof(DifficultySelect), "OnEnable", new Type[0])]
        private static class DisplayCustomNameDS
        {
            private static void Prefix(DifficultySelect __instance)
            {
                if (SongBrowser.songDataLoaderInstalled)
                {
                    IEnumerator coroutine = ChangeNamesDS(__instance);
                    MelonCoroutines.Start(coroutine);
                }

            }

        }
    }
}
