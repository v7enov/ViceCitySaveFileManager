using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using ViceCitySaveFileManager.Annotations;
using ViceCitySaveFileManager.Models;

namespace ViceCitySaveFileManager.ViewModels
{
    public class SaveFilesViewModel : INotifyPropertyChanged
    {
        private SaveFile _selectedSaveFile;
        private ReplayFile _selectedReplayFile;
        private RelayCommand _addCommand;
        private RelayCommand _removeCommand;
        private RelayCommand _addSaveFile;
        private RelayCommand _attachReplayFile;
        private RelayCommand _addReplayRecord;
        private readonly IDialogService _dialogService = new DefaultDialogService();

        public ObservableCollection<SaveFile> SaveFiles { get; set; }
        public ObservableCollection<ReplayFile> ReplayFiles { get; set; }


        public SaveFilesViewModel()
        {
            SaveFiles = new ObservableCollection<SaveFile>
            {
                new SaveFile(1, "asdasd", "asdasdad"),
                new SaveFile(2, "asdasdas", "asdasdasdasdasd")
            };

            ReplayFiles = new ObservableCollection<ReplayFile>
            {
                new ReplayFile(1, "asd", "asd"),
                new ReplayFile(2, "asdasd", "asd")
            };
        }

        public SaveFile SelectedSaveFile
        {
            get => _selectedSaveFile;
            set
            {
                _selectedSaveFile = value;
                OnPropertyChanged(nameof(SelectedSaveFile));
                OnPropertyChanged(nameof(IsReplayFilesAvailable));
            }
        }

        public ReplayFile SelectedReplayFile {
            get => _selectedReplayFile;
            set {
                _selectedReplayFile = value;
                OnPropertyChanged(nameof(SelectedReplayFile));
            }
        }

        public bool IsReplayFilesAvailable => ReplayFiles.Count > 0 && SelectedSaveFile != null;

        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ??
                       (_addCommand = new RelayCommand(obj =>
                       {
                           var saveFile = new SaveFile(GetMaxId(true) + 1, "New save", "New save file");
                           SaveFiles.Add(saveFile);
                           SelectedSaveFile = saveFile;
                       }));
            }
        }

        public RelayCommand AddSaveFile
        {
            get
            {
                return _addSaveFile ??
                       (_addSaveFile = new RelayCommand(obj =>
                           {
                               if (obj is SaveFile saveFile)
                               {
                                   if (!_dialogService.OpenFileDialog()) return;
                                   try
                                   {
                                       File.Copy(_dialogService.FilePath, saveFile.Location);
                                   }
                                   catch (Exception e)
                                   {
                                       MessageBox.Show(e.Message);
                                       throw;
                                   }
                                   finally
                                   {
                                       UpdateState();
                                   }
                               }
                           }, (obj) => SelectedSaveFile != null && SaveFiles.Count > 0 && !SelectedSaveFile.FileExists
                       ));
            }
        }

        public RelayCommand AttachReplayFile {
            get {
                return _attachReplayFile ??
                       (_attachReplayFile = new RelayCommand(obj =>
                       {
                           if (obj is ReplayFile replayFile)
                           {
                               if (!_dialogService.OpenFileDialog()) return;
                               try
                               {
                                   File.Copy(_dialogService.FilePath, replayFile.Location);
                               }
                               catch (Exception e)
                               {
                                   MessageBox.Show(e.Message);
                                   throw;
                               }
                               finally
                               {
                                   UpdateState();
                               }
                           }
                       }, (obj) => SelectedReplayFile != null && ReplayFiles.Count > 0 && !SelectedReplayFile.FileExists
                       ));
            }
        }

        public RelayCommand AddReplayRecord {
            get {
                return _addReplayRecord ??
                       (_addReplayRecord = new RelayCommand(obj =>
                       {
                           var replayFile = new ReplayFile(GetMaxId(false) + 1, "New replay", "New replay file");
                           ReplayFiles.Add(replayFile);
                           SelectedReplayFile = replayFile;
                       }));
            }
        }


        public RelayCommand RemoveCommand
        {
            get
            {
                return _removeCommand ??
                       (_removeCommand = new RelayCommand(obj =>
                           {
                               if (obj is SaveFile saveFile)
                               {
                                   SaveFiles.Remove(saveFile);
                                   try
                                   {
                                       File.Delete(saveFile.Location);
                                   }
                                   catch (Exception e)
                                   {
                                       _dialogService.ShowMessage(e.Message);
                                       throw;
                                   }
                               }
                           }, (obj) => SaveFiles.Count > 0
                       ));
            }
        }

        private int GetMaxId(bool isSavefiles)
        {
            switch(isSavefiles)
            {
                case true:
                    return SaveFiles.Select(saveFile => saveFile.Id).Concat(new[] { 0 }).Max();
                case false:
                    return ReplayFiles.Select(saveFile => saveFile.Id).Concat(new[] { 0 }).Max();
            }
            return 0;
        }

        private void UpdateState()
        {
            foreach (var saveFile in SaveFiles) saveFile.UpdateState();
            foreach (var replayFile in ReplayFiles) replayFile.UpdateState();
        }


        public event PropertyChangedEventHandler PropertyChanged;


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
