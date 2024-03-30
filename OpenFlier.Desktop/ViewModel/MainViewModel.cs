using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using OpenFlier.Desktop.Model;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFlier.Desktop.ViewModel
{
    internal partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _ipAddress = "";
        [ObservableProperty]
        private string _connectCode = "";
        [ObservableProperty]
        private string _machineIdentifier = "";

        public MainViewModel()
        {
            WeakReferenceMessenger.Default.Register<ServiceReset>(this, (obj, service) =>
            {
                if (service.IPAddresses is not null)
                    IpAddress = string.Join('\n', service.IPAddresses.Select((x) => x.ToString()));
                else
                    IpAddress = "";
                ConnectCode = service.ConnectCode ?? "";
                MachineIdentifier = service.MachineIdentifier ?? "";
            });
        }
    }
}
