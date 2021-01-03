using MelonLoader;
using System;
using TMPro;
using UnityEngine;

namespace AudicaModding
{
	internal static class SongLoadingManager
	{
		private static GunButton   soloButton               = null;
		private static TextMeshPro soloButtonLabel          = null;
		private static string      originalSoloButtonText   = null;

		private static GunButton   partyButton              = null;
		private static TextMeshPro partyButtonLabel         = null;
		private static string      originalPartyButtonText  = null;

		private static bool searching = false;
		private static bool disabled  = false;

		/// <summary>
		/// Ensures that once song search is complete (via SongList.OnSongListLoaded)
		/// various types of post-processing happens and that the UI is re-enabled.
		/// 
		/// Add any post-processing that would be triggered on SongList.OnSongListLoaded
		/// here if you want control over the order that post-processing happens in/
		/// want to ensure it is completed before the UI is re-enabled.
		/// </summary>
		public static void StartSongListUpdate()
		{
			if (searching)
				return;

			searching = true;

			SongList.OnSongListLoaded.On(new Action(() => {
				MapperNames.FixMappers();

				if (SongBrowser.songDataLoaderInstalled)
                {
					SafeDataLoaderReload();
				}

				SongBrowser.UpdateSongCaches();
				SongSearch.Search(); // update the search results with any new songs (if there is a search)

				KataConfig.I.CreateDebugText("Songs Loaded", new Vector3(0f, -1f, 5f), 5f, null, false, 0.2f);

				EnableButtons();
				searching = false;
			}));

			UpdateUI();
		}

		/// <summary>
		/// Makes sure the Song and Party buttons are only available if there is not currently
		/// an ongoing song search.
		/// </summary>
		public static void UpdateUI()
        {
			if (!Config.SafeSongListReload)
				return;

			if (!searching || disabled || MenuState.GetState() != MenuState.State.MainPage)
				return;

			disabled = true;

			if (soloButton == null)
			{
				soloButton      = GameObject.Find("menu/ShellPage_Main/page/ShellPanel_Center/Solo/Button").GetComponent<GunButton>();
				soloButtonLabel = GameObject.Find("menu/ShellPage_Main/page/ShellPanel_Center/Solo/Label").GetComponent<TextMeshPro>();
				GameObject.Destroy(soloButtonLabel.gameObject.GetComponent<Localizer>());
			}

			originalSoloButtonText  = soloButtonLabel.text;
			soloButtonLabel.text    = "Loading...";
			soloButton.SetInteractable(false);
			
			if (!SongBrowser.modSettingsInstalled)
            {
				if (partyButton == null)
				{
					partyButton      = GameObject.Find("menu/ShellPage_Main/page/ShellPanel_Center/Party/Button").GetComponent<GunButton>();
					partyButtonLabel = GameObject.Find("menu/ShellPage_Main/page/ShellPanel_Center/Party/Label").GetComponent<TextMeshPro>();
					GameObject.Destroy(partyButtonLabel.gameObject.GetComponent<Localizer>());
				}
				originalPartyButtonText  = partyButtonLabel.text;
				partyButtonLabel.text    = "Loading...";
				partyButton.SetInteractable(false);
            }
		}

		private static void EnableButtons()
		{
			if (!Config.SafeSongListReload)
				return;

			if (disabled)
			{
				soloButton.SetInteractable(true);
				soloButtonLabel.text = originalSoloButtonText;
				if (!SongBrowser.modSettingsInstalled)
				{
					partyButton.SetInteractable(true);
					partyButtonLabel.text = originalPartyButtonText;
				}
			}

			disabled = false;
		}

		private static void SafeDataLoaderReload()
		{
			SongDataLoader.ReloadSongData();
			MelonLogger.Log("Song Data Reloaded");
		}
	}
}