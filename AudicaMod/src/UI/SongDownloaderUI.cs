using System.Collections;
using System;
using TMPro;
using UnityEngine;
using MelonLoader;
using System.Linq;
using System.Collections.Generic;

namespace AudicaModding
{
	internal static class SongDownloaderUI
	{
		public static TextMeshPro searchText;
		public static GameObject secondaryPanel;
		public static ShellPage songItemPanel;
		public static OptionsMenu songItemMenu;
		public static DifficultyFilter difficultyFilter = DifficultyFilter.All;
		public static OptionsMenuButton difficultyToggle;
		public static bool curated;
		public static OptionsMenuButton curatedToggle;
		public static bool popularity;
		public static OptionsMenuButton popularityToggle;
		public static APISongList activeSongList;

		private static OptionsMenu primaryMenu;
		private static GunButton   backButton;
		private static TextMeshPro backText;
		private static string      originalBackText;
		private static int		   activeDownloadsCount = 0;

		private static Dictionary<Song, OptionsMenuButton> downloadButtons     = new Dictionary<Song, OptionsMenuButton>();
		private static HashSet<string>                     downloadedFileNames = new HashSet<string>();

		static public void AddPageButton(OptionsMenu optionsMenu, int col)
		{
			primaryMenu = optionsMenu;
			primaryMenu.AddButton(col, "Download Songs", new Action(() => {
				GoToWebSearchPage();
				if (songItemPanel != null)
					songItemPanel.SetPageActive(true);
			}), null, "Download new maps from the Audica Modding Discord");

		}

		public static void GoToWebSearchPage()
		{
			downloadedFileNames.Clear();
			SongDownloader.StartNewSongSearch();

			if (backButton == null)
			{
				var button       = GameObject.Find("menu/ShellPage_Settings/page/backParent/back");
				backButton       = button.GetComponentInChildren<GunButton>();
				backText         = button.GetComponentInChildren<TextMeshPro>();
				originalBackText = backText.text;

				UnityEngine.Object.Destroy(button.GetComponentInChildren<Localizer>());
			}

			if (songItemPanel == null)
			{
				secondaryPanel = GameObject.Instantiate(GameObject.Find("ShellPage_Settings"));
				secondaryPanel.SetActive(true);
				secondaryPanel.transform.Rotate(0, -65, 0);
				SpawnSecondaryPanel(secondaryPanel);
			}
			else
			{
				SpawnSecondaryPanel(secondaryPanel);
			}

			primaryMenu.ShowPage(OptionsMenu.Page.Customization);
			CleanUpPage(primaryMenu);
			AddButtons(primaryMenu);
			primaryMenu.screenTitle.text = "Filters";
			SongBrowser.lastSongCount = SongBrowser.newSongCount; //User has seen new songs
			SongBrowser.SavePrivateConfig();
		}

		public static void ResetScrollPosition()
        {
			songItemMenu?.scrollable.SnapTo(0);
		}
		
		private static void SpawnSecondaryPanel(GameObject secondaryPanel)
		{
			songItemPanel = secondaryPanel.GetComponent<ShellPage>();
			songItemPanel.SetPageActive(true, true);
			songItemPanel.gameObject.transform.GetChild(0).GetChild(1).gameObject.SetActive(false); // Removes the back button
			MelonCoroutines.Start(WaitForSpawningMenu(secondaryPanel));
		}

		private static void SetupSecondaryPanel(GameObject secondaryPanel)
		{
			songItemMenu = secondaryPanel.GetComponentInChildren<OptionsMenu>();
			songItemMenu.ShowPage(OptionsMenu.Page.Customization);
			CleanUpPage(songItemMenu);
			songItemMenu.screenTitle.text = "Songs";
		}

