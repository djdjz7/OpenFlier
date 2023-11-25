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
        public static string MachineIdentifier { get; set; } = "";
        public static string MacAddress { get; set; } = "";
        public static List<IPAddress>? IPAddresses { get; set; }
        public static string ConnectCode { get; set; } = "";
        public static string Version { get; set; } = "2.1.1";
    }
}
