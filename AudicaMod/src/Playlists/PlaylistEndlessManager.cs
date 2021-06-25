using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using AuthorableModifiers;
namespace AudicaModding
{
    public static class PlaylistEndlessManager
    {
        private static Il2CppSystem.Collections.Generic.List<SongSelectItem> songs;
        private static int index = 0;
        private static float volumeFadeTime = 3f;
        private static float originalVolume = PlayerPreferences.I.MusicLevel.mVal;
        private static bool fadeInProgress = false;
        private static bool previousNoFail = false;
        private static bool pendingReset = false;
        private static bool EndlessActive => PlaylistManager.state == PlaylistManager.PlaylistState.Endless;

        public static void StartEndlessSession()
        {
            MelonCoroutines.Start(IStartEndlessSession());
        }

        private static IEnumerator IStartEndlessSession()
        {
            MenuState.I.GoToSongPage();
            ResetIndex();
            SongSelect select = null;
            while(select is null)
            {
                select = GameObject.FindObjectOfType<SongSelect>();
                yield return new WaitForSecondsRealtime(0.2f);
            }
            if (!CanPlay(select))
            {
                PlaylistManager.state = PlaylistManager.PlaylistState.None;
                yield break;
            }
               
            previousNoFail = PlayerPreferences.I.NoFail.mVal;
            PlayerPreferences.I.NoFail.mVal = Config.NoFail;
            pendingReset = true;
            if (Config.Shuffle) songs.Shuffle();

            SetNextSong();
            MelonCoroutines.Start(ILaunch());
            yield return null;
        }

        public static void NextSong()
        {
            MelonCoroutines.Start(INextSong());
        }

        private static IEnumerator INextSong()
        {
            while (fadeInProgress)
            {
                yield return new WaitForSecondsRealtime(.2f);
            }
            yield return new WaitForSecondsRealtime(2f);
            float previousSongHealth = ScoreKeeper.I.GetHealth();
            AudioDriver.I.Pause();
            SetNextSong();
            InGameUI.I.Restart();
            if (SongBrowser.authorableInstalled)
            {
                modifiersLoaded = false;
                LoadModifiers(true);
                while (!modifiersLoaded)
                {
                    yield return new WaitForSecondsRealtime(.2f);
                }
            }
            yield return new WaitForSeconds(2f);
            UpdateVolume(originalVolume);
            if (!Config.ResetHealth) ScoreKeeper.I.mHealth = previousSongHealth;
        }

        public static void FadeOut()
        {
            MelonCoroutines.Start(DoFadeOut());
        }

        private static IEnumerator DoFadeOut()
        {
            originalVolume = PlayerPreferences.I.MusicLevel.mVal;
            float currentVol = originalVolume;
            float time = 0f;
            fadeInProgress = true;
            while (currentVol > -5f && time < 1f)
            {
                time = Mathf.MoveTowards(time, 1, Time.unscaledDeltaTime / volumeFadeTime);
                currentVol = Mathf.Lerp(originalVolume, -5f, time);
                UpdateVolume(currentVol);
                yield return null;
            }
            fadeInProgress = false;
            UpdateVolume(-5f);
            yield return null;
        }

        private static void UpdateVolume(float amount)
        {
            
            PlayerPreferences.I.MusicLevel.mVal = amount;
            PlayerPreferences.I.UpdateAudioLevels();
        }

        private static IEnumerator ILaunch()
        {
            if (SongBrowser.authorableInstalled)
            {
                SetEndlessActive(true);
            }
            MenuState.I.GoToLaunchPage();
            LaunchPanel launchPanel = null;
            while(launchPanel is null)
            {
                launchPanel = GameObject.FindObjectOfType<LaunchPanel>();
                yield return new WaitForSecondsRealtime(.5f);
            }
            if (SongBrowser.authorableInstalled)
            {
                modifiersLoaded = false;
                LoadModifiers(false);
                while (!modifiersLoaded)
                {
                    yield return new WaitForSecondsRealtime(.2f);
                }
                
            }
            /*while (EnvironmentLoader.I.IsSwitching())
            {
                yield return new WaitForSecondsRealtime(.2f);
            }*/
            launchPanel.Play();            
            yield return null;
        }

        private static void SetEndlessActive(bool active)
        {
            AuthorableModifiersMod.SetEndlessActive(active);
        }
        private static bool modifiersLoaded = false;
        private static void LoadModifiers(bool fromRestart)
        {
            MelonCoroutines.Start(ILoadModifiers(fromRestart));
        }

        private static IEnumerator ILoadModifiers(bool fromRestart)
        {
            AuthorableModifiersMod.audicaFilePath = SongDataHolder.I.songData.foundPath;
            AuthorableModifiersMod.LoadModifierCues(false);
            while (!AuthorableModifiersMod.modifiersLoaded)
            {
                yield return new WaitForSecondsRealtime(.2f);
            }
            modifiersLoaded = true;
        }

        public static void ResetIndex()
        {
            index = 0;
            if (pendingReset)
            {
                pendingReset = false;
                Reset();
            }
        }

        private static void Reset()
        {
            PlayerPreferences.I.NoFail.mVal = previousNoFail;
        }

        private static void SetNextSong()
        {
            SongDataHolder.I.songData = songs[index].mSongData;
            index++;
            if (index == songs.Count)
            {
                PlaylistManager.state = PlaylistManager.PlaylistState.None;
                /*if (SongBrowser.authorableInstalled)
                {
                    SetEndlessActive(false);
                }*/
            }
        }

        private static bool CanPlay(SongSelect select)
        {             
            if (select is null)
            {
                MelonLogger.Warning("SongSelect not found");
                return false;
            }
            songs = new Il2CppSystem.Collections.Generic.List<SongSelectItem>();
            songs = select.GetSongButtons();
            songs.RemoveAt(0);
            if (songs.Count == 0)
            {
                MelonLogger.Warning("No songs in playlist");
                return false;
            }
            return true;
        }

        private static void Shuffle<T>(this Il2CppSystem.Collections.Generic.List<T> list)
        {
            System.Random random = new System.Random();
            int n = list.Count;           
            while(n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
