using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OpenFlier.Services
{
    public static class HardwareService
    {
        public static void InitializeHardwareService()
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
            }
        }
    }
}
