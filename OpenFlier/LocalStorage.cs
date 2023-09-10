using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenFlier.Services;
using OpenFlier.Core.Services;

namespace OpenFlier
{
    public static class LocalStorage
    {
        public static Config Config { get; set; } = new Config();
        public static Rectangle ScreenSize = new Rectangle(0,0,1920,1080);
        public static ServiceManager ServiceManager;
    }
}
