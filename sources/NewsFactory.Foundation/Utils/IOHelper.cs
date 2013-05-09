using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;

namespace NewsFactory.Foundation.Utils
{
    public static class IOHelper
    {
        #region Methods

        public async static Task<StorageFile> GetFileOrDefault(this StorageFolder folder, string fileName)
        {
            var files = await folder.GetFilesAsync();
            return files.FirstOrDefault(f => f.Name == fileName);
        }

        public async static Task Move(this StorageFolder folder, string from, string to)
        {
            var fileFrom = await folder.GetFileOrDefault(from);
            if (fileFrom != null)
            {
                var fileTo = await folder.GetFileOrDefault(to);
                if (fileTo != null)
                {
                    await fileFrom.MoveAndReplaceAsync(fileTo);
                }
                else
                {
                    await fileFrom.MoveAsync(folder, to);
                }
            }
        }

        public async static Task Copy(this StorageFolder folder, string from, string to)
        {
            var fileFrom = await folder.GetFileOrDefault(from);
            if (fileFrom != null)
            {
                var fileTo = await folder.GetFileOrDefault(to);
                if (fileTo != null)
                {
                    await fileFrom.CopyAndReplaceAsync(fileTo);
                }
                else
                {
                    await fileFrom.CopyAsync(folder, to);
                }
            }
        }

        public async static Task<StorageFile> GetFirstFile(this StorageFolder folder, params string[] fileNames)
        {
            var files = await folder.GetFilesAsync();
            foreach (var item in fileNames)
            {
                var file = files.FirstOrDefault(f => f.Name == item);
                if (file != null)
                    return file;
            }
            return null;
        }

        #endregion Methods
    }
}