		private static void AddButtons(OptionsMenu optionsMenu)
		{
			var header = optionsMenu.AddHeader(0, "Filter by: Artist, Title, Mapper");
			optionsMenu.scrollable.AddRow(header);

			var searchField = optionsMenu.AddButton(0, "Search:", new Action(() => { SongBrowser.shouldShowKeyboard = true; optionsMenu.keyboard.Show(); }), null, "Filter by: Artist, Title, Mapper", optionsMenu.textEntryButtonPrefab);
			optionsMenu.scrollable.AddRow(searchField.gameObject);
			searchText = searchField.gameObject.GetComponentInChildren<TextMeshPro>();

			var difficultyHeader = optionsMenu.AddHeader(0, "Filter difficulty");
			optionsMenu.scrollable.AddRow(difficultyHeader);

			string difficultyFilterText = difficultyFilter.ToString();
			difficultyToggle = optionsMenu.AddButton
				(0,
				difficultyFilterText,
				new Action(() =>
				{
					difficultyFilter++;
					if ((int)difficultyFilter > 4) difficultyFilter = 0;
					difficultyToggle.label.text = difficultyFilter.ToString();
					SongDownloader.StartNewSongSearch();
				}),
				null,
				"Filters the search to the selected difficulty");
			difficultyToggle.button.doMeshExplosion = false;
			difficultyToggle.button.doParticles = false;
			optionsMenu.scrollable.AddRow(difficultyToggle.gameObject);

			var extraHeader = optionsMenu.AddHeader(0, "Extra");
			optionsMenu.scrollable.AddRow(extraHeader);

			string curatedFilterText = "Curated only: " + curated.ToString();
			curatedToggle = optionsMenu.AddButton
				(0,
				curatedFilterText,
				new Action(() =>
				{
					if (curated)
						curated = false;
					else
						curated = true;

					curatedToggle.label.text = "Curated only: " + curated.ToString();
					SongDownloader.StartNewSongSearch();
				}),
				null,
				"Filters the search to curated maps only");
			curatedToggle.button.doMeshExplosion = false;
			curatedToggle.button.doParticles = false;
			optionsMenu.scrollable.AddRow(curatedToggle.gameObject);

			var downloadFullPage = optionsMenu.AddButton
				(1,
				"Download current page",
				new Action(() =>
				{
					DownloadFullPage();
				}),
				null,
				"Downloads all songs from the current page, this will cause major stutters");

			var RestoreSongs = optionsMenu.AddButton
				(0,
				"Restore Deleted Songs",
				new Action(() =>
				{
					SongBrowser.RestoreDeletedSongs();
				}),
				null,
				"Restores all the songs you have deleted.");
			//optionsMenu.scrollable.AddRow(RestoreSongs.gameObject);

			string popularityFilterText = "Sort by playcount: " + popularity.ToString();
			popularityToggle = optionsMenu.AddButton
				(1,
				popularityFilterText,
				new Action(() =>
				{
					if (popularity)
						popularity = false;
					else
						popularity = true;

					popularityToggle.label.text = "Sort by playcount: " + popularity.ToString();
					SongDownloader.StartNewSongSearch();
				}),
				null,
				"Sorts downloads by leaderboard scores rather than date.");
			popularityToggle.button.doMeshExplosion = false;
			popularityToggle.button.doParticles = false;
			optionsMenu.scrollable.AddRow(popularityToggle.gameObject);

			var downloadFolderBlock = optionsMenu.AddTextBlock(0, "You can hotload songs by placing them in Audica/Downloads and pressing F5");
			optionsMenu.scrollable.AddRow(downloadFolderBlock);
		}

		private static void DownloadFullPage()
		{
			// for now this debug message will be visible the entire time
			// once downloads are actually async this should be called once every few seconds
			KataConfig.I.CreateDebugText("Downloading...", new Vector3(0f, -1f, 5f), 5f, null, false, 0.2f);
			foreach (var song in activeSongList.songs)
			{
				OnDownloadStart(song);
				MelonCoroutines.Start(SongDownloader.DownloadSong(song.download_url, () => { OnDownloadDone(song); }));
			}
		}

		public static void AddSongItems(OptionsMenu optionsMenu, APISongList songlist)
		{
			CleanUpPage(optionsMenu);
			activeSongList = songlist;
			optionsMenu.screenTitle.text = "Displaying page " + SongDownloader.page.ToString() + " out of " + songlist.total_pages.ToString();

			var pageHeader = optionsMenu.AddHeader(0, "Listing " + songlist.song_count.ToString() + " songs");
			optionsMenu.scrollable.AddRow(pageHeader.gameObject);

			AddPageButtons(optionsMenu);

			foreach (var song in songlist.songs)
			{
				CreateSongItem(song, optionsMenu);
			}

			AddPageButtons(optionsMenu);
		}

		private static void AddPageButtons(OptionsMenu optionsMenu)
		{
			var row = new Il2CppSystem.Collections.Generic.List<GameObject>();
			var previousPage = optionsMenu.AddButton(0,
				"Previous Page",
				new Action(() => { SongDownloader.PreviousPage(); SongDownloader.StartNewPageSearch(); optionsMenu.scrollable.SnapTo(0); }),
				null,
				null);
			row.Add(previousPage.gameObject);

			var nextPage = optionsMenu.AddButton(1,
				"Next Page",
				new Action(() => { SongDownloader.NextPage(); SongDownloader.StartNewPageSearch(); optionsMenu.scrollable.SnapTo(0); }),
				null,
				null);
			row.Add(nextPage.gameObject);
			optionsMenu.scrollable.AddRow(row);
		}

