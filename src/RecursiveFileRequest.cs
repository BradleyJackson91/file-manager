using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filemanager.src
{
    internal enum RecursiveStatus { ToProcess, Incomplete, Duplicate, NoFilesFound, SingleFileFound, MultipleFilesFound, Copying, Error, Copied, MultipleFilesCopied }

    internal class RecursiveFileRequest
    {
        protected internal string CSVLine { get; set; } = "";
        protected internal string FileName { get; set; } = "";
        protected internal List<SearchResult> SearchResults { get; set; } = new List<SearchResult>();
        protected internal RecursiveStatus Status { get; set; } = RecursiveStatus.ToProcess;
        protected internal Exception? Exception { get; set; }

        internal RecursiveFileRequest(string pCSVLine)
        {
            this.CSVLine = pCSVLine;
            this.CSVBreakDown();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("CSV Line: {0}, File Name: {1}, Search Results Count: {2}, Status: {3}, Exception: {4}",
                this.CSVLine,
                this.FileName,
                this.SearchResults.Count(),
                Enum.GetName(typeof(RecursiveStatus), this.Status),
                this.Exception.Message ?? "");

            return sb.ToString();
        }

        internal static string CSVFirstLine()
        {
            return String.Concat("CSV Line,", "FileName,", "Search,", "Result,", "Count,", "Status,", "Exception");
        }

        internal static string CSVTemplateFirstLine()
        {
            return "Image Name";
        }

        protected internal string ToCSVString()
        {
            return String.Format("{0}, {1}, {2}, {3}, {4}",
                this.CSVLine,
                this.FileName,
                this.SearchResults.Count(),
                Enum.GetName(typeof(RecursiveStatus), this.Status),
                this.Exception.Message ?? "");
        }

        internal static void CSVValidateFirstLine(string pFirstLine)
        {
            List<string> values = pFirstLine.Split(",").ToList();

            if (!values[0].Replace(" ", "").ToUpper().Equals("IMAGENAME"))
            {
                Console.WriteLine("Error: The first cell of the first line of the csv file must contain 'Image Name'.");
                System.Environment.Exit(1000);
            }
        }

        internal protected void CSVBreakDown()
        {
            try
            {
                List<string> values = this.CSVLine.Split(",").ToList();
                this.FileName = values[0];

                if(this.FileName.Trim().Equals(""))
                {
                    this.Status = RecursiveStatus.Incomplete;
                }
            }
            catch(Exception ex)
            {
                this.Status = RecursiveStatus.Error;
                this.Exception = ex;
            }
        }

        internal protected void CopyFile()
        {
            RecursiveSearchJob rsJob = RecursiveSearchJob.Instance;
            Console.WriteLine(String.Concat("Copying file: ", this.FileName));

            if (this.Status.Equals(RecursiveStatus.Error))
            {
                return;
            }

            if (this.Status.Equals(RecursiveStatus.Incomplete))
            {
                return;
            }

            if (this.Status.Equals(RecursiveStatus.Duplicate))
            {
                return;
            }

            if (this.SearchResults.Count() == 0)
            {
                this.Status = RecursiveStatus.NoFilesFound;
                return;
            }

            if (this.SearchResults.Count() == 1)
            {
                this.CopySingleFile();
                return;
            }

            if (this.SearchResults.Count() > 1)
            {
                this.CopyMultipleFiles();
                return;
            }
        }

        private void CopySingleFile()
        {
            RecursiveSearchJob rsJob = RecursiveSearchJob.Instance;

            try
            {
                this.Status = RecursiveStatus.Copying;

                File.Copy(this.SearchResults[0].FilePath,
                    String.Concat(rsJob.TargetDirectory, "\\", this.SearchResults[0].FileName));

                this.Status = RecursiveStatus.Copied;
            }
            catch(Exception ex)
            {
                this.Status = RecursiveStatus.Error;
                this.Exception = ex;
            }
        }

        private void CopyMultipleFiles()
        {
            RecursiveSearchJob rsJob = RecursiveSearchJob.Instance;

            this.Status = RecursiveStatus.Copying;

            string thisTarget = String.Concat(rsJob.TargetDirectory, "\\", Path.GetFileNameWithoutExtension(this.FileName));
            Directory.CreateDirectory(thisTarget);
            
            foreach(SearchResult sr in this.SearchResults)
            {
                try
                {
                    sr.SetOutputName();
                    File.Copy(sr.FilePath, String.Concat(thisTarget, "\\", sr.OutputName));
                }
                catch(Exception ex)
                {
                    this.Status = RecursiveStatus.Error;
                    this.Exception = ex;
                }
            }

            if(!this.Status.Equals(RecursiveStatus.Error))
            {
                this.Status = RecursiveStatus.MultipleFilesCopied;
            }
        }
    }

}
