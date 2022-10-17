using System;
using System.Text;
using System.Text.Json;

namespace filemanager.src
{
    internal class Utilities
    {

        internal static List<string> ListOfFiles(string pDirectory, string pSearchPattern = "")
        {
            List<string> result = new List<string>();

            try
            {
                result = Directory.GetFiles(pDirectory, pSearchPattern).ToList() ?? new List<string>();

            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(String.Concat("Error: UnathorizedAccessException caught at Utilties - ListOfFiles. (Ex = ", ex.Message, ")"));
                System.Environment.Exit(1);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(String.Concat("Error: ArgumentNullException caught at Utilties - ListOfFiles. (Ex = ", ex.Message, ")"));
                System.Environment.Exit(2);
            }
            catch (PathTooLongException ex)
            {
                Console.WriteLine(String.Concat("Error: Exception caught at Utilties - ListOfFiles. (Ex = ", ex.Message, ")"));
                System.Environment.Exit(3);
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine(String.Concat("Error: DirectoryNotFoundException caught at Utilties - ListOfFiles. (Ex = ", ex.Message, ")"));
                System.Environment.Exit(4);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(String.Concat("Error: ArgumentException caught at Utilties - ListOfFiles. (Ex = ", ex.Message, ")"));
                System.Environment.Exit(5);
            }
            catch (IOException ex)
            {
                Console.WriteLine(String.Concat("Error: IOException caught at Utilties - ListOfFiles. (Ex = ", ex.Message, ")"));
                System.Environment.Exit(6);
            }

            return result;
        }

        internal static void CheckIfDirectoryExists(string pDirectory)
        {
            if (!Directory.Exists(pDirectory))
            {
                Console.WriteLine("Error: the following directory does not exist.");
                Console.WriteLine(pDirectory);
                System.Environment.Exit(7);
            }
        }

        internal static string RemoveSpecialCharacters(string pInput)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in pInput)
            {
                if (c == '\\')
                {
                    sb.Append('_');
                    continue;
                }

                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        internal static void WriteToJSON(Object o)
        {
            // TODO: Use this within RecursiveSearchJob

            // TODO: Change to interface:
            RecursiveSearchJob rsJob = RecursiveSearchJob.Instance;

            try
            {
                string jsonString = JsonSerializer.Serialize(o);
                File.WriteAllText(String.Concat(rsJob.CSVDirectory, "\\", "RootObject_", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Utilities.RemoveSpecialCharacters(DateTime.Now.TimeOfDay.ToString()), ".json"), jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Concat("Error serialising object: ", ex.Message));
            }
        }

        internal static void CreateFolderStructure()
        {
            if (!Directory.Exists(String.Concat(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "\\FileManager")))
            {
                string baseDirectory = String.Concat(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "\\FileManager");
                Directory.CreateDirectory(baseDirectory);

                string knownDirectory = String.Concat(baseDirectory, "\\KnownPaths");
                Directory.CreateDirectory(knownDirectory);
                Directory.CreateDirectory(String.Concat(knownDirectory, "\\In"));
                Directory.CreateDirectory(String.Concat(knownDirectory, "\\Out"));
                Directory.CreateDirectory(String.Concat(knownDirectory, "\\Reports"));

                string rootDirectory = String.Concat(baseDirectory, "\\RootPath");
                Directory.CreateDirectory(rootDirectory);
                Directory.CreateDirectory(String.Concat(rootDirectory, "\\In"));
                Directory.CreateDirectory(String.Concat(rootDirectory, "\\Out"));
                Directory.CreateDirectory(String.Concat(rootDirectory, "\\Reports"));
            }
        }

        internal static void RenameFiles(string pDirectory, string pPrefix, string pPostfix)
        {
            List<string> files = Directory.GetFiles(pDirectory).ToList();

            foreach (string f in files)
            {
                Console.Write(String.Concat("Current File: ", Path.GetFileName(f), " -> New Name: "));

                string newName = String.Concat(pPrefix, Path.GetFileName(f));

                if (!pPostfix.Equals("") && f.Contains(pPostfix))
                {
                    newName = String.Concat(newName.Remove(newName.IndexOf(pPostfix), newName.Length - newName.IndexOf(pPostfix)), Path.GetExtension(f));
                }

                string newFilePath = String.Concat(Path.GetDirectoryName(f), "\\", newName);

                try
                {
                    File.Move(f, newFilePath);
                    Console.WriteLine(newName);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(String.Concat("Error copying file: ", ex.Message));
                }

            }
        }

        internal static void OutputDirectoryFilesToCSV(string pDirectory)
        {
            List<string> files = Directory.GetFiles(pDirectory).ToList();

            StringBuilder sb = new StringBuilder();

            foreach(string f in files)
            {
                sb.AppendLine(Path.GetFileName(f));
            }

            File.WriteAllText(String.Concat(pDirectory, "\\FileList.csv"), sb.ToString());
        }
    }
}
