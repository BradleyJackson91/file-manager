using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filemanager.src
{
    internal class SearchResult
    {
        protected internal string FileName { get; set; } = string.Empty;
        protected internal string FilePath { get; set; } = string.Empty;
        protected internal string OutputName { get; set; } = string.Empty;

        internal SearchResult(string filePath)
        {
            this.FileName = Path.GetFileName(filePath);
            this.FilePath = filePath;
        }   

        protected internal void SetOutputName()
        {
            // fileName__path_to_file.extension
            OutputName = String.Concat(Path.GetFileNameWithoutExtension(this.FilePath), "__", Path.GetDirectoryName(this.FilePath).Replace("\\", "_").Replace(" ", "").Replace(".", "").Replace(":", ""), Path.GetExtension(this.FilePath));
        }
    }
}
