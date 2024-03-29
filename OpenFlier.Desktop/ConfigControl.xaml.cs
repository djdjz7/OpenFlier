﻿using OpenFlier.Core;
using OpenFlier.Core.Services;
using OpenFlier.Desktop.Localization;
using OpenFlier.Plugin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OpenFlier.Desktop
{
    /// <summary>
    /// ConfigControl.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigControl : UserControl
    {
        public ConfigControl(Config currentConfig, ServiceManager serviceManager, Action<Config> preReloadAction)
        {
            InitializeComponent();
            DataContext = new ConfigControlModel(currentConfig, serviceManager, preReloadAction);
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

    public class ConnectCodeValidationRule : ValidationRule
    {
        Regex regex = new("^\\d{4}$");
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(value.ToString()) || regex.IsMatch(value.ToString()!.Trim()))
            {
                return new ValidationResult(true, null);
            }
            return new ValidationResult(false, Backend.ConnectCodeFormatError);
        }
    }
}
