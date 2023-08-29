using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenFlier.Core.Services
{
    public class ServiceManager
    {
        public event EventHandler? OnLoadCompleted;
        private Thread _loadServiceThread;
        public VerificationService VerificationService { get; } = new VerificationService();
        public UdpService UdpService { get; } = new UdpService();
        public MqttService MqttService { get; } = new MqttService();
        public FtpService FtpService { get; } = new FtpService();
        public ServiceManager(IPAddress? ipAddress = null)
        {
            List<IPAddress> ipAddresses = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .ToList();

            if(ipAddress is not null && !ipAddresses.Contains(ipAddress))
                throw new ArgumentOutOfRangeException(nameof(ipAddress));

            if (ipAddress == null)
                ipAddress = ipAddresses[0];

            CoreStorage.IPAddress = ipAddress;

            _loadServiceThread = new Thread(() =>
            { 
                VerificationService.Initialize();
                UdpService.Initialize();
                MqttService.Initialize();
                FtpService.Initialize();

                OnLoadCompleted?.Invoke(this, EventArgs.Empty);
            });
            _loadServiceThread.TrySetApartmentState(ApartmentState.STA);

        }
        public void BeginLoad()
        {
            _loadServiceThread?.Start();
        }
        public void TerminateAllServices()
        {
            UdpService.StopUdpBroadcast();
            FtpService.StopFtpServer();
        }
    }
}
