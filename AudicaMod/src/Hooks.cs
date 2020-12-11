using Harmony;
using UnityEngine;
using System.Reflection;
using System;
using MelonLoader;
using Valve.VR.InteractionSystem;
using TMPro;
using System.Linq;
using static DifficultyCalculator;

namespace AudicaModding
{
    internal static class Hooks
    {
        private static int buttonCount = 0;
        private static int scrollCounter = 2;

        /*
        public static void ApplyHooks(HarmonyInstance instance)
        {
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }
        */

        [HarmonyPatch(typeof(OptionsMenu), "AddButton", new Type[] { typeof(int), typeof(string), typeof(OptionsMenuButton.SelectedActionDelegate), typeof(OptionsMenuButton.IsCheckedDelegate), typeof(string), typeof(OptionsMenuButton), })]
        private static class AddButtonButton
        {
            private static void Postfix(OptionsMenu __instance, int col, string label, OptionsMenuButton.SelectedActionDelegate onSelected, OptionsMenuButton.IsCheckedDelegate isChecked)
            {
                if (__instance.mPage == OptionsMenu.Page.Main)
                {
                    buttonCount++;
                    if (buttonCount == 9)
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

        [HarmonyPatch(typeof(KeyboardEntry), "OnKey", new Type[] { typeof(KeyCode), typeof(string) })]
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

        [HarmonyPatch(typeof(SongSelect), "GetSongIDs", new Type[] { typeof(bool) })]
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
                var song = SongList.I.GetSong(entry.songID);
                if (entry.item.mapperLabel != null)
                {
                    //package data to be used for display
                    SongBrowser.SongDisplayPackage songd = new SongBrowser.SongDisplayPackage();

                    songd.hasEasy = song.hasEasy;
                    songd.hasStandard = song.hasNormal;
                    songd.hasAdvanced = song.hasHard;
                    songd.hasExpert = song.hasExpert;

                    //if song data loader is installed look for custom tags
                    if (SongBrowser.songDataLoaderInstalled)
                    {
                        songd = SongBrowser.SongDisplayPackage.FillCustomData(songd, song.songID);
                    }


                    CachedCalculation easy = DifficultyCalculator.GetRating(song.songID, KataConfig.Difficulty.Easy.ToString());
                    CachedCalculation normal = DifficultyCalculator.GetRating(song.songID, KataConfig.Difficulty.Normal.ToString());
                    CachedCalculation hard = DifficultyCalculator.GetRating(song.songID, KataConfig.Difficulty.Hard.ToString());
                    CachedCalculation expert = DifficultyCalculator.GetRating(song.songID, KataConfig.Difficulty.Expert.ToString());

                    //add mine tag if there are mines
                    if (song.hasEasy && easy.hasMines) songd.customEasyTags.Insert(0, "Mines");
                    if (song.hasNormal && normal.hasMines) songd.customStandardTags.Insert(0, "Mines");
                    if (song.hasHard && hard.hasMines) songd.customAdvancedTags.Insert(0, "Mines");
                    if (song.hasExpert && expert.hasMines) songd.customExpertTags.Insert(0, "Mines");

                    //add 360 tag
                    if (song.hasEasy && easy.is360) songd.customEasyTags.Insert(0, "360");
                    if (song.hasNormal && normal.is360) songd.customStandardTags.Insert(0, "360");
                    if (song.hasHard && hard.is360) songd.customAdvancedTags.Insert(0, "360");
                    if (song.hasExpert && expert.is360) songd.customExpertTags.Insert(0, "360");

                    songd.customExpertTags = songd.customExpertTags.Distinct().ToList();
                    songd.customStandardTags = songd.customStandardTags.Distinct().ToList();
                    songd.customAdvancedTags = songd.customAdvancedTags.Distinct().ToList();
                    songd.customEasyTags = songd.customEasyTags.Distinct().ToList();

                    entry.item.mapperLabel.text += SongBrowser.GetDifficultyString(songd);
                }
            }
        }



        [HarmonyPatch(typeof(MenuState), "SetState", new Type[] { typeof(MenuState.State) })]
        private static class Patch2SetMenuState
        {
            private static void Postfix(MenuState __instance, ref MenuState.State state)
            {
                if (state == MenuState.State.LaunchPage)
                {
                    DeleteButton.CreateDeleteButton();
                    FavoriteButtonButton.CreateFavoriteButtonButton();
                    DifficultyDisplay.Show();
                }
                else
                {
                    DifficultyDisplay.Hide();
                }
                if (state == MenuState.State.SongPage)
                {
                    ScoreDisplayList.Show();
                }
                else
                {
                    ScoreDisplayList.Hide();
                }

                if (state == MenuState.State.SongPage) RandomSong.CreateRandomSongButton();

            }
        }

        [HarmonyPatch(typeof(SongSelect), "OnEnable", new Type[0])]
        private static class AdjustSongSelect
        {
            private static void Postfix(SongSelect __instance)
            {
                //FilterPanel.filteringFavorites = false;
                FilterPanel.Initialize();
                ScoreHistory.LoadHistory(PlatformChooser.I.GetLeaderboardID());
                MelonCoroutines.Start(SongBrowser.UpdateLastSongCount());
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
