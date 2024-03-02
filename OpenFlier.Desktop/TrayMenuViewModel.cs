using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OpenFlier.Desktop
{
    public partial class TrayMenuViewModel : ObservableObject
    {
        public RelayCommand ShowWindowCommand { get; }
        public RelayCommand QuitCommand { get; }
        [ObservableProperty]
        private string _connectCode = "";
        public TrayMenuViewModel()
        {
            WeakReferenceMessenger.Default.Register<ConnectionCodeUpdatedMessage>(this, (obj, message) =>
            {
                ConnectCode = message.NewConnectCode;
            });
            ShowWindowCommand = new RelayCommand(() =>
            {
                Application.Current.MainWindow.Show();
            });
            QuitCommand = new RelayCommand(async () =>
            {
                await LocalStorage.ServiceManager?.MqttService.MqttServer?.StopAsync();
                foreach (var plugin in LocalStorage.ServiceManager.MqttService.LoadedMqttServicePlugins)
                {
                    await plugin.Value.BeforeExit();
                }
                if (LocalStorage.DesktopMqttService is not null)
                    foreach (var plugin in LocalStorage.DesktopMqttService.ImageHandler.LoadedCommandInputPlugins)
                    {
                        await plugin.Value.BeforeExit();
                    }
                Application.Current.Shutdown();
            });
        }
    }
}
