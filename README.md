# FileManager
A C# console application to copy files on mass from specified or unspecified locations.

Hello,

This project is still a work in progress. If you have any suggestions on how it can be improved, or if you would like to collaborate on it, let me know at bradley2.jackson@outlook.com

The project was originally created to search for duplicate files (1000+ different files, e.g., cheese.jpg, bree.jpg, wensleydale.jpg, etc...) within a 30+yr file archive. This repository has an MIT license attached to it: if it will help you, please, use it.

This repository is for a console application that will, on first-run, generate the required folders and csv template-file, then it can be used to read in the filled in csv file, run a breadth-first mapping process across and through-out the file-system from the user-specified root directory. 
As the program runs through the file structure, it builds up a map of the folders and files contained within them. Once this step is complete, the program will search through this comparing each file-name with the names specified in the read-in csv file. When a match is found, the directory of the matching file is added to the search item.
After the search process has finished, the program then runs through each search item, copying the matching files to an output directory. If there is only one matching file, it is copied to the root output path. If multiple matches exist, the program creates a folder within the root output for that specified search item, then copies all of the found files to that folder, appending the original file path to the end of the filename (e.g., cheese.jpg in C:/<User>/Desktop/Cheeses/ will have a file name of cheese_c_<User>_Desktop_Cheese.jpg). 

When the program finishes, a report is generated that contains information on which files were found, and how many of each were found, along with any errors that occured throughout the process.

To use: just clone the project, open it in Visual Studio, build and run. 
