using Microsoft.Win32;
using OpenFlier.Controls;
using OpenFlier.Core;
using OpenFlier.Core.Services;
using OpenFlier.Desktop.Localization;
using OpenFlier.Plugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OpenFlier.Desktop;

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow : Window
{
    private ServiceManager serviceManager = LocalStorage.ServiceManager;

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var config = ConfigService.ReadConfig();
        if (config.Appearances.EnableWindowEffects ?? true)
        {
            WindowEffects.EnableWindowEffects(this);
        }

        await ValidatePluginPrivilege(config);

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

        ConfigTab.Content = new ConfigControl(config, serviceManager, async (newConfig) =>
        {
            await ValidatePluginPrivilege(newConfig);
            this.LoadingTextBlock.Text = Localization.UI.ReloadingText;
            this.LoadingScreen.Visibility = Visibility.Visible;
            this.MainScreen.Visibility = Visibility.Hidden;
        });

    }

    private void ServiceManager_LoadCompleted(bool isReloaded)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (isReloaded)
            {
                ConfigTab.Content = new ConfigControl(LocalStorage.Config, serviceManager, async (newConfig) =>
                {
                    await ValidatePluginPrivilege(newConfig);
                    this.LoadingTextBlock.Text = Localization.UI.ReloadingText;
                    this.LoadingScreen.Visibility = Visibility.Visible;
                    this.MainScreen.Visibility = Visibility.Hidden;
                });
            }
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

    private static bool IsApplicationRunningAsAdmin()
    {
        WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
        WindowsPrincipal windowsPrincipal = new WindowsPrincipal(windowsIdentity);
        return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    private async Task ValidatePluginPrivilege(Config config)
    {

        bool isAdmin = IsApplicationRunningAsAdmin();
        foreach (var plugin in config.MqttServicePlugins)
        {
            plugin.TempDisabled = false;
        }
        foreach (var plugin in config.CommandInputPlugins)
        {
            plugin.TempDisabled = false;
        }
        if (!isAdmin)
        {
            bool requireAdmin
                = config.MqttServicePlugins.Where(x => x.PluginInfo.PluginNeedsAdminPrivilege == true && x.Enabled == true).Count()
                + config.CommandInputPlugins.Where(x => x.PluginInfo.PluginNeedsAdminPrivilege == true && x.Enabled == true).Count() > 0;
            if (requireAdmin)
            {
                var result = await new PrivilegeErrorWindow().ShowDialog();
                switch (result)
                {
                    case PrivilegeErrorWindow.PrivilegeResult.DisableAndContinue:
                        foreach (var plugin in config.MqttServicePlugins)
                        {
                            plugin.TempDisabled = false;
                            if (plugin.PluginInfo.PluginNeedsAdminPrivilege && plugin.Enabled)
                                plugin.TempDisabled = true;
                        }
                        foreach (var plugin in config.CommandInputPlugins)
                        {
                            plugin.TempDisabled = false;
                            if (plugin.PluginInfo.PluginNeedsAdminPrivilege && plugin.Enabled)
                                plugin.TempDisabled = true;
                        }
                        break;
                    case PrivilegeErrorWindow.PrivilegeResult.RestartAsAdmin:

                        var path = Process.GetCurrentProcess().MainModule?.FileName;
                        var directory = Path.GetDirectoryName(path);
                        ProcessStartInfo startInfo = new()
                        {
                            UseShellExecute = true,
                            WorkingDirectory = directory,
                            FileName = path,
                            Verb = "runas"
                        };
                        Process.Start(startInfo);
                        Application.Current.Shutdown();
                        return;
                }
            }
        }
    }
}
