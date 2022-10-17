using filemanager.src;

Console.WriteLine("Hello, World!");

Utilities.CreateFolderStructure();
CSVProcessing.CreateTemplateCSVFiles();

Console.WriteLine("Please, choose from the following options:");

List<string> options = new List<string>
{
    "Copy files from known locations",
    "Copy files from a root path",
    "Rename all files within a specified folder",
    "Create a list of files within a specified folder"
};

foreach(string o in options)
{
    Console.WriteLine(String.Concat((options.IndexOf(o)+1), ". ", o));
}

int option = -1;
Int32.TryParse(Console.ReadLine(), out option);

while(option < 1 || option > options.Count())
{
    Console.WriteLine(String.Concat("Please, select from the above options by entering 1 - ", options.Count()));
    Int32.TryParse(Console.ReadLine(), out option);
}

switch(option)
{
    case 1: FileJob.ProcessJob();
        break;
    case 2: RecursiveSearchJob.ProcessJob();
        break;
    case 3: SmallJobs.RenameFiles();
        break;
    case 4: SmallJobs.FileListToCSV();
        break;
    default:
        Console.WriteLine();
        break;
}


