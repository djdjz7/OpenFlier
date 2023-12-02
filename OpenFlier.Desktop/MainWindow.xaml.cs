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
    private Config tempConfig = new();

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
        if(ipAddresses.Count == 0)
        {
            MessageBox.Show("No available IP address.");
            Application.Current.Shutdown();
            return;
        }

        serviceManager = new ServiceManager(config);
        serviceManager.OnLoadCompleted += ServiceManager_LoadCompleted;
        serviceManager.BeginLoad();
        LoadingScreen.Visibility = Visibility.Visible;
        MainGrid.Visibility = Visibility.Hidden;

        ConfigTab.Content = new ConfigControl(config);

    }

    private void ServiceManager_LoadCompleted(object? sender, EventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            Services.MqttService.MqttServer = serviceManager.MqttService.MqttServer;
            serviceManager.MqttService.OnClientConnected += Services.MqttService.OnClientConnectedAsync;
            serviceManager.MqttService.OnClientDisconnected += Services
                .MqttService
                .OnClientDisConnectedAsync;
            serviceManager.MqttService.OnScreenshotRequestReceived += Services
                .MqttService
                .OnAppMessageReceivedAsync;

            if (CoreStorage.IPAddresses.Count >1)
            {
                IPAddress.Text = Backend.MultipleAddresses;
                IPAddress.ToolTip = string.Join('\n', CoreStorage.IPAddresses);
            }
            ConnectCode.Text = CoreStorage.ConnectCode;
            MachineIdentifier.Text = CoreStorage.MachineIdentifier;
            LoadingScreen.Visibility = Visibility.Hidden;
            MainGrid.Visibility = Visibility.Visible;
            var presentationSource = PresentationSource.FromVisual(this);
            var scale = 1.0;
            if (presentationSource != null)
                scale = presentationSource.CompositionTarget.TransformToDevice.M11;
            LocalStorage.ScreenSize.Width = (int)(SystemParameters.PrimaryScreenWidth * scale);
            LocalStorage.ScreenSize.Height = (int)(SystemParameters.PrimaryScreenHeight * scale);
            InitializeConfigurator();
        });
    }

    private void Window_MouseLeftButtonDown(
        object sender,
        System.Windows.Input.MouseButtonEventArgs e
    )
    {
        this.DragMove();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        serviceManager.TerminateAllServices();
        base.OnClosing(e);
    }

    private void InitializeConfigurator()
    {
        tempConfig = LocalStorage.Config;
    }

    private void AddMqttPlugin_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog ofd = new OpenFileDialog();
        ofd.Filter = "OpenFlier Mqtt Plugin (*.dll)|*.dll";
        ofd.Multiselect = true;
        ofd.CheckFileExists = true;
        if (ofd.ShowDialog() == true)
        {
            foreach (var path in ofd.FileNames)
            {
                try
                {
                    FileInfo assemblyFileInfo = new FileInfo(path);
                    var assembly = Assembly.LoadFrom(assemblyFileInfo.FullName);

                    Type[] types = assembly.GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.GetInterface("IMqttServicePlugin") == null)
                            continue;
                        if (type.FullName == null)
                            continue;
                        IMqttServicePlugin? mqttServicePlugin = (IMqttServicePlugin?)
                            assembly.CreateInstance(type.FullName);
                        if (mqttServicePlugin == null)
                            continue;
                        var pluginInfo = mqttServicePlugin.GetPluginInfo();
                        /*
                        tempConfig.MqttServicePlugins.Add(new MqttServicePlugin
                        {
                            MqttMessageType = pluginInfo.MqttMessageType,
                            PluginAuthor = pluginInfo.PluginAuthor,
                            PluginDescription = pluginInfo.PluginDescription,
                            PluginFilePath = path,
                            PluginName = pluginInfo.PluginName,
                            PluginNeedConfigEntry = pluginInfo.PluginNeedConfigEntry,
                            PluginVersion = pluginInfo.PluginVersion,
                            RequestedMinimumOpenFlierVersion = pluginInfo.RequestedMinimumOpenFlierVersion,
                        });
                        */
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                }
            }
        }
    }

    private void RemoveMqttPlugin_Click(object sender, RoutedEventArgs e)
    {
        /*
        tempConfig.MqttServicePlugins.Remove((MqttServicePlugin)MqttPluginsListBox.SelectedItem);
        //MqttPluginsListBox.ItemsSource = tempConfig.MqttServicePlugins;
        */
    }

    private void TestButton_Click(object sender, RoutedEventArgs e)
    {
        (new Video()).ShowDialog();
    }
}
