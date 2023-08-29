using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenFlier.Core.Services;
using System.Net;

namespace OpenFlier.Core
{
    public static class CoreStorage
    {
        public static CoreConfig CoreConfig { get; set; } = new CoreConfig();
        public static string MachineIdentifier = "";
        public static string MacAddress = "";
        public static IPAddress? IPAddress;
        public static string ConnectCode = "";
        public static string Version = "2.1.0";
    }
}
