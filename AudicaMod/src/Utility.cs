using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AudicaModding
{
    internal static class Utility
    {        public static void EmptyDownloadsFolderFolder()
        {
            String directoryName = Application.dataPath + @"\StreamingAssets\HmxAudioAssets\songs";
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
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
    }
}
