using OpenFlier.Core;
using OpenFlier.Core.Services;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace OpenFlier.Mobile
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void ContentPage_Loaded(object sender, EventArgs e)
        {
            /*
            List<IPAddress> iPAddresses = new List<IPAddress>();

            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        IP.Text = ip.Address.ToString();
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            IP.Text = ip.Address.ToString();
                            iPAddresses.Add(ip.Address);
                        }
                    }
                }
            }*/


            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            IP.Text = ip.Address.ToString();
                        }
                    }
                }
            }

            ServiceManager serviceManager = new ServiceManager(new CoreConfig
            {
                SpecifiedConnectCode = "9999",
                FtpDirectory = Path.Combine(FileSystem.CacheDirectory, "OpenFlier", "Screenshots")
            }, null);
            serviceManager.OnLoadCompleted += ServiceManager_OnLoadCompleted;
            serviceManager.BeginLoad();
            
        }

        private void ServiceManager_OnLoadCompleted(bool isReloaded)
        {
            Dispatcher.Dispatch(() =>
            {
                ConnectCode.Text = OpenFlier.Core.CoreStorage.ConnectCode;
            });
        }
    }
}