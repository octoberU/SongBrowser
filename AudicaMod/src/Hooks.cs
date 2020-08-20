using Harmony;
using UnityEngine;
using System.Reflection;
using System;
using MelonLoader;
using Valve.VR.InteractionSystem;
using TMPro;
using SongBrowser.src.UI.Buttons;

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
                SongBrowser.shouldShowKeyboard = false;
                buttonCount = 0;
                SongBrowser.searchString = "";

            }
        }

        [HarmonyPatch(typeof(OptionsMenu), "BackOut", new Type[0])]
        private static class Backout
        {
            private static void Prefix(OptionsMenu __instance)
            {
                if (SongDownloaderUI.songItemPanel != null)
                    SongDownloaderUI.songItemPanel.SetPageActive(false);
                if (SongBrowser.needRefresh)
                    SongBrowser.ReloadSongList();
                    
            }
        }

        [HarmonyPatch(typeof(KeyboardEntry), "Hide", new Type[0])]
        private static class KeyboardEntry_Hide
        {
            private static bool Prefix(KeyboardEntry __instance)
            {
                if (SongBrowser.shouldShowKeyboard)
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
                if (SongBrowser.shouldShowKeyboard)
                {
                    switch (label)
                    {
                        case "done":
                            __instance.Hide();
                            SongBrowser.shouldShowKeyboard = false;
                            SongBrowser.page = 1;
                            SongBrowser.StartSongSearch();
                            break;
                        case "clear":
                            SongBrowser.searchString = "";
                            break;
                        default:
                            SongBrowser.searchString += label;
                            break;
                    }
                    
                    if(SongDownloaderUI.searchText != null)
                    {
                        SongDownloaderUI.searchText.text = SongBrowser.searchString;
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
                if (SongBrowser.shouldShowKeyboard)
                {
                    SongBrowser.searchString += " ";

                    if (SongDownloaderUI.searchText != null)
                    {
                        SongDownloaderUI.searchText.text = SongBrowser.searchString;
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
                if (SongBrowser.shouldShowKeyboard)
                {
                    if (SongBrowser.searchString == "" || SongBrowser.searchString == null)
                        return false;
                    SongBrowser.searchString = SongBrowser.searchString.Substring(0, SongBrowser.searchString.Length - 1);


                    if (SongDownloaderUI.searchText != null)
                    {
                        SongDownloaderUI.searchText.text = SongBrowser.searchString;
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(SongList), "Awake", new Type[0])]
        private static class CustomSongFolder
        {
            private static void Postfix(SongList __instance)
            {
                if (!SongBrowser.emptiedDownloadsFolder)
                {
                    Utility.EmptyDownloadsFolderFolder();
                }
                //if (!AudicaMod.addedCustomsDir)
                //{
                //    SongList.AddSongSearchDir(Application.dataPath, AudicaMod.customSongDirectory);
                //    AudicaMod.addedCustomsDir = true; 
                //}
            }
        }

        [HarmonyPatch(typeof(SongSelect), "GetSongIDs", new Type[] {typeof(bool) })]
        private static class RemoveDeletedScrollerItems
        {
            private static void Postfix(SongSelect __instance, ref bool extras, ref Il2CppSystem.Collections.Generic.List<string> __result)
            {
                if (FilterPanel.filteringFavorites)
                {
                    extras = true;
                    if (FilterPanel.favorites != null)
                    {
                        __result.Clear();
                        for (int i = 0; i < FilterPanel.favorites.songIDs.Count; i++)
                        {
                            __result.Add(FilterPanel.favorites.songIDs[i]);
                        }
                    }
                    __instance.scroller.SnapTo(0, true);

                }
                if (SongBrowser.deletedSongs.Count > 0)
                {
                    foreach (var deletedSong in SongBrowser.deletedSongs)
                    {
                        __result.Remove(deletedSong);
                    }
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
                        entry.item.mapperLabel.text += SongBrowser.GetDifficultyString(song.hasEasy,
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
                if (state == MenuState.State.SongPage)
                {
                    DeleteButton.CreateDeleteButton();
                    FavoriteButtonButton.CreateFavoriteButtonButton();
                }

            }
        }

        [HarmonyPatch(typeof(SongSelect), "OnEnable", new Type[0])]
        private static class AdjustSongSelect
        {
            private static void Postfix(SongSelect __instance)
            {
                FilterPanel.Initialize();
            }
        }

        [HarmonyPatch(typeof(SongListControls), "FilterAll", new Type[0])]
        private static class FilterAll
        {
            private static void Prefix(SongListControls __instance)
            {
                FilterPanel.filteringFavorites = false;
                FilterPanel.favoritesButtonSelectedIndicator.SetActive(false);
            }
        }

        [HarmonyPatch(typeof(SongListControls), "FilterMain", new Type[0])]
        private static class FilterMain
        {
            private static void Prefix(SongListControls __instance)
            {
                FilterPanel.filteringFavorites = false;
                FilterPanel.favoritesButtonSelectedIndicator.SetActive(false);
            }
        }

        //[HarmonyPatch(typeof(SongListControls), "FilterExtras", new Type[0])]
        //private static class FilterExtras
        //{
        //    private static void Prefix(SongListControls __instance)
        //    {
        //        FilterPanel.filteringFavorites = false;
        //        FilterPanel.favoritesButtonSelectedIndicator.SetActive(false);
        //    }
        //}

        [HarmonyPatch(typeof(LaunchPanel), "Play", new Type[0])]
        private static class ResetFilterPanel
        {
            private static void Prefix(SongListControls __instance)
            {
                FilterPanel.firstTime = true;
            }
        }

    }
}
