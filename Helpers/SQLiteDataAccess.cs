using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViceCitySaveFileManager.Models;

namespace ViceCitySaveFileManager.Helpers
{
    public class SQLiteDataAccess
    {
        public static List<SaveFile> LoadSaveFiles()
        {
            using (var cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<SaveFile>("select Id, Name, Description, AttachedReplayFileId from SaveFiles ", new DynamicParameters());

                var attachedReplays = cnn.Query<ReplayFile>("SELECT ReplayFiles.* FROM ReplayFiles INNER JOIN SaveFiles ON ReplayFiles.Id = SaveFiles.AttachedReplayFileId", new DynamicParameters());

                var saveFiles = output as SaveFile[] ?? output.ToArray();
                foreach (var replay in attachedReplays)
                {
                    foreach (var save in saveFiles.Where(x => x.AttachedReplayFileId == replay.Id))
                    { 
                         save.AttachedReplayFile = replay;
                    }
                }

                return saveFiles.ToList();
            }
        }

        public static List<SaveSlot> LoadSaveSlots()
        {
            using (var cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<SaveSlot>("select SlotNumber, AttachedSaveFileId from SaveSlots ", new DynamicParameters());
                var attachedSaveFiles = cnn.Query<SaveFile>("SELECT SaveFiles.* FROM SaveFiles INNER JOIN SaveSlots ON SaveSlots.AttachedSaveFileId = SaveFiles.Id", new DynamicParameters());
                var saveSlots = output as SaveSlot[] ?? output.ToArray();

                foreach (var saveFile in attachedSaveFiles)
                {
                    foreach (var slot in saveSlots.Where(x => x.AttachedSaveFileId == saveFile.Id))
                    {
                        slot.AttachedSaveFile = saveFile;
                    }
                }

                return saveSlots.ToList();
            }
        }

        public static List<ReplayFile> LoadReplayFiles()
        {
            using (var cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ReplayFile>("select * from ReplayFiles", new DynamicParameters());
                return output.ToList();
            }
        }

        public static void Save(List<SaveFile> saveFiles, List<ReplayFile> replayFiles, List<SaveSlot> saveSlots)
        {
            using (var cnn = new SQLiteConnection(LoadConnectionString()))
            {
                //cnn.Execute("insert into SaveFiles (Name, Description, AttachedReplayFile) values (@Name, @Description, @ AttachedReplayFile)", saveFile);
                cnn.Execute("delete from SaveFiles");
                foreach (var item in saveFiles)
                {
                    cnn.Execute("insert into SaveFiles (Id, Name, Description, AttachedReplayFileId) values (@Id, @Name, @Description, @AttachedReplayFileId)", item);
                }

                cnn.Execute("delete from ReplayFiles");
                foreach (var item in replayFiles)
                {
                    cnn.Execute("insert into ReplayFiles (Id, Name, Description) values (@Id, @Name, @Description)", item);
                }

                cnn.Execute("delete from SaveSlots");
                foreach (var item in saveSlots)
                {
                    cnn.Execute("insert into SaveSlots (SlotNumber, AttachedSaveFileId) values (@SlotNumber, @AttachedSaveFileId)", item);
                }
            }
        }

        public static int SelectMaxId(bool isSaveFiles)
        {
            switch (isSaveFiles)
            {
                case true:
                    using (var cnn = new SQLiteConnection(LoadConnectionString()))
                    {
                        return cnn.Query<int>("SELECT MAX(id) FROM SaveFiles").FirstOrDefault();
                    }
                case false:
                    using (var cnn = new SQLiteConnection(LoadConnectionString()))
                    {
                        return cnn.Query<int>("SELECT MAX(id) FROM ReplayFiles").SingleOrDefault();
                    }
            }
            return 0;
        }


        public static string LoadConnectionString(string name = "Default")
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}
