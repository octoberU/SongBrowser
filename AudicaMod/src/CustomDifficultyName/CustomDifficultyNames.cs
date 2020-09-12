using Il2CppSystem;
using MelonLoader;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using MelonLoader.TinyJSON;

namespace AudicaModding
{
    public class CustomDifficultyNames : MelonMod
    {
        public static Dictionary<string, CustomNameData> CustomDifficulties = new Dictionary<string, CustomNameData>();

        public class CustomNameData
        {
            public bool hasCustomExpertName = false;
            public bool hasCustomAdvancedName = false;
            public bool hasCustomModerateName = false;
            public bool hasCustomBeginnerName = false;


            public string customExpert = "";
            public string customAdvanced = "";
            public string customModerate = "";
            public string customBeginner = "";
        }

        public static void LoadCustomNames()
        {
            CustomDifficulties = new Dictionary<string, CustomNameData>();

            foreach (SongList.SongData data in SongList.I.songs.ToArray())
            {
                ZipArchive SongFiles = ZipFile.OpenRead(data.foundPath);

                foreach (ZipArchiveEntry entry in SongFiles.Entries)
                {
                    if (entry.Name == "song.desc")
                    {
                        Stream songData = entry.Open();
                        StreamReader reader = new StreamReader(songData);
                        string descDump = reader.ReadToEnd();
                        CustomNameData DifficultyNames = new CustomNameData();
                        DifficultyNames = JSON.Load(descDump).Make<CustomNameData>();

                        bool anyAreTrue = false;


                        if (DifficultyNames.customExpert.Length > 0)
                        {
                            DifficultyNames.hasCustomExpertName = true;
                            anyAreTrue = true;
                        }

                        if (DifficultyNames.customAdvanced.Length > 0)
                        {
                            DifficultyNames.hasCustomAdvancedName = true;
                            anyAreTrue = true;
                        }

                        if (DifficultyNames.customModerate.Length > 0)
                        {
                            DifficultyNames.hasCustomModerateName = true;
                            anyAreTrue = true;
                        }

                        if (DifficultyNames.customBeginner.Length > 0)
                        {
                            DifficultyNames.hasCustomBeginnerName = true;
                            anyAreTrue = true;
                        }

                        if (anyAreTrue)
                            CustomDifficulties[data.songID] = DifficultyNames;


                    }
                }

                SongFiles.Dispose();
            }
        }
    }
}
