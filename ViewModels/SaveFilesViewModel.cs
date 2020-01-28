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
        private RelayCommand _addCommand;
        private RelayCommand _removeCommand;
        private RelayCommand _addSaveFile;
        private IDialogService dialogService = new DefaultDialogService();

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
                OnPropertyChanged(nameof(isReplayFilesAvailable));
            }
        }

        public bool isReplayFilesAvailable
        {
            get => ReplayFiles.Count > 0 && SelectedSaveFile != null;
        }

        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ??
                       (_addCommand = new RelayCommand(obj =>
                       {
                           var saveFile = new SaveFile(GetMaxId() + 1, "New save", "New save file");
                           SaveFiles.Add(saveFile);
                           SelectedSaveFile = saveFile;
                       }));
            }
        }

        public RelayCommand AddSaveFile {
            get {
                return _addSaveFile ??
                       (_addSaveFile = new RelayCommand(obj =>
                           {
                               if (obj is SaveFile saveFile)
                               {
                                   dialogService.OpenFileDialog();
                                   try
                                   {
                                       File.Copy(dialogService.FilePath, saveFile.Location);
                                   }
                                   catch (Exception e)
                                   {
                                       MessageBox.Show(e.Message);
                                       throw;
                                   }
                               }
                           }, (obj) => SaveFiles.Count > 0
                       ));
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
                                       MessageBox.Show(e.Message);
                                       throw;
                                   }
                               }
                           }, (obj) => SaveFiles.Count > 0
                           ));
            }
        }

        private int GetMaxId()
        {
            return SaveFiles.Select(saveFile => saveFile.Id).Concat(new[] {0}).Max() ;
        }


        public event PropertyChangedEventHandler PropertyChanged;


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
