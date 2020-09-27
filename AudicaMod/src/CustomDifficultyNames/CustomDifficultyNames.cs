using MelonLoader;
using UnityEngine;
using TMPro;
using System.Collections;
using Harmony;
using System;
using System.Linq;

//[assembly: MelonOptionalDependencies("SongDataLoader")]


namespace AudicaModding
{
    public class CustomDifficultyNames : MelonMod
    {
        public static IEnumerator ChangeNamesLP(LaunchPanel __instance)
        {
            yield return new WaitForSeconds(0.05f);

            var song = SongDataHolder.I.songData;

            if (!SongDataLoader.AllSongData[song.songID].HasCustomData())
                yield break;

            if (SongDataLoader.AllSongData[song.songID].SongHasCustomDataKey("customExpert"))
            {
                __instance.expert.GetComponentInChildren<TextMeshPro>().text = SongDataLoader.AllSongData[song.songID].GetCustomData<string>("customExpert");
            }
            else
            {
                __instance.expert.GetComponentInChildren<TextMeshPro>().text = "Expert";
            }

            if (SongDataLoader.AllSongData[song.songID].SongHasCustomDataKey("customAdvanced"))
            {
                __instance.hard.GetComponentInChildren<TextMeshPro>().text = SongDataLoader.AllSongData[song.songID].GetCustomData<string>("customAdvanced");
            }
            else
            {
                __instance.hard.GetComponentInChildren<TextMeshPro>().text = "Advanced";

            }

            if (SongDataLoader.AllSongData[song.songID].SongHasCustomDataKey("customModerate"))
            {
                __instance.normal.GetComponentInChildren<TextMeshPro>().text = SongDataLoader.AllSongData[song.songID].GetCustomData<string>("customModerate");
            }
            else
            {
                __instance.normal.GetComponentInChildren<TextMeshPro>().text = "Moderate";

            }

            if (SongDataLoader.AllSongData[song.songID].SongHasCustomDataKey("customBeginner"))
            {
                __instance.easy.GetComponentInChildren<TextMeshPro>().text = SongDataLoader.AllSongData[song.songID].GetCustomData<string>("customBeginner");
            }
            else
            {
                __instance.easy.GetComponentInChildren<TextMeshPro>().text = "Beginner";

            }
        }

        public static IEnumerator ChangeNamesDS(DifficultySelect __instance)
        {
            yield return new WaitForSeconds(0.05f);

            var song = SongDataHolder.I.songData;

            if (!SongDataLoader.AllSongData[song.songID].HasCustomData())
                yield break;

            if (SongDataLoader.AllSongData[song.songID].SongHasCustomDataKey("customExpert"))
            {
                __instance.expert.label.SetText(SongDataLoader.AllSongData[song.songID].GetCustomData<string>("customExpert"));
            }
            else
            {
                __instance.expert.label.text = "Expert";
            }

            if (SongDataLoader.AllSongData[song.songID].SongHasCustomDataKey("customAdvanced"))
            {
                __instance.hard.label.SetText(SongDataLoader.AllSongData[song.songID].GetCustomData<string>("customAdvanced"));
            }
            else
            {
                __instance.hard.label.text = "Advanced";

            }

            if (SongDataLoader.AllSongData[song.songID].SongHasCustomDataKey("customModerate"))
            {
                __instance.normal.label.SetText(SongDataLoader.AllSongData[song.songID].GetCustomData<string>("customModerate"));
            }
            else
            {
                __instance.normal.label.text = "Moderate";

            }

            if (SongDataLoader.AllSongData[song.songID].SongHasCustomDataKey("customBeginner"))
            {
                __instance.easy.label.SetText(SongDataLoader.AllSongData[song.songID].GetCustomData<string>("customBeginner"));
            }
            else
            {
                __instance.easy.label.text = "Beginner";
            }



        }

        [HarmonyPatch(typeof(LaunchPanel), "OnEnable", new Type[0])]
        private static class DisplayCustomNameLP
        {
            private static void Prefix(LaunchPanel __instance)
            {

                IEnumerator coroutine = ChangeNamesLP(__instance);
                MelonCoroutines.Start(coroutine);

            }

        }

        [HarmonyPatch(typeof(DifficultySelect), "OnEnable", new Type[0])]
        private static class DisplayCustomNameDS
        {
            private static void Prefix(DifficultySelect __instance)
            {

                IEnumerator coroutine = ChangeNamesDS(__instance);
                MelonCoroutines.Start(coroutine);

            }

        }
    }
}
