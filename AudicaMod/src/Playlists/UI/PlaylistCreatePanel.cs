using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace AudicaModding
{
	internal static class PlaylistCreatePanel
	{
		public static TextMeshPro playlistText;
		public static string newName;
		private static OptionsMenu primaryMenu;

		static public void SetMenu(OptionsMenu optionsMenu)
		{
			primaryMenu = optionsMenu;
		}

		static public void GoToPanel()
		{
			newName = "";
			primaryMenu.ShowPage(OptionsMenu.Page.Customization);
			CleanUpPage(primaryMenu);
			AddButtons(primaryMenu);
			primaryMenu.screenTitle.text = "Create Playlist";
		}

		public static void CancelCreate()
		{
			PlaylistManager.state = PlaylistManager.PlaylistState.Selecting;
			OptionsMenu.I.ShowPage(OptionsMenu.Page.Main);
			SelectPlaylistButton.UpdatePlaylistButton();
		}

		private static void AddButtons(OptionsMenu optionsMenu)
		{
			var header = optionsMenu.AddHeader(0, "Playlist Name");
			optionsMenu.scrollable.AddRow(header);

			var nameField = optionsMenu.AddButton(0, "Name:", new Action(() => { SongBrowser.shouldShowKeyboard = true; optionsMenu.keyboard.Show(); }), null, "Enter the desired name for the playlist", optionsMenu.textEntryButtonPrefab);
			optionsMenu.scrollable.AddRow(nameField.gameObject);
			playlistText = nameField.gameObject.GetComponentInChildren<TextMeshPro>();

			var createButton = optionsMenu.AddButton(0, "Create Playlist", new Action(() =>
			{
				if (newName.Length == 0) return;

				Playlist playlist = new Playlist(newName, new List<string>());
				PlaylistManager.AddNewPlaylist(playlist, true);
				//PlaylistManager.SelectPlaylist(playlist.name);
				PlaylistManager.SavePlaylist(playlist.name, false);
				PlaylistManager.SavePlaylistData();
				CancelCreate();
			}), null, "Create playlist with the entered name", optionsMenu.buttonPrefab);
			optionsMenu.scrollable.AddRow(createButton.gameObject);
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
	}
}