using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using ViceCitySaveFileManager.Annotations;

namespace ViceCitySaveFileManager.Models
{
    public class SaveFile : INotifyPropertyChanged
    {
        public int Id { get; }
        private string _name;
        private string _description;
        private ReplayFile _attachedReplayFile;
        private string _lastMission;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Location
        {
            get
            {
                return Path.Combine(GlobalConfig.GetSaveFilesPath(), $"{Id}.b");
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string LastMission
        {
            get
            {
                _lastMission = ReadLastMission(Location);
                return _lastMission;
            }
            set
            {
                _lastMission = value;
                OnPropertyChanged(nameof(LastMission));
            } 
        }

        public static string ReadLastMission(string path)
        {
            var allBytes = File.ReadAllBytes(path);
            var newBytes = new List<byte>();
            var counter = 0;

            for (var i = 0; i < allBytes.Length; ++i)
            {
                if (i <= 3) continue;

                newBytes.Add(allBytes[i]);

                if (allBytes[i] == 0)
                {
                    counter++;

                    if (counter != 2) continue;
                    newBytes.RemoveAt(newBytes.Count - 1);
                    break;
                }

                counter = 0;
            }
            return Encoding.Unicode.GetString(newBytes.ToArray());
        }

        public bool FileExists => File.Exists(Location);

        public SaveFile(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public ReplayFile AttachedReplayFile
        {
            get => _attachedReplayFile;
            set
            {
                _attachedReplayFile = value;
                OnPropertyChanged(nameof(AttachedReplayFile));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
