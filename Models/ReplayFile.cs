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

        public string Location => Path.Combine(GlobalConfig.GetReplayFilesPath(), $"{Id}.b");

        public ReplayFile(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
