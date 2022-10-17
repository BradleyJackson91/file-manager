using System;


namespace filemanager.src
{
    internal class FileJob
    {

        private static FileJob _instance = null;

        private FileJob()
        {

        }

        public static FileJob Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FileJob();
                }
                return _instance;
            }
        }
        protected internal static string BaseDirectory { get; } = String.Concat(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "\\FileManager\\KnownPaths\\");
        protected internal string CSVDirectory { get; set; } = String.Concat(BaseDirectory, "In\\");
        protected internal string TargetDirectory { get; set; } = String.Concat(BaseDirectory, "Out\\");
        protected internal string ReportPath { get; set; } = String.Concat(BaseDirectory, "Reports\\");
        protected internal List<FileRequest> FileRequests { get; set; } = new List<FileRequest>();

        protected internal void GenerateReport()
        {
            this.ReportPath += String.Concat("\\",
                "FileManagerReport_KnownPath_",
                DateTime.Now.Year.ToString(),
                DateTime.Now.Month.ToString(),
                DateTime.Now.Day.ToString(),
                DateTime.Now.TimeOfDay.ToString().Replace(":", "").Replace(" ", ""),
                ".csv");

            Console.WriteLine("Generating report for current job - file name:");
            Console.WriteLine(this.ReportPath);

            foreach (string s in Enum.GetNames(typeof(Status)))
            {
                CSVProcessing.GenerateReportSection(s);
            }

            CSVProcessing.GenerateSummary();

            Console.WriteLine("Report complete");
        }

        protected internal List<FileRequest> GetStatusFileRequests(string pStatus)
        {
            var result = from req in this.FileRequests
                         where req.Status.ToString() == pStatus
                         select req;

            return result.ToList();
        }

        internal static void ProcessJob()
        {
            FileJob fileJob = FileJob.Instance;

            fileJob.CSVDirectory = CSVProcessing.CSVExists(fileJob.CSVDirectory);
            Utilities.CheckIfDirectoryExists(fileJob.TargetDirectory);

            List<string> csvLines = CSVProcessing.ReadIn(fileJob.CSVDirectory);

            if (FileRequest.CSVValidateFirstLine(csvLines[0]))
            {
                csvLines.RemoveAt(0);
            }

            foreach (string csvLine in csvLines)
            {
                fileJob.FileRequests.Add(new FileRequest(csvLine));
            }

            foreach (FileRequest request in fileJob.FileRequests)
            {
                request.CopyFile();
            }

            fileJob.GenerateReport();
        }
    }
}
