using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace File_Comparison.Models
{
    public class FileProperties
    {
        public string fileName { get; set; }
        public string filePath { get; set; }
        public int numberOfLines { get; set; }
        public string fileType { get; set; }
        public bool isSame = false;
        public FileProperties()
        {

        }
        public FileProperties(string name, string path, int lines, string type)
        {
            fileName = name;
            filePath = path;
            numberOfLines = lines;
            fileType = type;
        }
    }

}
