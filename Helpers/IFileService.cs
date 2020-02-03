using System.Collections.Generic;
using ViceCitySaveFileManager.Models;

namespace ViceCitySaveFileManager.ViewModels
{
    public interface IFileService
    {
        List<SaveFile> Open(string filename);
        void Save(string filename, List<SaveFile> saveFilesList);
    }
}
