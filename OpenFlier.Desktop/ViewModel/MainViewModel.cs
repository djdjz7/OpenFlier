﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using OpenFlier.Desktop.Localization;
using OpenFlier.Desktop.Model;
using System.Linq;

namespace OpenFlier.Desktop.ViewModel
{
    internal partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _ipAddress = "";
        [ObservableProperty]
        private string _allIpAddress = "";
        [ObservableProperty]
        private string _connectCode = "";
        [ObservableProperty]
        private string _machineIdentifier = "";

        public MainViewModel()
        {
            WeakReferenceMessenger.Default.Register<ServiceReset>(this, (obj, service) =>
            {
                if (service.IPAddresses is null || service.IPAddresses.Length == 0)
                    IpAddress = "";
                else
                {
                    if (service.IPAddresses.Length == 1)
                    {
                        IpAddress = service.IPAddresses[0].ToString();
                        AllIpAddress = IpAddress;
                    }
                    else
                    {
                        IpAddress = Backend.MultipleAddresses;
                        AllIpAddress = string.Join('\n', service.IPAddresses.Select((x) => x.ToString()));
                    }
                }
                ConnectCode = service.ConnectCode ?? "";
                MachineIdentifier = service.MachineIdentifier ?? "";
            });
        }
    }
}
