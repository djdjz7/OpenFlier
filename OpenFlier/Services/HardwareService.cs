using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using log4net;
using System.Runtime.CompilerServices;

namespace OpenFlier.Services
{
    public static class HardwareService
    {
        public static void Initialize()
        {
            GetIPAddress();
        }
        public static void GetIPAddress()
        {
            List<IPAddress> ipAddressList = new List<IPAddress>(Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .Where(x => x.AddressFamily == AddressFamily.InterNetwork));
            if(ipAddressList.Count == 1)
            {
                LocalStorage.IPAddress = ipAddressList[0].ToString();
                LogManager.GetLogger(typeof(HardwareService)).Info($"IP address specified: {LocalStorage.IPAddress}");
                return;
            }
            if(ipAddressList.Count>1)
            {
                SelectNetworkInterface selectNetworkInterface = new SelectNetworkInterface(ipAddressList);
                selectNetworkInterface.InterfaceSelected += SelectNetworkInterface_InterfaceSelected;
                selectNetworkInterface.ShowDialog();
            }
        }

        private static void SelectNetworkInterface_InterfaceSelected(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                SelectNetworkInterface window = (SelectNetworkInterface)sender;
                LocalStorage.IPAddress = window.IPListBox.SelectedItem.ToString() ?? "";
                LogManager.GetLogger(typeof(HardwareService)).Info($"IP address specified: {LocalStorage.IPAddress}");
            }
        }
    }
}
