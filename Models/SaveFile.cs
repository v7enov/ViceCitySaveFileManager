using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using ViceCitySaveFileManager.Annotations;
using ViceCitySaveFileManager.ViewModels;

namespace ViceCitySaveFileManager.Models
{
    public class SaveFile : INotifyPropertyChanged
    {
        public int Id { get; private set; }
        private string _name;
        private string _description;
        private ReplayFile _attachedReplayFile;
        private string _lastMission;
        private bool _fileExists;
        private int _attachedReplayFileId;

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
            if (!File.Exists(path)) return "";
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

        public static void WriteInGameName(string path, string str)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Open)))
            {
                writer.BaseStream.Seek(4, SeekOrigin.Begin);
                int length = Math.Min(str.Length, 23);
                // Пробегаемся по символам строки
                for (int i = 0; i < length; ++i)
                {
                    // Записываем символ в файл
                    writer.Write(Convert.ToUInt16(str[i]));
                }
                // Добавляем завершающий символ с кодом 0x0000
                writer.Write(Convert.ToUInt16('\0'));
            }
            FixCheckSum(path);
        }

        public static void FixCheckSum(string path)
        {
            uint checkSum = 0;
            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                // Суммируем все байты
                while (reader.BaseStream.Position < reader.BaseStream.Length && reader.BaseStream.Position < 201824)
                {
                    checkSum += reader.ReadByte();
                }
            }
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Open)))
            {
                // Сохраняем новую чек-сумму
                writer.BaseStream.Seek(201824, SeekOrigin.Begin);
                writer.Write(checkSum);
            }
        }

        public bool FileExists
        {
            get
            {
                _fileExists = File.Exists(Location);
                return _fileExists;
            }
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

        public SaveFile()
        {

        }

        public ReplayFile AttachedReplayFile
        {
            get => _attachedReplayFile;
            set
            {
                _attachedReplayFile = value;
                if (value != null) _attachedReplayFileId = value.Id;
                OnPropertyChanged(nameof(AttachedReplayFile));
            }
        }

        public int AttachedReplayFileId 
        {
            get 
            {
                return _attachedReplayFileId;
            } 
            set 
            {
                _attachedReplayFileId = value;
                OnPropertyChanged(nameof(AttachedReplayFileId));
            }
        }

        public void UpdateState()
        {
            LastMission = ReadLastMission(Location);
            FileExists = File.Exists(Location);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
