using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ViceCitySaveFileManager.Annotations;

namespace ViceCitySaveFileManager.Models
{
    public class SaveSlot : INotifyPropertyChanged
    {
        private static readonly List<string> Names = new List<string>()
        {
            "GTAVCsf1.b",
            "GTAVCsf2.b",
            "GTAVCsf3.b",
            "GTAVCsf4.b",
            "GTAVCsf5.b",
            "GTAVCsf6.b",
            "GTAVCsf7.b",
            "GTAVCsf8.b"
        };

        const int BYTES_TO_READ = sizeof(Int64);

        private SaveFile _attachedSaveFile;
        private int _attachedSaveFileId;
        private bool _saveFileEquals;
        public int SlotNumber { get; set; }

        public SaveFile AttachedSaveFile
        {
            get => _attachedSaveFile;
            set 
            {
                _attachedSaveFile = value;
                if (AttachedSaveFile != null) _attachedSaveFileId = AttachedSaveFile.Id;
                OnPropertyChanged(nameof(AttachedSaveFile));
            }
        }

        public int AttachedSaveFileId
        {
            get => _attachedSaveFileId;
            set
            {
                _attachedSaveFileId = value;
                OnPropertyChanged(nameof(AttachedSaveFileId));
            }
        }

        public string FileName => Path.Combine(GlobalConfig.GetVCSaveFilesDirectory(), Names[SlotNumber - 1]);

        public string ReplayFileName => Path.Combine(GlobalConfig.GetVCSaveFilesDirectory(), "replay.rep");

        public bool SaveFileEquals
        {
            get {
                if (AttachedSaveFile == null) return false;
                    return FilesAreEqual(new FileInfo(FileName), new FileInfo(AttachedSaveFile.Location));
            }
            set
            {
                _saveFileEquals = value;
                OnPropertyChanged(nameof(SaveFileEquals));
            }
        } 

        public SaveSlot()
        {

        }

        public SaveSlot(int slotNumber, SaveFile save)
        {
            SlotNumber = slotNumber;
            AttachedSaveFile = save;
        }

        private static bool FilesAreEqual(FileInfo first, FileInfo second)
        {
            if (first.Length != second.Length)
                return false;

            if (string.Equals(first.FullName, second.FullName, StringComparison.OrdinalIgnoreCase))
                return true;

            int iterations = (int)Math.Ceiling((double)first.Length / BYTES_TO_READ);

            using (FileStream fs1 = first.OpenRead())
            using (FileStream fs2 = second.OpenRead())
            {
                byte[] one = new byte[BYTES_TO_READ];
                byte[] two = new byte[BYTES_TO_READ];

                for (int i = 0; i < iterations; i++)
                {
                    fs1.Read(one, 0, BYTES_TO_READ);
                    fs2.Read(two, 0, BYTES_TO_READ);

                    if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                        return false;
                }
            }

            return true;
        }

        public void UpdateState()
        {
            SaveFileEquals = FilesAreEqual(new FileInfo(FileName), new FileInfo(AttachedSaveFile.Location));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
