using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using ViceCitySaveFileManager.Annotations;

namespace ViceCitySaveFileManager.Models
{
    public class ReplayFile : INotifyPropertyChanged
    {
        private string _name;
        private int _id;
        private string _description;
        private bool _fileExists;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Name
        {
            get => _name;
            set 
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
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

        public ReplayFile()
        {

        }

        public string Location => Path.Combine(GlobalConfig.GetReplayFilesPath(), $"{Id}.rep");

        public ReplayFile(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public bool FileExists {
            get {
                _fileExists = File.Exists(Location);
                return _fileExists;
            }
            set {
                _fileExists = value;
                OnPropertyChanged(nameof(FileExists));
            }
        }

        public void UpdateState()
        {
            FileExists = File.Exists(Location);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ReplayFile)) return false;
            return (Id == ((ReplayFile)obj).Id);
        }

        public override string ToString()
        {
            return Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
