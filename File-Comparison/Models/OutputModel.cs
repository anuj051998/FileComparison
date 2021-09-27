using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace File_Comparison.Models
{
    public class FileInfo<T>
    {
        public T fileOne { get; set; }
        public T fileTwo { get; set; }
    }

    public class OutputModel
    {
        public FileInfo<string> fileName { get; set; }
        public FileInfo<string> filePath { get; set; }
        public FileInfo<int> lines { get; set; }
        public FileInfo<string> types { get; set; }
        public FileInfo<string> path { get; set; }
        public bool isSame { get; set; }
        public string message { get; set; }
        //public string parentDirectory { get; set; }
        public OutputModel()
        {
            isSame = false;
        }
    }
}
