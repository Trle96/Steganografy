using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplRad
{
    public class IOPaths
    {
        internal string ImagePath
        {
            get;
            set;
        }

        internal string MessagePath
        {
            get;
            set;
        }

        internal string OutputPath
        {
            get;
            set;
        }

        public IOPaths(string imagePath, string messagePath, string outputPath)
        {
            this.ImagePath = imagePath;
            this.MessagePath = messagePath;
            this.OutputPath = outputPath;
        }
    }
}
