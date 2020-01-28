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
            DataContext = new SaveFilesViewModel();
        }
    }
}
