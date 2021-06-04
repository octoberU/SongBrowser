using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace AudicaModding
{
	internal static class PlaylistSelectPanel
	{
		private static OptionsMenu	primaryMenu;

		static public void SetMenu(OptionsMenu optionsMenu)
		{
			primaryMenu = optionsMenu;
		}

		static public void GoToPanel()
		{
			primaryMenu.ShowPage(OptionsMenu.Page.Customization);
			CleanUpPage(primaryMenu);
			AddButtons(primaryMenu);
			primaryMenu.screenTitle.text = "Playlists";
		}

		public static void CancelSelect()
		{
			PlaylistManager.state = PlaylistManager.PlaylistState.None;
			MenuState.I.GoToSongPage();
			SelectPlaylistButton.UpdatePlaylistButton();
		}

		private static void AddButtons(OptionsMenu optionsMenu)
		{
			/*var header = optionsMenu.AddHeader(0, "Playlists");
			optionsMenu.scrollable.AddRow(header);*/
			OptionsMenuButton entry = null;
			foreach(string playlist in PlaylistManager.playlists.Keys)
            {
				var name = optionsMenu.AddTextBlock(0, playlist);
				var tmp = name.transform.GetChild(0).GetComponent<TextMeshPro>();
				tmp.fontSizeMax = 32;
				tmp.fontSizeMin = 8;
				optionsMenu.scrollable.AddRow(name.gameObject);
				OptionsMenuButton edit = null;
				if(PlaylistManager.state == PlaylistManager.PlaylistState.Selecting) 
				{
					edit = optionsMenu.AddButton(0, "Edit", new Action(() =>
					{
						PlaylistManager.state = PlaylistManager.PlaylistState.Editing;
						PlaylistManager.SetPlaylistToEdit(playlist);
						OptionsMenu.I.ShowPage(OptionsMenu.Page.Misc);
						SelectPlaylistButton.UpdatePlaylistButton();
					}), null, "Edit this playlist", optionsMenu.buttonPrefab);
				}

				string txt = PlaylistManager.state == PlaylistManager.PlaylistState.Selecting ? "Select" : "Add";
				entry = optionsMenu.AddButton(1, txt, new Action(() =>
				{
					if(PlaylistManager.state == PlaylistManager.PlaylistState.Selecting)
                    {
						PlaylistManager.SelectPlaylist(playlist);
                    }
                    else
                    {
						PlaylistManager.AddSongToPlaylist(playlist, AddPlaylistButton.songToAdd);
						MenuState.I.GoToLaunchPage();
						return;
                    }
					PlaylistManager.state = PlaylistManager.PlaylistState.None;
					FilterPanel.ResetFilterState();
					MenuState.I.GoToSongPage();
					SelectPlaylistButton.UpdatePlaylistButton();
				}), null, "Select this playlist", optionsMenu.buttonPrefab);

				Il2CppSystem.Collections.Generic.List<GameObject> row = new Il2CppSystem.Collections.Generic.List<GameObject>();
				//row.Add(name.gameObject);
				if (PlaylistManager.state == PlaylistManager.PlaylistState.Selecting) row.Add(edit.gameObject);
				row.Add(entry.gameObject);
				optionsMenu.scrollable.AddRow(row);
			}
			var header = optionsMenu.AddHeader(0, "Create");
			optionsMenu.scrollable.AddRow(header);
			entry = optionsMenu.AddButton(0, "Create new Playlist", new Action(() =>
			{
				MelonLoader.MelonLogger.Log("Create button shot");
				PlaylistManager.state = PlaylistManager.PlaylistState.Creating;
				OptionsMenu.I.ShowPage(OptionsMenu.Page.Misc);
				MenuState.I.GoToSettingsPage();
				SelectPlaylistButton.UpdatePlaylistButton();
			}), null, "Create a new playlist", optionsMenu.buttonPrefab);
			optionsMenu.scrollable.AddRow(entry.gameObject);
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