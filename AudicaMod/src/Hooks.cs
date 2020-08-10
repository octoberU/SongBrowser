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
        private static int scrollCounter = 2;

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
                    if (AudicaMod.searchString == "" || AudicaMod.searchString == null)
                        return false;
                    AudicaMod.searchString = AudicaMod.searchString.Substring(0, AudicaMod.searchString.Length - 1);


                    if (SongDownloaderUI.searchText != null)
                    {
                        SongDownloaderUI.searchText.text = AudicaMod.searchString;
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(SongList), "Start", new Type[0])]
        private static class CustomSongFolder
        {
            private static void Postfix(SongList __instance)
            {
                if (!AudicaMod.emptiedDownloadsFolder)
                {
                    AudicaMod.EmptyDownloadsFolderFolder();
                }
                SongList.AddSongSearchDir(Application.dataPath, AudicaMod.customSongDirectory);
                AudicaMod.addedCustomsDir = true;
            }
        }

        [HarmonyPatch(typeof(SongSelect), "GetSongIDs", new Type[] {typeof(bool) })]
        private static class RemoveDeletedScrollerItems
        {
            private static void Postfix(SongSelect __instance, bool extras, ref Il2CppSystem.Collections.Generic.List<string> __result)
            {
                foreach (var deletedSong in AudicaMod.deletedSongs)
                {
                    __result.Remove(deletedSong);
                }
            }
        }

        [HarmonyPatch(typeof(SongSelect), "AddToScroller", new Type[] { typeof(SongSelect.SongSelectItemEntry) })]
        private static class ModifySongSelectEntryName
        {
            private static void Postfix(SongSelect __instance, SongSelect.SongSelectItemEntry entry)
            {
                if (scrollCounter % 2 == 0)
                {
                    var song = SongList.I.GetSong(entry.songID);
                    if (entry.item.mapperLabel != null)
                    {
                        entry.item.mapperLabel.text += AudicaMod.GetDifficultyString(song.hasEasy,
                                        song.hasNormal,
                                        song.hasHard,
                                        song.hasExpert);  
                    }
                }
                scrollCounter++;
            }
        }

        [HarmonyPatch(typeof(MenuState), "SetState", new Type[] { typeof(MenuState.State) })]
        private static class Patch2SetMenuState
        {
            private static void Postfix(MenuState __instance, ref MenuState.State state)
            {
                if (state == MenuState.State.SongPage && !AudicaMod.deleteButtonCreated) AudicaMod.CreateDeleteButton();

                if (AudicaMod.deleteButtonCreated)
                {
                    if (state == MenuState.State.LaunchPage) MelonCoroutines.Start(AudicaMod.SetDeleteButtonActive(true));
                    else if (state != MenuState.State.Launched) MelonCoroutines.Start(AudicaMod.SetDeleteButtonActive(false));
                    else if (state == MenuState.State.Launching) MelonCoroutines.Start(AudicaMod.SetDeleteButtonActive(false, true));
                }
            }
        }

    }
}
