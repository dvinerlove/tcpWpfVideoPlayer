using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfVideo.Models
{
    public class Video
    {
        private string fullPath;
        private string fileName;

        public string FullPath { get { return fullPath; }  }
        public string  FileName { get { return fileName; } }
        public Video(string fp="",string fn="null")
        {
            fullPath = fp;
            fileName = fn;
        }

    }
}
