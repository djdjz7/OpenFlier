using OpenFlier.Core;
using OpenFlier.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
        public ConfigControl(Config currentConfig)
        {
            InitializeComponent();
            DataContext = new ConfigControlModel(currentConfig);
        }


        private void ConfigurePluginButton_Click(object sender, RoutedEventArgs e)
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

    }
}