		private static void CreateSongItem(Song song, OptionsMenu optionsMenu)
		{
			var row = new Il2CppSystem.Collections.Generic.List<GameObject>();
			
			var textBlock = optionsMenu.AddTextBlock(0, song.title + " - " + song.artist + " (mapped by " + song.author + ")");
			var TMP = textBlock.transform.GetChild(0).GetComponent<TextMeshPro>();
			TMP.fontSizeMax = 32;
			TMP.fontSizeMin = 8;
			optionsMenu.scrollable.AddRow(textBlock.gameObject);

            //package data to be used for display
            SongBrowser.SongDisplayPackage songd = new SongBrowser.SongDisplayPackage();

            songd.hasEasy = song.beginner;
            songd.hasStandard = song.standard;
            songd.hasAdvanced = song.advanced;
            songd.hasExpert = song.expert;

            //if song data loader is installed look for custom tags
            if (SongBrowser.songDataLoaderInstalled)
            {
                songd = SongBrowser.SongDisplayPackage.FillCustomData(songd, song.song_id);
            }

            songd.customExpertTags = songd.customExpertTags.Distinct().ToList();
            songd.customStandardTags = songd.customStandardTags.Distinct().ToList();
            songd.customAdvancedTags = songd.customAdvancedTags.Distinct().ToList();
            songd.customEasyTags = songd.customEasyTags.Distinct().ToList();

			bool   destroyOnShot = true;
			Action onHit         = new Action(() => {
				OnDownloadStart(song);
				MelonCoroutines.Start(SongDownloader.DownloadSong(song.download_url, () => { OnDownloadDone(song); }));
				KataConfig.I.CreateDebugText("Downloading...", new Vector3(0f, -1f, 5f), 5f, null, false, 0.2f);
			});
			string label         = "Download" + SongBrowser.GetDifficultyString(songd);
			float  alpha         = 1f;
			bool   interactable  = true;

			string[] splitURL = song.download_url.Split('/');
			string audicaName = splitURL[splitURL.Length - 1];

			if (SongLoadingManager.songFilenames.Contains(song.filename) || downloadedFileNames.Contains(song.filename))
            {
				destroyOnShot = false;
				onHit         = new Action(() => { });
				label         = "Downloaded";
				alpha         = 0.25f;
				interactable  = false;
			}

			var downloadButton = optionsMenu.AddButton(0,
				label,
				onHit,
				null,
				null);
			downloadButton.button.SetInteractable(interactable);
			downloadButton.button.destroyOnShot   = destroyOnShot;
			downloadButton.button.doMeshExplosion = destroyOnShot;
			downloadButton.label.alpha            = alpha;

			downloadButtons.Add(song, downloadButton);

			row.Add(downloadButton.gameObject);

			var previewButton = optionsMenu.AddButton(1,
				"Preview",
				new Action(() => { MelonCoroutines.Start(SongDownloader.StreamPreviewSong(song.preview_url)); }),
				null,
				null);
			row.Add(previewButton.gameObject);


			optionsMenu.scrollable.AddRow(row);
		}

		private static void OnDownloadStart(Song song)
        {
			activeDownloadsCount++;

			if (activeDownloadsCount > 0)
            {
				backButton.SetInteractable(false);
				backText.alpha = 0.25f;
				backText.text  = "Waiting...";
            }
			if (downloadButtons.ContainsKey(song))
            {
				downloadButtons[song].button.SetInteractable(false);
				downloadButtons[song].button.destroyOnShot   = false;
				downloadButtons[song].button.doMeshExplosion = false;
				downloadButtons[song].label.alpha = 0.25f;
				downloadButtons[song].label.text  = "Downloading...";
			}
        }

		private static void OnDownloadDone(Song song)
        {
			downloadedFileNames.Add(song.filename);
			if (downloadButtons.ContainsKey(song))
			{
				downloadButtons[song].label.text = "Downloaded!";
			}
			activeDownloadsCount--;
			
			if (activeDownloadsCount == 0)
			{
				backText.alpha = 1.0f;
				backText.text  = originalBackText;
				backButton.SetInteractable(true);
			}
		}

		private static void CleanUpPage(OptionsMenu optionsMenu)
		{
			Transform optionsTransform = optionsMenu.transform;
			for (int i = 0; i < optionsTransform.childCount; i++)
			{
				Transform child = optionsTransform.GetChild(i);
				if (child.gameObject.name.Contains("(Clone)"))
				{
					GameObject.Destroy(child.gameObject);
				}
			}
			optionsMenu.mRows.Clear();
			optionsMenu.scrollable.ClearRows();
			optionsMenu.scrollable.mRows.Clear();
			downloadButtons.Clear();
		}

		static IEnumerator WaitForSpawningMenu(GameObject panel)
		{
			yield return new WaitForSeconds(0.05f);
			SetupSecondaryPanel(panel);
			AddSongItems(panel.GetComponentInChildren<OptionsMenu>(), SongDownloader.songlist);
		}
	}
}

public enum DifficultyFilter
{
	All,
	Expert,
	Advanced,
	Standard,
	Beginner,
}