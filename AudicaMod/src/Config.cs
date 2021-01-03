using MelonLoader;
using System.Reflection;

namespace AudicaModding
{
    internal static class Config
    {
        public const string Category = "SongBrowser";

        public static bool BlockOnSongListReload;

        public static void RegisterConfig()
        {
            MelonPrefs.RegisterBool(Category, nameof(BlockOnSongListReload), true, 
                                    "Blocks access to song list during song list reload (safety feature).");

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
