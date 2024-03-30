using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenFlier.Desktop.Model
{
    public class ServiceReset
    {
        public string? ConnectCode { get; set; }
        public IPAddress[]? IPAddresses { get; set; }
        public string? MachineIdentifier { get; set; }
    }
}
