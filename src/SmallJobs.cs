using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace filemanager.src
{
    internal class SmallJobs
    {
        internal static void RenameFiles()
        {
            string directory = "";

            do
            {
                Console.WriteLine("Please, enter the folder path:");
                directory = Console.ReadLine() ?? "";
            }
            while(!Directory.Exists(directory));

            string prefix = "";
            bool acceptInput = false;
            do
            {
                Console.WriteLine("Please, enter a prefix for the files:");
                Console.WriteLine("(or leave blank if you don't want to add a prefix)");
                prefix = Console.ReadLine() ?? "";
                string acceptResponse = "";
                do
                {
                    Console.WriteLine(String.Concat("Confirm using prefix '", prefix, "' (y/n)"));
                    acceptResponse = Console.ReadLine();
                    acceptResponse = acceptResponse.Replace(" ", "").ToUpper();
                }
                while (!acceptResponse.Equals("Y") || !acceptResponse.Equals("N"));

                if (acceptResponse.Equals("Y"))
                {
                    acceptInput = true;
                }
            }
            while (!acceptInput);

            string postfix = "";
            acceptInput = false;
            do
            {
                Console.WriteLine("Please, enter the identifier for removing filename endings:");
                Console.WriteLine("Or leave blank if you don't want to remove anything from the end of the filenames.");
                Console.WriteLine("(e.g. '__' to remove '__thisPart' from 'thisfile__thisPart.txt') -> 'thisfile.txt'");
                postfix = Console.ReadLine() ?? "";
                string acceptResponse = "";
                do
                {
                    Console.WriteLine(String.Concat("Confirm using identifier '", postfix, "' (y/n)"));
                    acceptResponse = Console.ReadLine();
                    acceptResponse = acceptResponse.Replace(" ", "").ToUpper();
                }
                while (!acceptResponse.Equals("Y") || !acceptResponse.Equals("N"));

                if (acceptResponse.Equals("Y"))
                {
                    acceptInput = true;
                }
            }
            while (!acceptInput);


            Console.WriteLine(String.Concat("Renaming files in ", directory));
            Console.WriteLine("--------------------");

            Utilities.RenameFiles(directory, prefix, postfix);

            Console.WriteLine("--------------------");
            Console.WriteLine(String.Concat("Finished renaming files in ", directory));
        }

        internal static void FileListToCSV()
        {
            string directory = "";

            do
            {
                Console.WriteLine("Please, enter the folder path:");
                directory = Console.ReadLine() ?? "";

                if (!Directory.Exists(directory))
                {
                    Console.WriteLine(String.Concat("Error: the folder path '", directory, "' does not exist."));
                }
            }
            while (!Directory.Exists(directory));

            Console.WriteLine(String.Concat("Writing out file names in ", directory));
            Console.WriteLine("--------------------");

            Utilities.OutputDirectoryFilesToCSV(directory);

            Console.WriteLine("--------------------");
            Console.WriteLine(String.Concat("Finished writing out file names in ", directory));
        }
    }
}
