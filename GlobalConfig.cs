using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace ViceCitySaveFileManager
{
    public static class GlobalConfig
    {
        private static readonly string UserFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        private const string SaveFiles = "SaveFiles";
        private const string ReplayFiles = "ReplayFiles";
        private const string ExportFolder = "Export";

        private static string GetApplicationBasePath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static void GrantAccess(string fullPath)
        {
            if (!Directory.Exists(fullPath)) return;
            var dInfo = new DirectoryInfo(fullPath);
            var dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);

            foreach (var item in Directory.GetFiles(fullPath))
            {
                var fi = new FileInfo(item);
                if (fi.IsReadOnly) fi.IsReadOnly = false;
            }
        }

        public static string GetSaveFilesPath()
        {
            return Path.Combine(GetApplicationBasePath() + SaveFiles);
        }

        public static string GetVCSaveFilesDirectory()
        {
            return Path.Combine(UserFolder, "Documents", "GTA Vice City User Files");
        }

        public static string GetReplayFilesPath()
        {
            return Path.Combine(GetApplicationBasePath() + ReplayFiles);
        }

        public static void CreateDirectories()
        {
            if (!Directory.Exists(GetReplayFilesPath())) Directory.CreateDirectory(GetReplayFilesPath());
            if (!Directory.Exists(GetSaveFilesPath())) Directory.CreateDirectory(GetSaveFilesPath());
        }

        public static string GetDbFile()
        {
            return GetApplicationBasePath() + "vcdb.db";
        }

        public static string GetExportFolder()
        {
            return Path.Combine(GetApplicationBasePath() + ExportFolder);
        }
    }
}
