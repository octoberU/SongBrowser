using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace AudicaModding
{
	internal static class PlaylistEditPanel
	{
		private static OptionsMenu primaryMenu;
		private static GunButton backButton = null;
		static public void SetMenu(OptionsMenu optionsMenu)
		{
			primaryMenu = optionsMenu;
		}

		static public void GoToPanel()
		{
			primaryMenu.ShowPage(OptionsMenu.Page.Customization);
			RefreshList();
		}

		public static void CancelEdit()
		{
			PlaylistManager.SavePlaylist(PlaylistManager.playlistToEdit.name, true);
			/*if (PlaylistDownloadManager.needReload)
            {
				PlaylistManager.state = PlaylistManager.PlaylistState.None;
				MenuState.I.GoToSongPage();
				//PlaylistDownloadManager.Reload();
			}
            else
            {
				PlaylistManager.state = PlaylistManager.PlaylistState.Selecting;
				OptionsMenu.I.ShowPage(OptionsMenu.Page.Main);
            }*/
			PlaylistManager.state = PlaylistManager.PlaylistState.Selecting;
			OptionsMenu.I.ShowPage(OptionsMenu.Page.Main);
			SelectPlaylistButton.UpdatePlaylistButton();
		}

		private static void RefreshList()
        {
			CleanUpPage(primaryMenu);
			AddButtons(primaryMenu);
			//primaryMenu.screenTitle.text = "Edit Playlist";
			primaryMenu.screenTitle.text = PlaylistManager.playlistToEdit.name;
        }

		private static void AddButtons(OptionsMenu optionsMenu)
		{
			//var header = optionsMenu.AddHeader(0, PlaylistManager.playlistToEdit.name);
			var header = optionsMenu.AddHeader(0, "Song List");
			optionsMenu.scrollable.AddRow(header);
			int index = 0;
			foreach (KeyValuePair<string, string> song in PlaylistManager.playlistToEdit.songNames)
			{
				var name = optionsMenu.AddTextBlock(0, song.Value);
				var tmp = name.transform.GetChild(0).GetComponent<TextMeshPro>();
				tmp.fontSizeMax = 32;
				tmp.fontSizeMin = 8;
				optionsMenu.scrollable.AddRow(name.gameObject);

				var delete = optionsMenu.AddButton(1, "Remove", new Action(() =>
				{
					PlaylistManager.RemoveSongFromPlaylist(song.Key);
					RefreshList();
				}), null, "Removes this song from this playlist", optionsMenu.buttonPrefab);
				Il2CppSystem.Collections.Generic.List<GameObject> row = new Il2CppSystem.Collections.Generic.List<GameObject>();
				row.Add(delete.gameObject);
				if(!SongLoadingManager.songDictionary.ContainsKey(song.Key + ".audica"))
                {
					var download = optionsMenu.AddButton(0, "Download", new Action(() =>
					{
						if (backButton is null)
						{
							var button = GameObject.Find("menu/ShellPage_Settings/page/backParent/back");
							backButton = button.GetComponentInChildren<GunButton>();
						}
						backButton.SetInteractable(false);
						PlaylistManager.DownloadSong(song.Key, backButton);

					}), null, "Download this song", optionsMenu.buttonPrefab);
					download.button.destroyOnShot = true;
					row.Add(download.gameObject);
				}
				
				optionsMenu.scrollable.AddRow(row);
				row = new Il2CppSystem.Collections.Generic.List<GameObject>();
				if(index < PlaylistManager.playlistToEdit.songs.Count - 1)
                {
					var moveDown = optionsMenu.AddButton(0, "Move Down", new Action(() =>
					{
						PlaylistManager.MoveSongDown(song.Key);
						RefreshList();
					}), null, "Moves this song down in the playlist", optionsMenu.buttonPrefab);
					row.Add(moveDown.gameObject);
				}
				if(index != 0)
                {
					var moveUp = optionsMenu.AddButton(1, "Move Up", new Action(() =>
					{
						PlaylistManager.MoveSongUp(song.Key);
						RefreshList();
					}), null, "Moves this song up in the playlist", optionsMenu.buttonPrefab);
					row.Add(moveUp.gameObject);
				}
				optionsMenu.scrollable.AddRow(row);
				index++;
			}
			header = optionsMenu.AddHeader(0, "Delete Playlist");
			optionsMenu.scrollable.AddRow(header);
			var entry = optionsMenu.AddButton(0, "Delete", new Action(() =>
			{
				PlaylistManager.DeletePlaylist();
				PlaylistManager.state = PlaylistManager.PlaylistState.Selecting;				
				OptionsMenu.I.ShowPage(OptionsMenu.Page.Main);
				SelectPlaylistButton.UpdatePlaylistButton();
			}), null, "Deletes this Playlist", optionsMenu.buttonPrefab);
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