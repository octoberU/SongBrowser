using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace AudicaModding
{
	internal static class SongSearchScreen
	{
		public static TextMeshPro searchText;

		private static OptionsMenu       primaryMenu;
		private static OptionsMenuButton mapTypeToggle;

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
			var header = optionsMenu.AddHeader(0, "Search by: Artist, Title, Mapper");
			optionsMenu.scrollable.AddRow(header);

			var searchField = optionsMenu.AddButton(0, "Search:", new Action(() => { SongBrowser.shouldShowKeyboard = true; optionsMenu.keyboard.Show(); }), null, "Filter by: Artist, Title, Mapper", optionsMenu.textEntryButtonPrefab);			
			optionsMenu.scrollable.AddRow(searchField.gameObject);
			searchText = searchField.gameObject.GetComponentInChildren<TextMeshPro>();

			var infoText = optionsMenu.AddTextBlock(0, "Searching for nothing will find all songs, unless limited by additional filters.");
			optionsMenu.scrollable.AddRow(infoText);

			var filtersHeader = optionsMenu.AddHeader(0, "Additional filters");
			optionsMenu.scrollable.AddRow(filtersHeader);

			mapTypeToggle = optionsMenu.AddButton(0,
				SplitCamelCase(SongSearch.mapType.ToString()),
				new Action(() =>
				{
					SongSearch.mapType++;
					if ((int)SongSearch.mapType > 2) SongSearch.mapType = SongSearch.MapType.All;
					mapTypeToggle.label.text = SplitCamelCase(SongSearch.mapType.ToString());
				}),
				null,
				"Filters the search to the selected map type");
			mapTypeToggle.button.doMeshExplosion = false;
			mapTypeToggle.button.doParticles     = false;
			optionsMenu.scrollable.AddRow(mapTypeToggle.gameObject);
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

		private static string SplitCamelCase(string text)
        {
			return Regex.Replace(text, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");
        }
	}
}