﻿using System.Collections.Generic;
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

        public string LastMission => ReadLastMission(Location);

        private string ReadLastMission(string path)
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

                    if (counter == 2)
                    {
                        newBytes.RemoveAt(newBytes.Count - 1);
                        break;
                    }
                }
                else
                {
                    counter = 0;
                }
            }
            return Encoding.Unicode.GetString(newBytes.ToArray());
        }

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