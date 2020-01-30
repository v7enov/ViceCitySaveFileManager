using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using ViceCitySaveFileManager.Annotations;
using ViceCitySaveFileManager.Models;
using ViceCitySaveFileManager.Helpers;

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
        private RelayCommand _saveToDbCommand;
        private RelayCommand _deattachSaveFile;
        private RelayCommand _deselectReplayFile;
        private RelayCommand _deattachReplayFile;
        private RelayCommand _removeReplayRecord;
        private RelayCommand _moveToSlot;
        private bool _isSaved;
        private readonly IDialogService _dialogService = new DefaultDialogService();

        public TrulyObservableCollection<SaveFile> SaveFiles { get; set; }
        public TrulyObservableCollection<ReplayFile> ReplayFiles { get; set; }
        public TrulyObservableCollection<SaveSlot> SaveSlots { get; set; }


        public SaveFilesViewModel()
        {
            SaveFiles = new TrulyObservableCollection<SaveFile>(SQLiteDataAccess.LoadSaveFiles());
            ReplayFiles = new TrulyObservableCollection<ReplayFile>(SQLiteDataAccess.LoadReplayFiles());
            SaveSlots = new TrulyObservableCollection<SaveSlot>(SQLiteDataAccess.LoadSaveSlots());

            ReplayFiles.CollectionChanged += OnCollectionChanged;
            SaveFiles.CollectionChanged += OnCollectionChanged;
            SaveSlots.CollectionChanged += OnCollectionChanged;
            _isSaved = true;
        }

        public SaveFile SelectedSaveFile
        {
            get => _selectedSaveFile;
            set
            {
                _selectedSaveFile = value;
                OnPropertyChanged(nameof(SelectedSaveFile));
                OnPropertyChanged(nameof(IsReplayFilesAvailable));
                OnPropertyChanged(nameof(IsMoveAvailable));
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

        public bool IsMoveAvailable => SelectedSaveFile != null && SelectedSaveFile.FileExists;

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
                           }, (obj) => SaveFiles.Count > 0 && SelectedSaveFile != null
                       ));
            }
        }

        public RelayCommand DeattachSaveFile {
            get {
                return _deattachSaveFile ??
                       (_deattachSaveFile = new RelayCommand(obj =>
                           {
                               if (obj is SaveFile saveFile)
                               {
                                   try
                                   {
                                       File.Delete(saveFile.Location);
                                   }
                                   catch (Exception e)
                                   {
                                       _dialogService.ShowMessage(e.Message);
                                       throw;
                                   }
                                   finally
                                   {
                                       saveFile.UpdateState();
                                   }
                               }
                           }, (obj) => SaveFiles.Count > 0 && SelectedSaveFile != null && SelectedSaveFile.FileExists 
                       ));
            }
        }

        public RelayCommand DeselectReplayFile {
            get {
                return _deselectReplayFile ??
                       (_deselectReplayFile = new RelayCommand(obj =>
                           {
                               if (obj is SaveFile saveFile)
                               {
                                   saveFile.AttachedReplayFile = null;
                                   saveFile.AttachedReplayFileId = default;
                               }
                           }, (obj) => SaveFiles.Count > 0 && SelectedSaveFile != null && SelectedSaveFile.AttachedReplayFile != null
                       ));
            }
        }

        public RelayCommand RemoveReplayRecord {
            get {
                return _removeReplayRecord ??
                       (_removeReplayRecord = new RelayCommand(obj =>
                           {
                               if (obj is ReplayFile replayFile)
                               {
                                   ReplayFiles.Remove(replayFile);
                                   try
                                   {
                                       File.Delete(replayFile.Location);
                                   }
                                   catch (Exception e)
                                   {
                                       _dialogService.ShowMessage(e.Message);
                                       throw;
                                   }
                               }
                           }, (obj) => ReplayFiles.Count > 0 && SelectedReplayFile != null
                       ));
            }
        }

        public RelayCommand DeattachReplayFile {
            get {
                return _deattachReplayFile ??
                       (_deattachReplayFile = new RelayCommand(obj =>
                           {
                               if (obj is ReplayFile replayFile)
                               {
                                   try
                                   {
                                       File.Delete(replayFile.Location);
                                   }
                                   catch (Exception e)
                                   {
                                       _dialogService.ShowMessage(e.Message);
                                       throw;
                                   }
                                   finally
                                   {
                                       replayFile.UpdateState();
                                   }
                               }
                           }, (obj) => ReplayFiles.Count > 0 && SelectedReplayFile != null && SelectedReplayFile.FileExists
                       ));
            }
        }

        public RelayCommand SaveToDbCommand {
            get {
                return _saveToDbCommand ??
                      (_saveToDbCommand = new RelayCommand(obj =>
                      {
                          {
                              Task.Factory.StartNew(() =>
                                  SQLiteDataAccess.Save(SaveFiles.ToList(), ReplayFiles.ToList(), SaveSlots.ToList())
                                  );
                              _isSaved = true;
                          }
                      }, (obj) => !_isSaved));
            }
        }

        public RelayCommand MoveToSlot {
            get {
                return _moveToSlot ??
                      (_moveToSlot = new RelayCommand(obj =>
                      {
                          {
                              foreach (var slot in SaveSlots.Where(x => x.SlotNumber == int.Parse(obj.ToString())))
                              {
                                  slot.AttachedSaveFile = SelectedSaveFile;
                                  slot.AttachedSaveFileId = SelectedSaveFile.Id;
                                  SaveFile.WriteIngameName(SelectedSaveFile.Location, SelectedSaveFile.Name);

                                  try
                                  {
                                      File.Copy(SelectedSaveFile.Location, slot.FileName, true);
                                  }
                                  catch (Exception e)
                                  {
                                      _dialogService.ShowMessage(e.Message);
                                  }
                                  finally
                                  {
                                      slot.UpdateState();
                                  }

                                  if (SelectedSaveFile.AttachedReplayFile != null)
                                  {
                                      if (SelectedSaveFile.AttachedReplayFile.FileExists)
                                      {
                                          try
                                          {
                                              File.Copy(SelectedSaveFile.AttachedReplayFile.Location, slot.ReplayFileName, true);
                                          }
                                          catch (Exception e)
                                          {
                                              _dialogService.ShowMessage(e.Message);
                                          }
                                          finally
                                          {
                                              slot.UpdateState();
                                          }
                                      }
                                  }
                              }
                          }
                      }));
            }
        }

        private int GetMaxId(bool isSavefiles)
        {
            switch (isSavefiles)
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

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _isSaved = false;
        }


        public event PropertyChangedEventHandler PropertyChanged;


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
