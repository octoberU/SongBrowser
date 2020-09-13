using Harmony;
using UnityEngine;
using System.Reflection;
using System;
using MelonLoader;
using Valve.VR.InteractionSystem;
using TMPro;
using System.Collections;

namespace AudicaModding
{
    internal static class DifficultyHooks
    {
        public static IEnumerator ChangeNamesLP(LaunchPanel __instance)
        {
            yield return new WaitForSeconds(0.05f);

            var song = SongDataHolder.I.songData;
            CustomDifficultyNames.CustomNameData data;
            if (CustomDifficultyNames.CustomDifficulties.ContainsKey(song.songID))
            {
                data = CustomDifficultyNames.CustomDifficulties[song.songID];

                if (data.hasCustomExpertName)
                    __instance.expert.GetComponentInChildren<TextMeshPro>().text = data.customExpert;

                if (data.hasCustomAdvancedName)
                    __instance.hard.GetComponentInChildren<TextMeshPro>().text = data.customAdvanced;

                if (data.hasCustomModerateName)
                    __instance.normal.GetComponentInChildren<TextMeshPro>().text = data.customModerate;

                if (data.hasCustomBeginnerName)
                    __instance.easy.GetComponentInChildren<TextMeshPro>().text = data.customBeginner;

            }
            else
            {
                __instance.expert.GetComponentInChildren<TextMeshPro>().text = "Expert";
                __instance.hard.GetComponentInChildren<TextMeshPro>().text = "Advanced";
                __instance.normal.GetComponentInChildren<TextMeshPro>().text = "Moderate";
                __instance.easy.GetComponentInChildren<TextMeshPro>().text = "Beginner";
            }
        }

        public static IEnumerator ChangeNamesDS(DifficultySelect __instance)
        {
            yield return new WaitForSeconds(0.05f);

            var song = SongDataHolder.I.songData;
            CustomDifficultyNames.CustomNameData data;
            if (CustomDifficultyNames.CustomDifficulties.ContainsKey(song.songID))
            {
                data = CustomDifficultyNames.CustomDifficulties[song.songID];

                if (data.hasCustomExpertName)
                    __instance.expert.label.text = data.customExpert;

                if (data.hasCustomAdvancedName)
                    __instance.hard.label.text = data.customAdvanced;

                if (data.hasCustomModerateName)
                    __instance.normal.label.text = data.customModerate;

                if (data.hasCustomBeginnerName)
                    __instance.easy.label.text = data.customBeginner;

            }
            else
            {
                __instance.expert.label.text = "Expert";
                __instance.hard.label.text = "Advanced";
                __instance.normal.label.text = "Moderate";
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
            private static void Postfix(DifficultySelect __instance)
            {
                IEnumerator coroutine = ChangeNamesDS(__instance);
                MelonCoroutines.Start(coroutine);
            }

        }

        [HarmonyPatch(typeof(SongSelect), "OnEnable", new Type[0])]
        private static class LoadDiffNamesOnSongSelectLoad
        {
            private static void Postfix(DifficultySelect __instance)
            {
                CustomDifficultyNames.LoadCustomNames();
            }

        }
    }
}
