using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ViceCitySaveFileManager
{
    public static class GlobalConfig
    {
        private static readonly string UserFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        private static readonly string SaveFiles = "SaveFiles";
        private static readonly string ReplayFiles = "ReplayFiles";
        private static string GetApplicationBasePath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string GetSaveFilesPath()
        {
            return Path.Combine(GetApplicationBasePath() + SaveFiles);
        }

        public static string GetVCSaveFilesDirectory()
        {
            return Path.Combine(UserFolder, "Documents", "Vice City User Files");
        }

        public static string GetReplayFilesPath()
        {
            return Path.Combine(GetApplicationBasePath() + ReplayFiles);
        }
    }
}
