using Microsoft.Win32;
using OpenFlier.Plugin;
using OpenFlier.Core.Services;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using OpenFlier.Services;
using static OpenFlier.Controls.PInvoke.Methods;
using static OpenFlier.Controls.PInvoke.ParameterTypes;
using OpenFlier.Core;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Security.Cryptography.Xml;

namespace OpenFlier;

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
        ConfigService.ReadConfig();
        if (LocalStorage.Config.Appearances.EnableWindowEffects ?? true)
        {
            RefreshFrame();
            RefreshDarkMode();
        }
        
        List<IPAddress> ipAddresses = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .ToList();
        if(ipAddresses.Count > 1)
        {
            var selectNetworkInterface = new SelectNetworkInterface(ipAddresses);
            selectNetworkInterface.InterfaceSelected += (s, e) =>
            {
                serviceManager = new ServiceManager((IPAddress?)((SelectNetworkInterface)s).IPListBox.SelectedItem);
                serviceManager.OnLoadCompleted += ServiceManager_LoadCompleted;
                serviceManager.BeginLoad();
                LoadingScreen.Visibility = Visibility.Visible;
                MainGrid.Visibility = Visibility.Hidden;
            };
            selectNetworkInterface.ShowDialog();
        }
        else
        {
            serviceManager.OnLoadCompleted += ServiceManager_LoadCompleted;
            serviceManager.BeginLoad();
            LoadingScreen.Visibility = Visibility.Visible;
            MainGrid.Visibility = Visibility.Hidden;
        }
    }

    private void ServiceManager_LoadCompleted(object? sender, EventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            IPAddress.Text = CoreStorage.IPAddress?.ToString();
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

    private void RefreshFrame()
    {
        var mainWindowPtr = new WindowInteropHelper(this).Handle;
        var mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
        mainWindowSrc.CompositionTarget.BackgroundColor = System.Windows.Media.Color.FromArgb(0, 0, 0, 0);

        var margins = new MARGINS();
        margins.cxLeftWidth = -1;
        margins.cxRightWidth = -1;
        margins.cyTopHeight = -1;
        margins.cyBottomHeight = -1;

        ExtendFrame(mainWindowSrc.Handle, margins);

        SetWindowAttribute(
            new WindowInteropHelper(this).Handle,
            DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
            2);
    }

    private void RefreshDarkMode()
    {
        SetWindowAttribute(
            new WindowInteropHelper(this).Handle,
            DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE,
            0);
    }

    private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        this.DragMove();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        serviceManager.TerminateAllServices();
        base.OnClosing(e);
    }

    private void ConfigurePluginButton_Click(object sender, RoutedEventArgs e)
    {
        var pluginInfo = (LocalMqttServicePluginInfo)((ListBoxItem)MqttPluginsListBox.ContainerFromElement((Button)sender)).Content;
        if (pluginInfo.PluginFilePath == null)
            return;
        try
        {
            FileInfo assemblyFileInfo = new FileInfo(pluginInfo.PluginFilePath);
            var assembly = Assembly.LoadFrom(assemblyFileInfo.FullName);

            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (type.GetInterface("IMqttServicePlugin") == null)
                    continue;
                if (type.FullName == null)
                    continue;
                IMqttServicePlugin? mqttServicePlugin = (IMqttServicePlugin?)assembly.CreateInstance(type.FullName);
                if (mqttServicePlugin == null)
                    continue;
                mqttServicePlugin.PluginOpenConfig();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
        }

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
                        IMqttServicePlugin? mqttServicePlugin = (IMqttServicePlugin?)assembly.CreateInstance(type.FullName);
                        if (mqttServicePlugin == null)
                            continue;
                        var pluginInfo = mqttServicePlugin.GetMqttServicePluginInfo();
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