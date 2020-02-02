using System.Windows;
using ViceCitySaveFileManager.ViewModels;

namespace ViceCitySaveFileManager.Views
{
    /// <summary>
    /// Interaction logic for SaveFilesView.xaml
    /// </summary>
    public partial class SaveFilesView : Window
    {

        public SaveFilesView()
        {
            InitializeComponent();
            Initialize();
            DataContext = new SaveFilesViewModel();
        }

        private void Initialize()
        {
            GlobalConfig.CreateDirectories();
            GlobalConfig.GrantAccess(GlobalConfig.GetVCSaveFilesDirectory());
        }
    }
}
