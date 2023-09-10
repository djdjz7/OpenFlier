using OpenFlier.Core;
using OpenFlier.Core.Services;
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
            IPAddress a = new IPAddress(0);
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            a = ip.Address;
                            break;
                        }
                    }
                }
            }

            ServiceManager serviceManager = new ServiceManager(new CoreConfig
            {
                SpecifiedConnectCode = "9999",
                FtpDirectory = Path.Combine(FileSystem.CacheDirectory, "OpenFlier", "Screenshots")
            }, a);
            serviceManager.OnLoadCompleted += ServiceManager_OnLoadCompleted;
            serviceManager.BeginLoad();
            
        }

        private void ServiceManager_OnLoadCompleted(object sender, EventArgs e)
        {
            Dispatcher.Dispatch(() =>
            {
                ConnectCode.Text = OpenFlier.Core.CoreStorage.ConnectCode;
            });
        }
    }
}