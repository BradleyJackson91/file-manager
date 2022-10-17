using System;


namespace filemanager.src
{
    internal class FMFolder
    {

        protected internal string FolderName { get; set; } = "";
        protected internal string FolderPath { get; set; } = "";
        protected internal FMFolder? ParentFolder { get; set; }
        protected internal List<FMFolder> ChildFolders { get; set; } = new List<FMFolder>();
        protected internal List<FMFile> Files { get; set; } = new List<FMFile>();


        internal FMFolder(string pFolderPath, string pFolderName, FMFolder? pParent = null)
        {
            this.FolderPath = pFolderPath;
            this.FolderName = pFolderName;
            this.ParentFolder = pParent;
        }

        protected internal void FindChildItems(ref Queue<FMFolder> q)
        {
            Console.WriteLine(String.Concat("Current Folder: ", this.FolderPath));
            this.FindFiles();

            List<string> children = new List<String>();

            try
            {
                children = Directory.GetDirectories(this.FolderPath).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Concat("Error: ", ex.Message));
            }

            if (children.Count == 0)
            {
                return;
            }

            foreach (string child in children)
            {
                FMFolder thisFolder = new FMFolder(child, Path.GetDirectoryName(child), this);
                this.ChildFolders.Add(thisFolder);
                q.Enqueue(thisFolder);
            }

            return;
        }

        protected internal void FindFiles()
        {
            List<string> files = new List<string>();

            try
            {
                files = Directory.GetFiles(this.FolderPath).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Concat("Exception: ", ex.Message));
            }

            foreach (string file in files)
            {
                this.Files.Add(new FMFile(file, Path.GetFileName(file)));
            }
        }

        internal static void RecursiveBuild(FMFolder pRoot)
        {
            Queue<FMFolder> q = new Queue<FMFolder>();
            q.Enqueue(pRoot);

            while(q.Count > 0)
            {
                FMFolder currentFolder = q.Dequeue();
                currentFolder.FindChildItems(ref q);
            }
        }

        internal static void RecursiveSearch(FMFolder pRoot, RecursiveFileRequest rfr)
        {
            Queue<FMFolder> q = new Queue<FMFolder>();
            q.Enqueue(pRoot);

            while(q.Count > 0)
            {
                FMFolder currentFolder = q.Dequeue();
                currentFolder.FileSearch(ref q, ref rfr);
            }
        }

        protected internal void FileSearch(ref Queue<FMFolder> pQ, ref RecursiveFileRequest pR)
        {
            foreach(FMFolder folder in this.ChildFolders)
            {
                pQ.Enqueue(folder);
            }

            foreach(FMFile file in this.Files)
            {
                if(file.FileName.Equals(pR.FileName))
                {
                    pR.SearchResults.Add(new SearchResult(file.FilePath));
                }
            }
        }
    }
}