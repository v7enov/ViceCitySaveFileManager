using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViceCitySaveFileManager.Models;

namespace ViceCitySaveFileManager.ViewModels
{
    public interface IFileService
    {
        List<SaveFile> Open(string filename);
        void Save(string filename, List<SaveFile> saveFilesList);
    }
}
