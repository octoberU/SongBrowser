using Harmony;
using UnityEngine;
using System.Reflection;
using System;
using MelonLoader;

namespace AudicaModding
{
    internal static class Hooks
    {
        private static int buttonCount = 0;

        public static void ApplyHooks(HarmonyInstance instance)
        {
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }

        [HarmonyPatch(typeof(OptionsMenu), "AddButton", new Type[] { typeof(int), typeof(string), typeof(OptionsMenuButton.SelectedActionDelegate), typeof(OptionsMenuButton.IsCheckedDelegate), typeof(string), typeof(OptionsMenuButton), })]
        private static class AddButtonButton
        {
            private static void Postfix(OptionsMenu __instance, int col, string label, OptionsMenuButton.SelectedActionDelegate onSelected, OptionsMenuButton.IsCheckedDelegate isChecked)
            {
                if(__instance.mPage == OptionsMenu.Page.Main)
                {
                    buttonCount++;
                    if (buttonCount == 18)
                    {
                        SongDownloaderUI.AddPageButton(__instance, 0);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(OptionsMenu), "ShowPage", new Type[] { typeof(OptionsMenu.Page) })]
        private static class ResetButtons
        {
            private static void Prefix(OptionsMenu __instance, OptionsMenu.Page page)
            {
                AudicaMod.shouldShowKeyboard = false;
                MelonModLogger.Log(page.ToString());
                buttonCount = 0;
                AudicaMod.searchString = "";

            }
        }

        [HarmonyPatch(typeof(OptionsMenu), "BackOut", new Type[0])]
        private static class Backout
        {
            private static void Prefix(OptionsMenu __instance)
            {
                if (SongDownloaderUI.songItemPanel != null)
                    SongDownloaderUI.songItemPanel.SetPageActive(false);
                if (AudicaMod.needRefresh)
                    AudicaMod.ReloadSongList();
                    
            }
        }

        [HarmonyPatch(typeof(KeyboardEntry), "Hide", new Type[0])]
        private static class KeyboardEntry_Hide
        {
            private static bool Prefix(KeyboardEntry __instance)
            {
                if (AudicaMod.shouldShowKeyboard)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(KeyboardEntry), "OnKey", new Type[] {typeof(KeyCode), typeof(string) })]
        private static class KeyboardEntry_OnKey
        {
            private static bool Prefix(KeyboardEntry __instance, KeyCode keyCode, string label)
            {
                if (AudicaMod.shouldShowKeyboard)
                {
                    switch (label)
                    {
                        case "done":
                            __instance.Hide();
                            AudicaMod.shouldShowKeyboard = false;
                            AudicaMod.page = 1;
                            AudicaMod.StartSongSearch();
                            break;
                        case "clear":
                            AudicaMod.searchString = "";
                            break;
                        default:
                            AudicaMod.searchString += label;
                            break;
                    }
                    
                    if(SongDownloaderUI.searchText != null)
                    {
                        SongDownloaderUI.searchText.text = AudicaMod.searchString;
                        MelonLogger.Log(AudicaMod.searchString);
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(KeyboardEntry), "OnUnderscore", new Type[0])]
        private static class KeyboardEntry_UnderScore
        {
            private static bool Prefix(KeyboardEntry __instance)
            {
                if (AudicaMod.shouldShowKeyboard)
                {
                    AudicaMod.searchString += " ";

                    if (SongDownloaderUI.searchText != null)
                    {
                        SongDownloaderUI.searchText.text = AudicaMod.searchString;
                        MelonLogger.Log(AudicaMod.searchString);
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(KeyboardEntry), "OnBackspace", new Type[0])]
        private static class KeyboardEntry_BackSpace
        {
            private static bool Prefix(KeyboardEntry __instance)
            {
                if (AudicaMod.shouldShowKeyboard)
                {
                    AudicaMod.searchString = AudicaMod.searchString.Substring(0, AudicaMod.searchString.Length - 1);


                    if (SongDownloaderUI.searchText != null)
                    {
                        SongDownloaderUI.searchText.text = AudicaMod.searchString;
                        MelonLogger.Log(AudicaMod.searchString);
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

    }
}
