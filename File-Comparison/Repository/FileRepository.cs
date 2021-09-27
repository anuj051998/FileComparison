using File_Comparison.Models;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace File_Comparison.Repositiry
{
    public class FileRepository:IFileRepo
    {
        public static List<string> checkList;
        public string getChecklist()
        {
            return String.Join(", ", checkList.ToArray());
        }
        public string getContentType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
        public List<FileProperties> ListDirectory(string path)
        {
            List<FileProperties> prop = new List<FileProperties>();
            //getting array of all directories and sub directories
            string[] li = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            foreach (var i in li)
            {
                string temporaryFileName = Path.GetFileName(i);
                string tempFilePath = Path.GetFullPath(i);
                try
                {
                    //checking if the file name is already present in Dictionry or not
                    var exists = prop.Where(x => x.fileName == temporaryFileName).Select(x => x.fileName).FirstOrDefault();
                    int count = 2;
                    while (!string.IsNullOrEmpty(exists))
                    {
                        var splitted = tempFilePath.Split("\\");
                        string tempRootPath = splitted[splitted.Length - count++];
                        temporaryFileName = $"{tempRootPath}\\{temporaryFileName}";
                        exists = prop.Where(x => x.fileName == temporaryFileName).Select(x => x.fileName).FirstOrDefault();
                    }
                    prop.Add(new FileProperties()
                    {
                        fileName = temporaryFileName,
                        filePath = Path.GetFullPath(i),
                        fileType = getContentType(Path.GetFileName(i)),
                        numberOfLines = File.ReadLines(Path.GetFullPath(i)).Count(),
                        //parentDirectory = Path.GetFullPath(i).Split("\\")[Path.GetFullPath(i).Split("\\").Length - 2]
                    });
                    //string existing = prop.Where(x => x.fileName == Path.GetFileName(i)).Select(x => x.fileName).FirstOrDefault();
                    //if (string.IsNullOrEmpty(existing))
                    //{
                    //    prop.Add(new FileProperties()
                    //    {
                    //        fileName = Path.GetFileName(i),
                    //        filePath = Path.GetFullPath(i),
                    //        fileType = getContentType(Path.GetFileName(i)),
                    //        numberOfLines = File.ReadLines(Path.GetFullPath(i)).Count(),
                    //        parentDirectory = Path.GetFullPath(i).Split("\\")[Path.GetFullPath(i).Split("\\").Length - 2]
                    //    }); 
                    //}
                    //else
                    //{
                    //    var s = Path.GetFullPath(i).Split('\\');
                    //    existing = "";
                    //    int count = 2;
                    //    string tempName = "";
                    //    //it was causing exception because of adding duplicate keys in Dictionary
                    //    //adding parent directory name with files 
                    //    while (existing != null)
                    //    {
                    //        tempName = s[s.Length - count++] + "/" + tempName;
                    //        string checkName = $"{tempName}/{Path.GetFileName(i)}";
                    //        existing = prop.Where(x => x.fileName == checkName).Select(x => x.fileName).FirstOrDefault();
                    //    }
                    //    prop.Add(new FileProperties()
                    //    {
                    //        fileName = tempName + "/" + Path.GetFileName(i),
                    //        filePath = Path.GetFullPath(i),
                    //        fileType = getContentType(Path.GetFileName(i)),
                    //        numberOfLines = File.ReadLines(Path.GetFullPath(i)).Count()
                    //    });
                    //}
                }
                catch(Exception ex) { }
            }
            return prop;
        }
        /// <summary>
        /// Method to compare the file contents line by line
        /// </summary>
        /// <param name="pathOne">Takes the path of first file</param>
        /// <param name="pathTwo">Takes the path of Second file</param>
        /// <returns>true if files are same otherwise returns false</returns>
        public bool checkContents(string pathOne, string pathTwo)
        {
            //reading all lines in files
            string[] linesOne = File.ReadAllLines(pathOne);
            string[] linesTwo = File.ReadAllLines(pathTwo);
            int index = 0;
            while (index < linesOne.Count())
            {
                if (linesOne[index] != linesTwo[index])
                    return false;
                index++;
            }
            return true;
        }
        /// <summary>
        /// Method to compare all files in provided paths
        /// </summary>
        /// <returns> returns a list of OutputModel object</returns>
        public List<OutputModel> check(string path1, string path2)
        {
            //add file extenstions for the files to be included in result
            checkList = new List<string>();
            checkList.Add("cs");
            checkList.Add("cshtml");
            checkList.Add("config");
            var d1 = ListDirectory(path1);
            var d2 = ListDirectory(path2);
            List<OutputModel> opModel = new List<OutputModel>();
            //if(d1.Count() != d2.Count())
            //{
            //    var o = d1.Select(i => i.fileName).ToList();
            //    var op = d1.Where(x => !o.Contains(x.fileName)).Select(x=>x);
            //}
            foreach (var i in d1)
            {
                //check if path contains the required file extensions
                if (checkList.Contains(i.fileName.Split('.')[i.fileName.Split('.').Length - 1]))
                {
                    FileProperties file = d2.Where(x => x.fileName == i.fileName).FirstOrDefault();
                    OutputModel model = new OutputModel();
                    if (file != null)
                    {
                        //checking if the number of lines and content in both files are same
                        if (file.numberOfLines == i.numberOfLines && checkContents(file.filePath, i.filePath))
                        {
                            model.isSame = true;
                        }
                        model.fileName = new FileInfo<string> { fileOne = string.IsNullOrEmpty(i.fileName) ? "Not Found" : i.fileName, fileTwo = string.IsNullOrEmpty(file.fileName) ? "Not Found" : file.fileName };
                        model.lines = new FileInfo<int> { fileOne = i.numberOfLines, fileTwo = file.numberOfLines };
                        model.types = new FileInfo<string> { fileOne = getContentType(Path.GetFileName(i.fileName)), fileTwo = getContentType(Path.GetFileName(file.fileName)) };
                        model.filePath = new FileInfo<string> { fileOne = i.filePath, fileTwo = file.filePath };
                    }
                    else
                    {
                        model.isSame = false;
                        model.fileName = new FileInfo<string> { fileOne = i.fileName == null ? "Not Found" : i.fileName, fileTwo = "Not Found" };
                        model.filePath = new FileInfo<string> { fileOne = i.filePath == null ? "Not Found" : i.filePath, fileTwo = "Not Found" };
                        model.message = $"The following file not found in folder two:  {i.fileName} ";
                        model.lines = new FileInfo<int> { fileOne = i.numberOfLines, fileTwo = 0 };
                    }
                   // model.parentDirectory = i.parentDirectory;
                    opModel.Add(model);
                }
            }
            foreach (var i in d2)
            {
                //check if path contains the required file extensions
                if (checkList.Contains(i.fileName.Split('.')[i.fileName.Split('.').Length - 1]))
                {
                    FileProperties file = d1.Where(x => x.fileName == i.fileName && x.numberOfLines == File.ReadAllLines(i.filePath).Length).FirstOrDefault();
                    if (file == null)
                    {
                        OutputModel output = new OutputModel();
                        output.fileName = new FileInfo<string> { fileOne = "Not Found", fileTwo = string.IsNullOrEmpty(i.fileName) ? "Not Found" : i.fileName };
                        output.filePath = new FileInfo<string> { fileOne = "Not Found", fileTwo = i.filePath };
                        output.message = $"The following file not found in folder One:  {i.fileName} ";
                        output.lines = new FileInfo<int> { fileOne = 0, fileTwo = File.ReadAllLines(i.filePath).Length };
                        opModel.Add(output);
                    }
                    else { continue; }
                }
            }
            return opModel;
        }
        /// <summary>
        /// Method to read contents of provided file path
        /// </summary>
        /// <param name="path">Takes path of a file</param>
        /// <returns>returns a list containing each lines of file</returns>
        public List<string> read(string path)
        {
            List<string> li = new List<string>();
            using (StreamReader reader = new StreamReader(path))
            {
                while (reader.ReadLine() != null)
                {
                    li.Add(reader.ReadLine());
                }
            }
            return li;
        }
        public string verifyPath(string path)
        {
            FileInfo info = new FileInfo(path);
            if (info.Exists)
            {
                return "<i class='fa fa-file'></i>";
            }
            else
            {
                return Directory.Exists(path) ? "<i class='fa fa-folder'></i>" : "<i class='fa fa-times'></i>";
            }
        }

        public string updateContent(string filePath, string content)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write(content);
                }
                return "Ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public string readFileContents(string fileName)
        {
            if (fileName == "Not Found")
                return string.Empty;
            string[] fileLines = File.ReadAllLines(fileName);
            return string.Join("\n", fileLines);
        }
        public string fakeList(string fileOne, string fileTwo)
        {
            var d1 = Directory.GetFiles(fileOne, "*", SearchOption.AllDirectories);
            var d2 = Directory.GetFiles(fileTwo, "*", SearchOption.AllDirectories);

            Dictionary<string, string> ContentsOne = new Dictionary<string, string>();
            Dictionary<string, string> ContentsTwo = new Dictionary<string, string>();

            foreach (var i in d1)
            {
                if (!Path.GetFullPath(i).Contains("bin") && !Path.GetFullPath(i).Contains("git"))
                {
                    try
                    {
                        ContentsOne.Add(Path.GetFileName(i), Path.GetFullPath(i));
                    }
                    catch
                    {
                        var s = Path.GetFullPath(i).Split('\\');
                        string existing = "";
                        int count = 2;
                        string tempName = "";
                        while (existing != null)
                        {
                            tempName = s[s.Length - count++] + "\\" + tempName;
                            string checkName = $"{tempName}\\{Path.GetFileName(i)}";
                            existing = ContentsOne.Keys.Where(x => x == checkName).Select(x => x).FirstOrDefault();
                        }
                        ContentsOne.Add($"{tempName}\\{Path.GetFileName(i)}", Path.GetFullPath(i));
                    }
                }

            }
            foreach (var i in d1)
            {
                if (!Path.GetFullPath(i).Contains("bin") && !Path.GetFullPath(i).Contains("git"))
                {
                    try
                    {
                        ContentsTwo.Add(Path.GetFileName(i), Path.GetFullPath(i));
                    }
                    catch
                    {
                        var s = Path.GetFullPath(i).Split('\\');
                        string existing = "";
                        int count = 2;
                        string tempName = "";
                        while (existing != null)
                        {
                            tempName = s[s.Length - count++] + "\\" + tempName;
                            string checkName = $"{tempName}\\{Path.GetFileName(i)}";
                            existing = ContentsTwo.Keys.Where(x => x == checkName).Select(x => x).FirstOrDefault();
                        }

                        ContentsTwo.Add($"{tempName}\\{Path.GetFileName(i)}", Path.GetFullPath(i));
                    }
                }
            }
            Dictionary<string, string> missingFiles = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> li in ContentsOne)
            {
                if (!ContentsTwo.ContainsKey(li.Key))
                {
                    try
                    {
                        missingFiles.Add(li.Key, li.Value);
                    }
                    catch
                    {

                    }
                }
            }
            foreach (KeyValuePair<string, string> li in ContentsTwo)
            {
                if (!ContentsOne.ContainsKey(li.Key))
                {
                    try
                    {
                        missingFiles.Add(li.Key, li.Value);
                    }
                    catch
                    {

                    }
                }
            }

            string raw = "";
            foreach (var i in missingFiles.Keys)
            {
                raw += $"{i},";
            }
            return $"{d1.Length} : {d2.Length} : {raw}";
        }
        //public static Dictionary<string[], string[]> readFiles(string pathOne, string pathTwo)
        //{
        //    List<string> fileOneLines = read(pathOne);
        //    List<string> fileTwoLines = read(pathTwo);
        //    Dictionary<string[], string[]> di = new Dictionary<string[], string[]>();
        //    try
        //    {
        //        di.Add(fileOneLines.ToArray(), fileTwoLines.ToArray());
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"File is having changes {Path.GetFileName(pathOne)} and {Path.GetFileName(pathTwo)}");
        //    }
        //    return di;
        //}
    }
}
