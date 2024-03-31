using System.Net;

namespace OpenFlier.Desktop.Model
{
    public class ServiceReset
    {
        public string? ConnectCode { get; set; }
        public IPAddress[]? IPAddresses { get; set; }
        public string? MachineIdentifier { get; set; }
    }
}
