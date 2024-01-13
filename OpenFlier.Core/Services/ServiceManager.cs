using System.Net;

namespace OpenFlier.Core.Services
{
    public class ServiceManager
    {
        public delegate void ServiceLoadedEventHandler(bool isReloaded);
        public event ServiceLoadedEventHandler? OnLoadCompleted;
        private Thread _loadServiceThread;
        public VerificationService VerificationService { get; } = new VerificationService();
        public UdpService UdpService { get; } = new UdpService();
        public MqttService MqttService { get; } = new MqttService();
        public FtpService FtpService { get; } = new FtpService();
        public ServiceManager(CoreConfig coreConfig, List<IPAddress>? ipAddresses = null)
        {
            if (ipAddresses == null)
            {
                ipAddresses = Dns.GetHostEntry(Dns.GetHostName())
                                .AddressList
                                .Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                .ToList();
            }

            CoreStorage.IPAddresses = ipAddresses;
            CoreStorage.CoreConfig = coreConfig;

            _loadServiceThread = new Thread(() =>
            {
                VerificationService.Initialize();
                UdpService.Initialize();
                MqttService.Initialize();
                FtpService.Initialize();

                OnLoadCompleted?.Invoke(false);
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

        public void RestartAllServices(CoreConfig newConfig)
        {
            TerminateAllServices();
            CoreStorage.CoreConfig = newConfig;
            _loadServiceThread = new Thread(() =>
            {
                VerificationService.Initialize();
                UdpService.Initialize();

                // These are absolutely unnecessary:
                // MqttService.Initialize();
                FtpService.Initialize();

                OnLoadCompleted?.Invoke(true);
            });
            _loadServiceThread.TrySetApartmentState(ApartmentState.STA);
            _loadServiceThread.Start();
        }
    }
}
