using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filemanager.src
{
    internal class RecursiveSearchJob
    {
        private static RecursiveSearchJob _instance = null;

        private RecursiveSearchJob(){}

        public static RecursiveSearchJob Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RecursiveSearchJob();
                }
                return _instance;
            }
        }
        protected internal static string BaseDirectory { get; } = String.Concat(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "\\FileManager\\RootPath\\");
        protected internal string CSVDirectory { get; set; } = String.Concat(BaseDirectory, "In\\");
        protected internal string TargetDirectory { get; set; } = String.Concat(BaseDirectory, "Out\\");
        protected internal string ReportPath { get; set; } = String.Concat(BaseDirectory, "Reports\\");
        protected internal string RootPath { get; set; } = String.Empty;

        protected internal FMFolder Root { get; set; }

        protected internal List<RecursiveFileRequest> FileRequests { get; set; } = new List<RecursiveFileRequest>();

        internal static void ProcessJob()
        {
            RecursiveSearchJob rsJob = RecursiveSearchJob.Instance;

            Console.WriteLine("Hello, World!");

            Console.WriteLine("Please, enter the root folder path:");
            string rootFolderPath = Console.ReadLine() ?? "";

            while (!Directory.Exists(rootFolderPath))
            {
                Console.WriteLine(String.Concat("The root directory ", rootFolderPath, " does not exist."));
                Console.WriteLine("Please, enter the root folder path:");
                rootFolderPath = Console.ReadLine() ?? "";
            }


            // Step 1: Create the root folder:
            rsJob.Root = new FMFolder(rootFolderPath, Path.GetDirectoryName(rootFolderPath));

            // Create the output folder
            rsJob.TargetDirectory = String.Concat(rsJob.TargetDirectory, 
                "\\", 
                Utilities.RemoveSpecialCharacters(
                    String.Concat(
                        rsJob.Root.FolderPath, 
                        "_", 
                        DateTime.Now.Year, 
                        DateTime.Now.Month, 
                        DateTime.Now.Day, 
                        DateTime.Now.TimeOfDay.ToString()
                        )
                    )
                );

            Directory.CreateDirectory(rsJob.TargetDirectory);

            // Step 2: Build the folder and file structure:
            FMFolder.RecursiveBuild(rsJob.Root);

            // Save the root folder item to a json file
            Utilities.WriteToJSON(rsJob.Root);

            // Step 3: Read in required file list
            Utilities.CheckIfDirectoryExists(rsJob.CSVDirectory);
            rsJob.CSVDirectory = CSVProcessing.CSVExists(rsJob.CSVDirectory);
            List<string> csvStrings = CSVProcessing.ReadIn(rsJob.CSVDirectory);

            RecursiveFileRequest.CSVValidateFirstLine(csvStrings[0]);

            csvStrings.RemoveAt(0);

            foreach(string c in csvStrings)
            {
                rsJob.FileRequests.Add(new RecursiveFileRequest(c));
            }

            // Search and identify duplicate file names.
            List<string> fileNames = new List<string>();
            foreach(RecursiveFileRequest r in rsJob.FileRequests)
            {
                if(fileNames.Contains(r.FileName))
                {
                    r.Status = RecursiveStatus.Duplicate;
                }
                fileNames.Add(r.FileName);
            }

            // Recursive Search:
            foreach(RecursiveFileRequest r in rsJob.FileRequests)
            {
                if(!r.Status.Equals(RecursiveStatus.Duplicate))
                {
                    FMFolder.RecursiveSearch(rsJob.Root, r);
                }
            }

            // Copy Files  
            foreach(RecursiveFileRequest r in rsJob.FileRequests)
            {
                r.CopyFile();
            }

            // Create an output report.
            rsJob.GenerateReport();

        }

        protected internal void GenerateReport()
        {
            this.ReportPath += String.Concat("\\",
                "FileManagerReport_RootReport_",
                DateTime.Now.Year.ToString(),
                DateTime.Now.Month.ToString(),
                DateTime.Now.Day.ToString(),
                DateTime.Now.TimeOfDay.ToString().Replace(":", "").Replace(" ", ""),
                ".csv");

            Console.WriteLine("Generating report for current job - file name:");
            Console.WriteLine(this.ReportPath);

            foreach (string s in Enum.GetNames(typeof(RecursiveStatus)))
            {
                CSVProcessing.GenerateRecursiveReportSection(s);
            }

            CSVProcessing.GenerateRecursiveSummary();

            Console.WriteLine("Report complete");
        }

        protected internal List<RecursiveFileRequest> GetStatusFileRequests(string pStatus)
        {
            var result = from req in this.FileRequests
                         where req.Status.ToString() == pStatus
                         select req;

            return result.ToList();
        }
    }
}


