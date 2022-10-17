using System;


namespace filemanager.src
{
    internal class FMFile
    {

        protected internal string FileName { get; set; } = "";
        protected internal string FilePath { get; set; } = "";

        internal FMFile(string pFilePath, string pFileName)
        {
            this.FilePath = pFilePath;
            this.FileName = pFileName;
        }

    }
}