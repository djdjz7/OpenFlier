using Microsoft.Win32;
using OpenFlier.Controls;
using OpenFlier.Core;
using OpenFlier.Core.Services;
using OpenFlier.Desktop.Localization;
using OpenFlier.Plugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace OpenFlier.Desktop;

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow : Window
{
    private ServiceManager serviceManager = LocalStorage.ServiceManager;

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        ConfigService.OutputDefaultConfig();

        var config = ConfigService.ReadConfig();
        if (config.Appearances.EnableWindowEffects ?? true)
        {
            WindowEffects.EnableWindowEffects(this);
        }

        List<IPAddress> ipAddresses = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .ToList();
        if (ipAddresses.Count == 0)
        {
            MessageBox.Show("No available IP address.");
            Application.Current.Shutdown();
            return;
        }

        serviceManager = new ServiceManager(config);
        serviceManager.OnLoadCompleted += ServiceManager_LoadCompleted;
        serviceManager.BeginLoad();
        LoadingScreen.Visibility = Visibility.Visible;
        MainScreen.Visibility = Visibility.Hidden;

        ConfigTab.Content = new ConfigControl(config, serviceManager, () =>
        {
            this.LoadingTextBlock.Text = Localization.UI.ReloadingText;
            this.LoadingScreen.Visibility = Visibility.Visible;
            this.MainScreen.Visibility = Visibility.Hidden;
        });

    }

    private void ServiceManager_LoadCompleted(bool isReloaded)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (!isReloaded)
            {
                LocalStorage.DesktopMqttService = new()
                {
                    MqttServer = serviceManager.MqttService.MqttServer
                };
                serviceManager.MqttService.OnClientConnected += LocalStorage.DesktopMqttService.OnClientConnectedAsync;
                serviceManager.MqttService.OnClientDisconnected += LocalStorage
                    .DesktopMqttService
                    .OnClientDisConnectedAsync;
                serviceManager.MqttService.OnScreenshotRequestReceived += LocalStorage
                    .DesktopMqttService
                    .OnAppMessageReceivedAsync;

                if (CoreStorage.IPAddresses.Count > 1)
                {
                    IPAddress.Text = Backend.MultipleAddresses;
                    IPAddress.ToolTip = string.Join('\n', CoreStorage.IPAddresses);
                }
            }
            ConnectCode.Text = CoreStorage.ConnectCode;
            MachineIdentifier.Text = CoreStorage.MachineIdentifier;
            LoadingScreen.Visibility = Visibility.Hidden;
            MainScreen.Visibility = Visibility.Visible;
            var presentationSource = PresentationSource.FromVisual(this);
            var scale = 1.0;
            if (presentationSource != null)
                scale = presentationSource.CompositionTarget.TransformToDevice.M11;
            LocalStorage.ScreenSize.Width = (int)(SystemParameters.PrimaryScreenWidth * scale);
            LocalStorage.ScreenSize.Height = (int)(SystemParameters.PrimaryScreenHeight * scale);
        });
    }

    private void Window_MouseLeftButtonDown(
        object sender,
        System.Windows.Input.MouseButtonEventArgs e
    )
    {
        this.DragMove();
    }

    private void TestButton_Click(object sender, RoutedEventArgs e)
    {
        (new Video()).ShowDialog();
    }
}
