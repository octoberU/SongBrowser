using System.Resources;
using System.Reflection;
using System.Runtime.InteropServices;
using MelonLoader;
using AudicaModding;

[assembly: AssemblyTitle(AudicaMod.BuildInfo.Name)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(AudicaMod.BuildInfo.Company)]
[assembly: AssemblyProduct(AudicaMod.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + AudicaMod.BuildInfo.Author)]
[assembly: AssemblyTrademark(AudicaMod.BuildInfo.Company)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
//[assembly: Guid("")]
[assembly: AssemblyVersion(AudicaMod.BuildInfo.Version)]
[assembly: AssemblyFileVersion(AudicaMod.BuildInfo.Version)]
[assembly: NeutralResourcesLanguage("en")]
[assembly: MelonModInfo(typeof(AudicaMod), AudicaMod.BuildInfo.Name, AudicaMod.BuildInfo.Version, AudicaMod.BuildInfo.Author, AudicaMod.BuildInfo.DownloadLink)]


// Create and Setup a MelonModGame to mark a Mod as Universal or Compatible with specific Games.
// If no MelonModGameAttribute is found or any of the Values for any MelonModGame on the Mod is null or empty it will be assumed the Mod is Universal.
// Values for MelonModGame can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonModGame(null, null)]