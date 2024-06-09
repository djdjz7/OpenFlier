﻿using OpenFlier.Core;
using OpenFlier.Core.Services;
using OpenFlier.Desktop.ViewModel;
using OpenFlier.Plugin;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace OpenFlier.Desktop.View
{
    /// <summary>
    /// ConfigControl.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigControl : UserControl
    {
        public ConfigControl(Config currentConfig, ServiceManager serviceManager, Action<Config> preReloadAction)
        {
            InitializeComponent();
            DataContext = new ConfigViewModel(currentConfig, serviceManager, preReloadAction);
        }


        private void ConfigureMqttServicePluginButton_Click(object sender, RoutedEventArgs e)
        {
            var pluginInfo =
                (LocalPluginInfo<MqttServicePluginInfo>)
                    ((ListBoxItem)MqttPluginsListBox.ContainerFromElement((Button)sender)).Content;
            if (pluginInfo.LocalFilePath == null)
                return;
            try
            {
                FileInfo assemblyFileInfo = new FileInfo(pluginInfo.LocalFilePath);
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
                    mqttServicePlugin.PluginOpenConfig();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }
        private void ConfigureCommandInputPluginButton_Click(object sender, RoutedEventArgs e)
        {
            var pluginInfo =
                (LocalPluginInfo<CommandInputPluginInfo>)
                    ((ListBoxItem)CommandInputPluginsListBox.ContainerFromElement((Button)sender)).Content;
            if (pluginInfo.LocalFilePath == null)
                return;
            try
            {
                FileInfo assemblyFileInfo = new FileInfo(pluginInfo.LocalFilePath);
                var assembly = Assembly.LoadFrom(assemblyFileInfo.FullName);

                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if (type.GetInterface("ICommandInputPlugin") == null)
                        continue;
                    if (type.FullName == null)
                        continue;
                    ICommandInputPlugin? mqttServicePlugin = (ICommandInputPlugin?)
                        assembly.CreateInstance(type.FullName);
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
    }
}