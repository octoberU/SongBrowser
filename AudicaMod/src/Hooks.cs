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
                if(__instance.mPage == OptionsMenu.Page.Main)
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

        [HarmonyPatch(typeof(SongList), "GetSong", new Type[] { typeof(string) })]
        private static class AddToScrollerPatch
        {
            private static void Postfix(SongList __instance, string songID, SongList.SongData __result)
            {
                if (__result.songID == "destiny" ||
                    __result.songID == "adrenaline" ||
                    __result.songID == "collider" ||
                    __result.songID == "golddust" ||
                    __result.songID == "hr8938cephei" ||
                    __result.songID == "ifeellove" ||
                    __result.songID == "iwantu" ||
                    __result.songID == "lazerface" ||
                    __result.songID == "popstars" ||
                    __result.songID == "perfectexceeder" ||
                    __result.songID == "predator" ||
                    __result.songID == "resistance" ||
                    __result.songID == "smoke" ||
                    __result.songID == "splinter" ||
                    __result.songID == "synthesized" ||
                    __result.songID == "thespace" ||
                    __result.songID == "titanium_cazzette" ||
                    __result.songID == "reedsofmitatrush" ||
                    __result.songID == "destiny_full" ||
                    __result.songID == "popstars_full")
                {
                    __result.author = "HMXJeff";
                }
                else if (__result.songID == "addictedtoamemory" ||
                         __result.songID == "breakforme" ||
                         __result.songID == "channel42" ||
                         __result.songID == "everyday" ||
                         __result.songID == "gametime" ||
                         __result.songID == "highwaytooblivion_short" ||
                         __result.songID == "overtime" ||
                         __result.songID == "tothestars" ||
                         __result.songID == "addictedtoamemory_full" ||
                         __result.songID == "highwaytooblivion_full" ||
                         __result.songID == "avalanche" ||
                         __result.songID == "badguy" ||
                         __result.songID == "believer" ||
                         __result.songID == "betternow" ||
                         __result.songID == "cantfeelmyface" ||
                         __result.songID == "centuries" ||
                         __result.songID == "countingstars" ||
                         __result.songID == "dontletmedown" ||
                         __result.songID == "exitwounds" ||
                         __result.songID == "gdfr" ||
                         __result.songID == "girlsbedancing" ||
                         __result.songID == "intoyou" ||
                         __result.songID == "juice" ||
                         __result.songID == "longrun" ||
                         __result.songID == "methanebreather" ||
                         __result.songID == "moveslikejagger" ||
                         __result.songID == "newrules" ||
                         __result.songID == "sorryforpartyrocking" ||
                         __result.songID == "starships" ||
                         __result.songID == "stook" ||
                         __result.songID == "thegreatest" ||
                         __result.songID == "themiddle" ||
                         __result.songID == "themotherweshare" ||
                         __result.songID == "urprey" ||
                         __result.songID == "weallbecome" ||
                         __result.songID == "youngblood")
                {
                    __result.author = "HMXRick";
                }
                else if (__result.songID == "boomboom" ||
                         __result.songID == "raiseyourweapon_noisia" ||
                         __result.songID == "timeforcrime")
                {
                    __result.author = "HMXJeff & HMXRick";
                }
                else if (__result.songID == "eyeforaneye" ||
                         __result.songID == "goatpolyphia" ||
                         __result.songID == "illmerica" ||
                         __result.songID == "funkycomputer")
                {
                    __result.author = "Simon";
                }
                else if (__result.songID == "loyal")
                {
                    __result.author = "Simon & HMXRick";
                }
                else if (__result.songID == "highhopes" ||
                         __result.songID == "goodbyedearsorrows_ab42b2e6b0934471474875729b4f9934" ||
                         __result.songID == "shatterme_30eb4181110577459bc89b8650d3386a")
                {
                    __result.author = "aggrogahu";
                }
                else if (__result.songID == "bigppwoo_0966cf748cb5f637e3b0f00feeda9d9a")
                {
                    __result.author = "Sleepyhead";
                }
                else if (__result.songID == "children-of-a-miracle_bc34b4da4eea98a2a2e7c28d378738e6" ||
                         __result.songID == "get-jinxed_bd9d8a475804d6e2086fc3d2090ea9fb" ||
                         __result.songID == "no-worries_b48e9121c4f412e2da920212ff375a45")
                {
                    __result.author = "Fredrix";
                }
                else if (__result.songID == "LegendsNeverDie_96f3da6e3455fc7b74535f4ad3171955")
                {
                    __result.author = "CircuitLord";
                }
                else if (__result.songID == "Camellia_The_King_of_Lions_f39276e8867fbfd9c0d9c1e99dc03052")
                {
                    __result.author = "CriminalCannoli";
                }
                else if (__result.songID == "weaponizedcelldwellershark_a0508d763c057c198291149233c4d150")
                {
                    __result.author = "whattheshark";
                }
                else if (__result.songID == "ainideaikoiwatsudzukuoctober_d674ca136c43e57ef82a92cb8f80da87" ||
                         __result.songID == "sadmachine_a49b6978f4ab867057f8bb22bcf53580")
                {
                    __result.author = "october";
                }
                else if (__result.songID == "deviltrigger_728ff099c5d7d1f2ad0f724fb53b9b43" ||
                         __result.songID == "onceagain_ProtoPip_0e8bb6d431dd2fdabb62a4d988c263eb")
                {
                    __result.author = "ProtoPip";
                }
            }
        }

    }
}
