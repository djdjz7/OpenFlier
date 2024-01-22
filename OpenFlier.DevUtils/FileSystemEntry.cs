using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFlier.DevUtils
{
    public class FileSystemEntry
    {
        public bool IsDirectory { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
    }
}
