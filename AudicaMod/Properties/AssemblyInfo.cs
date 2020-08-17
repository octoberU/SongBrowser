using System.Resources;
using System.Reflection;
using System.Runtime.InteropServices;
using MelonLoader;
using AudicaModding;

[assembly: AssemblyTitle(SongBrowser.BuildInfo.Name)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(SongBrowser.BuildInfo.Company)]
[assembly: AssemblyProduct(SongBrowser.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + SongBrowser.BuildInfo.Author)]
[assembly: AssemblyTrademark(SongBrowser.BuildInfo.Company)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
//[assembly: Guid("")]
[assembly: AssemblyVersion(SongBrowser.BuildInfo.Version)]
[assembly: AssemblyFileVersion(SongBrowser.BuildInfo.Version)]
[assembly: NeutralResourcesLanguage("en")]
[assembly: MelonInfo(typeof(SongBrowser), SongBrowser.BuildInfo.Name, SongBrowser.BuildInfo.Version, SongBrowser.BuildInfo.Author, SongBrowser.BuildInfo.DownloadLink)]


// Create and Setup a MelonModGame to mark a Mod as Universal or Compatible with specific Games.
// If no MelonModGameAttribute is found or any of the Values for any MelonModGame on the Mod is null or empty it will be assumed the Mod is Universal.
// Values for MelonModGame can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("Harmonix Music Systems, Inc.", "Audica")]