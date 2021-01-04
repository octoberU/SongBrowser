using MelonLoader;
using System.Reflection;

namespace AudicaModding
{
    internal static class Config
    {
        public const string Category = "SongBrowser";

        public static bool SafeSongListReload;

        public static bool HideDownloadedSongs;

        public static void RegisterConfig()
        {
            MelonPrefs.RegisterBool(Category, nameof(SafeSongListReload), true, 
                                    "Blocks access to song list during song list reload (safety feature).");

            MelonPrefs.RegisterBool(Category, nameof(HideDownloadedSongs), true,
                                    "Will only show songs that have not been downloaded yet in the download menu.");

            OnModSettingsApplied();
        }

        public static void OnModSettingsApplied()
        {
            foreach (var fieldInfo in typeof(Config).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (fieldInfo.FieldType == typeof(bool))
                    fieldInfo.SetValue(null, MelonPrefs.GetBool(Category, fieldInfo.Name));
            }
        }
    }
}
