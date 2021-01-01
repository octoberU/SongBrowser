using System;
using TMPro;
using UnityEngine;

namespace AudicaModding
{
	internal static class SongSearchScreen
	{
		public static TextMeshPro searchText;

		private static OptionsMenu primaryMenu;

		static public void SetMenu(OptionsMenu optionsMenu)
		{
			primaryMenu = optionsMenu;
		}

		static public void GoToSearch()
		{
			SongSearch.query = "";
			primaryMenu.ShowPage(OptionsMenu.Page.Customization);
			CleanUpPage(primaryMenu);
			AddButtons(primaryMenu);
			primaryMenu.screenTitle.text = "Search";
		}

		private static void AddButtons(OptionsMenu optionsMenu)
		{
			var header = optionsMenu.AddHeader(0, "Filter by: Artist, Title, Mapper");
			optionsMenu.scrollable.AddRow(header);

			var searchField = optionsMenu.AddButton(0, "Search:", new Action(() => { SongBrowser.shouldShowKeyboard = true; optionsMenu.keyboard.Show(); }), null, "Filter by: Artist, Title, Mapper", optionsMenu.textEntryButtonPrefab);
			optionsMenu.scrollable.AddRow(searchField.gameObject);
			searchText = searchField.gameObject.GetComponentInChildren<TextMeshPro>();
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