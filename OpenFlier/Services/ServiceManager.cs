using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenFlier.Services
{
    public class ServiceManager
    {
        public event EventHandler? LoadCompleted;
        private Thread LoadServiceThread { get; set; }
        public ServiceManager()
        {
            LoadServiceThread = new Thread(() =>
            {
                VerificationService.Initialize();
                HardwareService.Initialize();
                UdpService.Initialize();
                FtpService.Initialize();
                MqttService.Initialize();
                LoadCompleted?.Invoke(this, EventArgs.Empty);
            });
            LoadServiceThread.TrySetApartmentState(ApartmentState.STA);
        }
        public void BeginLoad()
        {
            LoadServiceThread?.Start();
        }
        public void EndAllServices()
        {
            UdpService.StopUdpBroadcast();
            FtpService.StopFtpServer();
        }
    }
}
