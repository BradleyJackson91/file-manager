using System;
using System.Text;


namespace filemanager.src
{
    internal class CSVProcessing
    {


        internal static List<string> ReadIn(string pDirectory)
        {
            List<string> result = new List<string>();

            using (StreamReader reader = new StreamReader(pDirectory))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (!line.Equals(null))
                    {
                        result.Add(line);
                    }
                }
            }

            return result;
        }

        internal static void GenerateReportSection(string pStatus)
        {
            FileJob fileJob = FileJob.Instance;

            StringBuilder sb = new StringBuilder();

            List<FileRequest> requests = fileJob.GetStatusFileRequests(pStatus);

            sb.AppendLine(String.Concat(pStatus, ": Total - ", requests.Count));
            sb.AppendLine(FileRequest.CSVFirstLine());

            if (requests.Count > 0)
            {
                switch (pStatus)
                {
                    case ("Error"):
                        Console.WriteLine("Errors have occurred whilst copying certain files. See report for details.");
                        break;
                    case ("Skipped"):
                        Console.WriteLine("Certain files have been skipped. See report for details.");
                        break;
                    case ("Overwritten"):
                        Console.WriteLine("Certain files have been overwritten. See report for details.");
                        break;
                };

                foreach (FileRequest request in requests)
                {
                    sb.AppendLine(request.CSVLine);
                }
            }
            else
            {
                sb.AppendLine(String.Concat("There are no records with a status of ", pStatus, "."));
            }

            sb.AppendLine("");

            File.AppendAllText(fileJob.ReportPath, sb.ToString());
        }

        internal static void GenerateSummary()
        {
            FileJob fileJob = FileJob.Instance;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Summary:");

            sb.AppendLine(String.Concat("Total files:,", fileJob.FileRequests.Count()));

            int runningTotal = 0;
            foreach (string s in Enum.GetNames(typeof(Status)))
            {
                int statusCount = fileJob.GetStatusFileRequests(s).Count();
                runningTotal += statusCount;

                sb.AppendLine(String.Concat(s, ":,", statusCount));
            }

            sb.AppendLine(String.Concat("Total:,", runningTotal));

            File.AppendAllText(fileJob.ReportPath, sb.ToString());
        }

        internal static string CSVExists(string pCSVDirectory)
        {
            int count = Utilities.ListOfFiles(pCSVDirectory, "*.csv").ToList().Count();

            if (count < 2)
            {
                Console.WriteLine("Error: Please make sure that your CSV file is in the following directory (CSVTemplate.csv will not be processed):");
                Console.WriteLine(pCSVDirectory);
                System.Environment.Exit(101);
            }
            if (count > 2)
            {
                Console.WriteLine("Error: Please make sure that the following directory contains only one CSV file (other than CSVTemplate.csv:");
                Console.WriteLine(pCSVDirectory);
                System.Environment.Exit(102);
            }

            List<string> files = Utilities.ListOfFiles(pCSVDirectory, "*.csv");

            foreach(string f in files)
            {
                if (Path.GetFileName(f).Equals("CSVTemplate.csv"))
                {
                    files.RemoveAt(files.IndexOf(f));
                    break;
                }
            }

            return files[0];
        }

        internal static void GenerateRecursiveReportSection(string pStatus)
        {
            RecursiveSearchJob fileJob = RecursiveSearchJob.Instance;

            StringBuilder sb = new StringBuilder();

            List<RecursiveFileRequest> requests = fileJob.GetStatusFileRequests(pStatus);

            sb.AppendLine(String.Concat(pStatus, ": Total - ", requests.Count));
            sb.AppendLine(RecursiveFileRequest.CSVFirstLine());

            if (requests.Count > 0)
            {
                switch (pStatus)
                {
                    case ("Error"):
                        Console.WriteLine("Errors have occurred whilst copying certain files. See report for details.");
                        break;
                    case ("Missing"):
                        Console.WriteLine("Certain files were missing. See report for details.");
                        break;
                    case ("MultipleFilesCopied"):
                        Console.WriteLine("Certain file requests have multiple files. See report for details.");
                        break;
                };

                foreach (RecursiveFileRequest request in requests)
                {
                    sb.AppendLine(request.CSVLine);
                }
            }
            else
            {
                sb.AppendLine(String.Concat("There are no records with a status of ", pStatus, "."));
            }

            sb.AppendLine("");

            File.AppendAllText(fileJob.ReportPath, sb.ToString());
        }

        internal static void GenerateRecursiveSummary()
        {
            RecursiveSearchJob rsJob = RecursiveSearchJob.Instance;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Summary:");

            sb.AppendLine(String.Concat("Total files requests:,", rsJob.FileRequests.Count()));

            int runningTotal = 0;
            foreach (string s in Enum.GetNames(typeof(RecursiveStatus)))
            {
                int statusCount = rsJob.GetStatusFileRequests(s).Count();
                runningTotal += statusCount;

                sb.AppendLine(String.Concat(s, ":,", statusCount));
            }

            sb.AppendLine(String.Concat("Total:,", runningTotal));

            File.AppendAllText(rsJob.ReportPath, sb.ToString());
        }

        internal static void CreateTemplateCSVFiles()
        {
            RecursiveSearchJob rsJob = RecursiveSearchJob.Instance;
            string templateRecursiveCSVPath = String.Concat(rsJob.CSVDirectory, "\\CSVTemplate.csv");
            if(!File.Exists(templateRecursiveCSVPath))
            {
                File.WriteAllText(templateRecursiveCSVPath, RecursiveFileRequest.CSVTemplateFirstLine());
            }

            FileJob fileJob = FileJob.Instance;
            string templateCSVPath = String.Concat(fileJob.CSVDirectory, "\\CSVTemplate.csv");
            if(!File.Exists(templateCSVPath))
            {
                File.WriteAllText(templateCSVPath, FileRequest.CSVTemplateFirstLine());
            }
        }
    }
}