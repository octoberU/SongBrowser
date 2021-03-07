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

        public static void RegisterConfig()
        {
            MelonPrefs.RegisterBool(Category, nameof(SafeSongListReload), true,
                                    "Blocks access to song list during song list reload (safety feature).");

            MelonPrefs.RegisterInt(Category, nameof(LastSongCount), 0, "", true);
            LastSongCount = MelonPrefs.GetInt(Category, nameof(LastSongCount));

            MelonPrefs.RegisterInt(Category, nameof(RandomSongBagSize), 10, "", true);
            RandomSongBagSize = MelonPrefs.GetInt(Category, nameof(RandomSongBagSize));

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
