﻿using MQTTnet.Server;
using MQTTnet;
using OpenFlier.Plugin;
using OpenFlier.Services;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Loader;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using static OpenFlier.Controls.PInvoke.Methods;
using static OpenFlier.Controls.PInvoke.ParameterTypes;
using System.Reflection;
using System.Collections.ObjectModel;
using Microsoft.Win32;

namespace OpenFlier;

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow : Window
{
    private readonly ServiceManager serviceManager = new();
    private Config tempConfig=new();
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        ConfigService.OutputDefaultConfig();
        ConfigService.ReadConfig();
        if (LocalStorage.Config.Appearances.EnableWindowEffects ?? true)
        {
            RefreshFrame();
            RefreshDarkMode();
        }
        serviceManager.LoadCompleted += ServiceManager_LoadCompleted;
        serviceManager.BeginLoad();
    }

    private void ServiceManager_LoadCompleted(object? sender, EventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            IPAddress.Text = LocalStorage.IPAddress;
            ConnectCode.Text = LocalStorage.ConnectCode;
            MachineIdentifier.Text = LocalStorage.MachineIdentifier;
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
        mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

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
        serviceManager.EndAllServices();
        base.OnClosing(e);
    }

    private void ConfigurePluginButton_Click(object sender, RoutedEventArgs e)
    {
        var pluginInfo = (MqttServicePlugin)((ListBoxItem)MqttPluginsListBox.ContainerFromElement((Button)sender)).Content;
        if (pluginInfo.PluginFilePath == null)
            return;
        try
        {
            FileInfo assemblyFileInfo = new FileInfo(pluginInfo.PluginFilePath);
            var alc = new ConfiguratorAssemblyLoadContext(assemblyFileInfo.FullName);
            var assembly = alc.LoadFromAssemblyPath(assemblyFileInfo.FullName);
            
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
            alc.Unload();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message+"\n"+ex.StackTrace);
        }

    }

    private void InitializeConfigurator()
    {
        tempConfig = LocalStorage.Config;
        MqttPluginsListBox.ItemsSource = tempConfig.MqttServicePlugins;
    }

    private void AddMqttPlugin_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog ofd = new OpenFileDialog();
        ofd.Filter = "OpenFlier Mqtt Plugin (*.dll)|*.dll";
        ofd.Multiselect = true;
        ofd.CheckFileExists = true;
        if(ofd.ShowDialog()==true)
        {
            foreach(var path in ofd.FileNames)
            {
                try
                {
                    FileInfo assemblyFileInfo = new FileInfo(path);
                    var alc = new ConfiguratorAssemblyLoadContext(assemblyFileInfo.FullName);
                    var assembly = alc.LoadFromAssemblyPath(assemblyFileInfo.FullName);

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
                    }
                    alc.Unload();
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
        tempConfig.MqttServicePlugins.Remove((MqttServicePlugin)MqttPluginsListBox.SelectedItem);
        //MqttPluginsListBox.ItemsSource = tempConfig.MqttServicePlugins;
    }
}

public class ConfiguratorAssemblyLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;
    public ConfiguratorAssemblyLoadContext(string mainAssemblyToLoadPath) : base(isCollectible: true)
    {
        _resolver = new AssemblyDependencyResolver(mainAssemblyToLoadPath);
    }
    protected override Assembly? Load(AssemblyName name)
    {
        var assemblyPath = _resolver.ResolveAssemblyToPath(name);
        if (assemblyPath != null)
        {
            return LoadFromAssemblyPath(assemblyPath);
        }
        return null;
    }
}
