

namespace ViceCitySaveFileManager.ViewModels
{
    public interface IDialogService
    {
        void ShowMessage(string message);  
        string FilePath { get; set; }  
        bool OpenFileDialog();  
        bool SaveFileDialog();  
    }
}
