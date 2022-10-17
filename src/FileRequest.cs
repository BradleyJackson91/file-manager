using System;
using System.Text;


namespace filemanager.src
{

    internal enum Status { ToProcess, ToSkip, ToOverwrite, Incomplete, Missing, Skipped, Overwritten, Copying, Error, Copied }

    internal class FileRequest
    {
        // This class will hold and process the data of the input csv file of files to be copied.

        protected internal string CSVLine { get; set; } = "";
        protected internal string InternalID { get; set; } = "";
        protected internal string Name { get; set; } = "";
        protected internal string DisplayName { get; set; } = "";
        protected internal string ImageName { get; set; } = "";
        protected internal Status Status { get; set; } = Status.ToProcess;
        protected internal Exception? Exception { get; set; }
        protected internal FileItem? FileItem { get; set; }

        internal FileRequest(string pCSVLine)
        {
            this.CSVLine = pCSVLine;
            this.CSVBreakDown();
            this.FileItem = new FileItem(this);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Internal ID: {0}, Name: {1}, DisplayName: {2}, Image Name: {3}, Status: {4}", this.InternalID, this.Name, this.DisplayName, this.ImageName, this.Status);
            return sb.ToString();
        }
        internal static string CSVFirstLine()
        {
            return (String.Concat("Internal ID,", "Name,", "Display Name,", "Image Name,", "Status,", "Exception"));
        }
        internal static string CSVTemplateFirstLine()
        {
            return (String.Concat("Internal ID,", "Name,", "Display Name,", "Image Name"));
        }
        protected internal string ToCSVString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}, {1}, {2}, {3}, {4}, {5}", this.InternalID, this.Name, this.DisplayName, this.ImageName, this.Status.ToString(), (this.Exception.Message ?? ""));
            return sb.ToString();
        }

        internal static bool CSVValidateFirstLine(string pFirstLine)
        {
            List<string> values = pFirstLine.Split(",").ToList();

            if (!values[0].Replace(" ", "").ToUpper().Equals("INTERNALID"))
            {
                return false;
            }
            if (!values[1].Replace(" ", "").ToUpper().Equals("NAME"))
            {
                return false;
            }
            if (!values[2].Replace(" ", "").ToUpper().Equals("DISPLAYNAME"))
            {
                return false;
            }
            if (!values[3].Replace(" ", "").ToUpper().Equals("IMAGENAME"))
            {
                return false;
            }

            return true;
        }

        internal protected void CSVBreakDown()
        {
            try
            {

                //TODO: need to check the FileRequests for records without values in columns
                List<string> values = this.CSVLine.Split(",").ToList();
                this.InternalID = values[0];
                this.Name = values[1];
                this.DisplayName = values[2];
                this.ImageName = values[3];
                this.Status = Status.ToProcess;

                if (this.ImageName.Trim().Equals(""))
                {
                    this.Status = Status.Incomplete;
                }
            }
            catch (Exception ex)
            {
                this.Status = Status.Error;
                this.Exception = ex;
            }
        }

        internal protected void CopyFile()
        {
            FileJob fileJob = FileJob.Instance;

            if (this.Status.Equals(Status.Error))
            {
                return;
            }

            if (this.Status.Equals(Status.Incomplete))
            {
                return;
            }

            if (!this.FileItem.ExistsInSource)
            {
                this.Status = Status.Missing;
                return;
            }

            if (this.Status.Equals(Status.ToSkip) && this.FileItem.CheckIfTargetExists())
            {
                this.Status = Status.Skipped;
                return;
            }

            try
            {
                this.Status = Status.Copying;
                File.Copy(this.FileItem.FilePath, String.Concat(fileJob.TargetDirectory, "\\", this.FileItem.FileName), this.Status.Equals(Status.ToOverwrite));
                this.Status = Status.Copied;
            }
            catch (Exception ex)
            {
                this.Status = Status.Error;
                this.Exception = ex;
                //Console.WriteLine("Error: The following exception has been caught:");
                //Console.WriteLine(String.Concat("File Name: ", this.FileItem.FileName));
                //Console.WriteLine(String.Concat("Error Message: ", ex.Message));
            }
        }
    }
}