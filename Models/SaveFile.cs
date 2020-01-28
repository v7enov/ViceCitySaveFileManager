using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using ViceCitySaveFileManager.Annotations;

namespace ViceCitySaveFileManager.Models
{
    public class SaveFile : INotifyPropertyChanged
    {
        public int Id { get; }
        private string _name;
        private string _description;
        private string _lastMission;
        private bool _fileExists;
        private ReplayFile _attachedReplayFile;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Location => Path.Combine(GlobalConfig.GetSaveFilesPath(), $"{Id}.b");

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string LastMission => "Mission Name";

        public bool FileExists
        {
            get => File.Exists(Location);
            set
            {
                _fileExists = value;
                OnPropertyChanged(nameof(FileExists));
            }
        }

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
