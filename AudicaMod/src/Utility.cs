using MelonLoader.TinyJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace AudicaModding
{
    internal static class Utility
    {
        public static void EmptyDownloadsFolder()
        {
            String directoryName = Application.dataPath + @"\StreamingAssets\HmxAudioAssets\songs";
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            // first remove all files that were marked for deletion by the user
            if (File.Exists(SongBrowser.deletedDownloadsListPath))
            {
                string       text    = File.ReadAllText(SongBrowser.deletedDownloadsListPath);
                List<string> deleted = JSON.Load(text).Make<List<string>>();

                foreach (string fileName in deleted)
                {
                    string path = Path.Combine(SongBrowser.downloadsDirectory, fileName);
                    try
                    {
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        else // could also be located in the main directory
                        {
                            path = Path.Combine(SongBrowser.mainSongDirectory, fileName);
                            if (File.Exists(path))
                            {
                                File.Delete(path);
                            }
                        }
                    }
                    catch
                    {
                        MelonLoader.MelonLogger.LogWarning($"Unable to delete {fileName}");
                    }
                }
                File.Delete(SongBrowser.deletedDownloadsListPath);
            }

            var dirInfo = new DirectoryInfo(directoryName);
            List<String> AudicaFiles = Directory
                               .GetFiles(SongBrowser.downloadsDirectory, "*.*", SearchOption.TopDirectoryOnly).ToList();
            foreach (string file in AudicaFiles)
            {
                FileInfo audicaFile = new FileInfo(file);
                if (new FileInfo(dirInfo + "\\" + audicaFile.Name).Exists == false)
                {
                    audicaFile.MoveTo(dirInfo + "\\" + audicaFile.Name);
                }
                else
                {
                    File.Delete(file);
                }
            }
            SongBrowser.emptiedDownloadsFolder = true;
        }

        public static bool IsCustomSong(string songID)
        {
            string[] components = songID.Split('_');
            if (components.Length == 1) // only official songs don't have a hash in their ID
                return false;

            string potentialHash = components[components.Length - 1];

            // hash is always 32 characters long and only contains (lowercase) hex characters
            if (potentialHash.Length == 32 && Regex.IsMatch(potentialHash, @"^[0-9a-f]+$"))
                return true;

            return false;
        }
    }
}
