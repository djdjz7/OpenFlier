using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenFlier.Services;

namespace OpenFlier
{
    public static class LocalStorage
    {
        public static Config Config { get; set; } = new Config();
        public static string MachineIdentifier = "";
        public static string MacAddress = "";
        public static string IPAddress = "";
        public static string ConnectCode = "";
        public static string Version = "2.0.9";
        public static Rectangle ScreenSize = new Rectangle(0,0,1920,1080);
    }
}
