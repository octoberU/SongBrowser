using MelonLoader;
using System.Reflection;

namespace AudicaModding
{
    internal static class Config
    {
        public const string Category = "SongBrowser";

        public static bool SafeSongListReload;

        public static int LastSongCount { get; private set; }
        public static int RandomSongBagSize { get; private set; }

        public static string playlistTitle = "[Header]Playlist Settings";

        public static bool Shuffle { get; set; }
        public static bool ShowScores { get; set; }
        public static bool NoFail { get; set; }
        public static bool ResetHealth { get; set; }
        public static void RegisterConfig()
        {
            MelonPrefs.RegisterBool(Category, nameof(SafeSongListReload), true,
                                    "Blocks access to song list during song list reload (safety feature).");

            MelonPrefs.RegisterInt(Category, nameof(LastSongCount), 0, "", true);
            LastSongCount = MelonPrefs.GetInt(Category, nameof(LastSongCount));

            MelonPrefs.RegisterInt(Category, nameof(RandomSongBagSize), 10, "", true);
            RandomSongBagSize = MelonPrefs.GetInt(Category, nameof(RandomSongBagSize));

            MelonPrefs.RegisterString(Category, nameof(playlistTitle), "", "", true);           

            MelonPrefs.RegisterBool(Category, nameof(Shuffle), false, "", false);
            Shuffle = MelonPrefs.GetBool(Category, nameof(Shuffle));

            MelonPrefs.RegisterBool(Category, nameof(ShowScores), false, "", false);
            ShowScores = MelonPrefs.GetBool(Category, nameof(ShowScores));

            MelonPrefs.RegisterBool(Category, nameof(NoFail), false, "", false);
            NoFail = MelonPrefs.GetBool(Category, nameof(NoFail));

            MelonPrefs.RegisterBool(Category, nameof(ResetHealth), false, "", false);
            ResetHealth = MelonPrefs.GetBool(Category, nameof(ResetHealth));

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

        public static void UpdateSongCount(int newCount)
        {
            MelonPrefs.SetInt(Category, nameof(LastSongCount), newCount);
            LastSongCount = newCount;
        }

        public static void UpdateRandomSongBagSize(int newSize)
        {
            MelonPrefs.SetInt(Category, nameof(RandomSongBagSize), newSize);
            RandomSongBagSize = newSize;
        }
    }
}
