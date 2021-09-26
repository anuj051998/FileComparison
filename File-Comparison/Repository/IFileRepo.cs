using File_Comparison.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace File_Comparison.Repositiry
{
    public interface IFileRepo
    {
        public List<string> read(string path);
        public List<OutputModel> check(string path1, string path2);
        public bool checkContents(string pathOne, string pathTwo);
        public List<FileProperties> ListDirectory(string path);
        public string getChecklist();
        public string fakeList(string pathOne, string pathTwo);
        public string readFileContents(string fileName);
        public string updateContent(string filePath, string content);
        public string verifyPath(string path);
    }
}
