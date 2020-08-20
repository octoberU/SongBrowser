using System.Collections;
using System;
using TMPro;
using UnityEngine;
using MelonLoader;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Experimental.XR;

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
		public static APISongList activeSongList;

		static public void AddPageButton(OptionsMenu optionsMenu, int col)
		{
			optionsMenu.AddButton(col, "Download Songs", new System.Action(() => {
				GoToArenaPage(optionsMenu);
				if (songItemPanel != null)
					songItemPanel.SetPageActive(true);
				}), null, "Download new maps from the Audica Modding Discord");
		}

		public static void GoToArenaPage(OptionsMenu optionsMenu)
		{
			SongBrowser.page = 1;
			if(songItemPanel == null)
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
			
			optionsMenu.ShowPage(OptionsMenu.Page.Customization);	
			CleanUpPage(optionsMenu);
			AddButtons(optionsMenu);
			optionsMenu.screenTitle.text = "Filters";
		}
		
		private static void SpawnSecondaryPanel(GameObject secondaryPanel)
		{
			songItemPanel = secondaryPanel.GetComponent<ShellPage>();
			songItemPanel.SetPageActive(true, true);
			songItemPanel.gameObject.transform.GetChild(0).GetChild(1).gameObject.SetActive(false); // Removes the back button
			MelonCoroutines.Start(WaitForSpawningMenu(secondaryPanel));
		}

		private static void SetupSeoondaryPanel(GameObject secondaryPanel)
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
					SongBrowser.page = 1;
					SongBrowser.StartSongSearch();
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
					SongBrowser.page = 1;
					SongBrowser.StartSongSearch();
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
			optionsMenu.scrollable.AddRow(RestoreSongs.gameObject);
		}

		private static void DownloadFullPage()
		{
			foreach (var song in activeSongList.songs)
			{
				MelonCoroutines.Start(SongBrowser.DownloadSong(song.download_url));
			}
		}

		public static void AddSongItems(OptionsMenu optionsMenu, APISongList songlist)
		{
			CleanUpPage(optionsMenu);
			activeSongList = songlist;
			optionsMenu.screenTitle.text = "Displaying page " + SongBrowser.page.ToString() + " out of " + songlist.total_pages.ToString();

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
				new Action(() => { SongBrowser.PreviousPage(); SongBrowser.StartSongSearch(); optionsMenu.scrollable.SnapTo(0); }),
				null,
				null);
			row.Add(previousPage.gameObject);

			var nextPage = optionsMenu.AddButton(1,
				"Next Page",
				new Action(() => { SongBrowser.NextPage(); SongBrowser.StartSongSearch(); optionsMenu.scrollable.SnapTo(0); }),
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
			
			var downloadButton = optionsMenu.AddButton(0,
				"Download" + SongBrowser.GetDifficultyString(song.beginner, song.standard, song.advanced, song.expert),
				new Action(() => { MelonCoroutines.Start(SongBrowser.DownloadSong(song.download_url)); TMP.text = "Added song to download queue!"; }),
				null,
				null);
			downloadButton.button.destroyOnShot = true;

			row.Add(downloadButton.gameObject);

			var previewButton = optionsMenu.AddButton(1,
				"Preview",
				new Action(() => { MelonCoroutines.Start(SongBrowser.StreamPreviewSong(song.preview_url)); }),
				null,
				null);
			row.Add(previewButton.gameObject);


			optionsMenu.scrollable.AddRow(row);
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
		}

		static IEnumerator WaitForSpawningMenu(GameObject panel)
		{
			yield return new WaitForSeconds(0.01f);
			SetupSeoondaryPanel(panel);
			AddSongItems(panel.GetComponentInChildren<OptionsMenu>(), SongBrowser.songlist);
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