using MelonLoader;
using System.Reflection;

namespace AudicaModding
{
    internal static class Config
    {
        public const string Category = "SongBrowser";

        public static bool SafeSongListReload;

        public static int LastSongCount { get; private set; }

        public static string playlistTitle = "[Header]Playlist Settings";

        public static bool Shuffle { get; set; }
        public static bool ShowScores { get; set; }
        public static bool NoFail { get; set; }
        public static bool ResetHealth { get; set; }
        public static void RegisterConfig()
        {
            MelonPreferences.CreateEntry(Category, nameof(SafeSongListReload), true,
                                    "Blocks access to song list during song list reload (safety feature).");

            MelonPreferences.CreateEntry(Category, nameof(LastSongCount), 0, "");
            LastSongCount = MelonPreferences.GetEntryValue<int>(Category, nameof(LastSongCount));

            MelonPreferences.CreateEntry(Category, nameof(playlistTitle), "", "[Header]Playlist Settings");

            MelonPreferences.CreateEntry(Category, nameof(Shuffle), false, "");
            Shuffle = MelonPreferences.GetEntryValue<bool>(Category, nameof(Shuffle));

            MelonPreferences.CreateEntry(Category, nameof(ShowScores), false, "");
            ShowScores = MelonPreferences.GetEntryValue<bool>(Category, nameof(ShowScores));

            MelonPreferences.CreateEntry(Category, nameof(NoFail), false, "");
            NoFail = MelonPreferences.GetEntryValue<bool>(Category, nameof(NoFail));

            MelonPreferences.CreateEntry(Category, nameof(ResetHealth), false, "");
            ResetHealth = MelonPreferences.GetEntryValue<bool>(Category, nameof(ResetHealth));

            OnPreferencesSaved();
        }

        public static void UpdateValue(string name, object value)
        {
            if(value is bool)
            {
                MelonPreferences.SetEntryValue(Category, name, (bool)value);
            }
        }

        public static void OnPreferencesSaved()
        {
            foreach (var fieldInfo in typeof(Config).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (fieldInfo.FieldType == typeof(bool))
                    fieldInfo.SetValue(null, MelonPreferences.GetEntryValue<bool>(Category, fieldInfo.Name));
            }
        }

        public static void UpdateSongCount(int newCount)
        {
            MelonPreferences.SetEntryValue(Category, nameof(LastSongCount), newCount);
            LastSongCount = newCount;
        }
    }
}
