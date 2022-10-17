using System;


namespace filemanager.src
{
    internal class FileItem
    {
        internal protected string FilePath { get; set; } = "";
        internal protected string FileName { get; set; } = "";
        internal protected bool ExistsInSource { get; set; } = false;
        internal protected bool ExistsInTarget { get; set; } = false;

        protected internal FileItem(FileRequest pFileRequest)
        {
            this.FilePath = pFileRequest.ImageName;
            this.ExistsInSource = this.CheckIfSourceExists();
            this.ExistsInTarget = this.CheckIfTargetExists();
            this.FileName = Path.GetFileName(this.FilePath);
        }

        protected internal bool CheckIfSourceExists()
        {

            if (File.Exists(this.FilePath))
            {
                this.ExistsInSource = true;
                return true;
            }

            return false;
        }

        protected internal bool CheckIfTargetExists()
        {
            FileJob fileJob = FileJob.Instance;
            if (File.Exists(String.Concat(fileJob.TargetDirectory, "\\", this.FileName)))
            {
                this.ExistsInTarget = true;
                return true;
            }
            return false;
        }

    }
}