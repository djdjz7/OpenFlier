using OpenFlier.Core;
using OpenFlier.Desktop;
using OpenFlier.Plugin;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace OpenFlier.Utils
{
    /// <summary>
    /// InstallPlugin.xaml 的交互逻辑
    /// </summary>
    public partial class InstallPlugin : Window
    {
        string filePath;
        SinglePluginPackage singlePluginPackage;

        public InstallPlugin(string pluginFilePath)
        {
            InitializeComponent();
            Buttons.Visibility = Visibility.Visible;
            DoneText.Visibility = Visibility.Hidden;
            filePath = pluginFilePath.Trim();
            if (!System.IO.File.Exists(filePath))
            {
                MessageBox.Show("No such .ofspp file.");
                Application.Current.Shutdown();
                return;
            }
            var pluginData = File.ReadAllBytes(filePath);
            singlePluginPackage = SinglePluginPackage.Parser.ParseFrom(pluginData);
            var empty = new SinglePluginPackage();
            if (singlePluginPackage.ToString() == empty.ToString())
            {
                MessageBox.Show("Not a valid .ofspp file.");
                Application.Current.Shutdown();
                return;
            }
            PluginNameTextBlock.Text = singlePluginPackage.PluginName;
            PluginVersionTextBlock.Text = singlePluginPackage.PluginVersion;
            PluginIdentifierTextBlock.Text = singlePluginPackage.PluginIdentifier;
            PluginTypeTextBlock.Text = singlePluginPackage.PluginType.ToString();
            PluginDescriptionTextBlock.Text = singlePluginPackage.PluginDescription;
            PluginAuthorTextBlock.Text = singlePluginPackage.PluginAuthor;
            switch (singlePluginPackage.PluginType)
            {
                case PluginType.MqttServicePlugin:
                    MqttMessageTypeTextBlock.Visibility = Visibility.Visible;
                    CommandInputCallerNamesTextBlock.Visibility = Visibility.Collapsed;
                    MqttMessageTypeTextBlock.Text = singlePluginPackage.MqttMessageType.ToString();
                    break;
                case PluginType.CommandInputPlugin:
                    MqttMessageTypeTextBlock.Visibility = Visibility.Collapsed;
                    CommandInputCallerNamesTextBlock.Visibility = Visibility.Visible;
                    CommandInputCallerNamesTextBlock.Text = string.Join(
                        ", ",
                        singlePluginPackage.InvokeCommands
                    );
                    break;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Shutdown();
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            var processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                if (process.ProcessName == "OpenFlier.Desktop")
                    process.Kill();
            }
            Config? currentConfig = null;
            var pluginEntry = singlePluginPackage.PluginEntry;
            if (File.Exists("config.json"))
            {
                var configContent = File.ReadAllText("config.json");
                try
                {
                    currentConfig = JsonSerializer.Deserialize<Config>(configContent);
                }
                catch (System.Exception ex) { }
            }
            else
                currentConfig = new Config();
            if (currentConfig == null)
                currentConfig = new Config();
            switch (singlePluginPackage.PluginType)
            {
                case PluginType.MqttServicePlugin:
                    foreach (var mqttPlugin in currentConfig.MqttServicePlugins)
                    {
                        if (
                            mqttPlugin.PluginInfo.PluginIdentifier
                            == singlePluginPackage.PluginIdentifier
                        )
                        {
                            currentConfig.MqttServicePlugins.Remove(mqttPlugin);
                            break;
                        }
                    }
                    currentConfig.MqttServicePlugins.Add(
                        new LocalPluginInfo<MqttServicePluginInfo>
                        {
                            PluginInfo = new MqttServicePluginInfo
                            {
                                PluginAuthor = singlePluginPackage.PluginAuthor,
                                MqttMessageType = singlePluginPackage.MqttMessageType,
                                PluginDescription = singlePluginPackage.PluginDescription,
                                PluginIdentifier = singlePluginPackage.PluginIdentifier,
                                PluginName = singlePluginPackage.PluginName,
                                PluginNeedsConfigEntry = singlePluginPackage.PluginNeedsConfigEntry,
                                PluginVersion = singlePluginPackage.PluginVersion,
                            },
                            LocalFilePath =
                                $"Plugins\\{singlePluginPackage.PluginIdentifier}\\{pluginEntry}",
                            Enabled = true,
                        }
                    );

                    break;
                case PluginType.CommandInputPlugin:
                    foreach (var cmdPlugin in currentConfig.CommandInputPlugins)
                    {
                        if (
                            cmdPlugin.PluginInfo.PluginIdentifier
                            == singlePluginPackage.PluginIdentifier
                        )
                        {
                            currentConfig.CommandInputPlugins.Remove(cmdPlugin);
                            break;
                        }
                    }
                    currentConfig.CommandInputPlugins.Add(
                        new LocalPluginInfo<CommandInputPluginInfo>
                        {
                            PluginInfo = new CommandInputPluginInfo
                            {
                                PluginAuthor = singlePluginPackage.PluginAuthor,
                                InvokeCommands = singlePluginPackage.InvokeCommands.ToList(),
                                PluginDescription = singlePluginPackage.PluginDescription,
                                PluginIdentifier = singlePluginPackage.PluginIdentifier,
                                PluginName = singlePluginPackage.PluginName,
                                PluginNeedsConfigEntry = singlePluginPackage.PluginNeedsConfigEntry,
                                PluginVersion = singlePluginPackage.PluginVersion,
                            },
                            LocalFilePath =
                                $"Plugins\\{singlePluginPackage.PluginIdentifier}\\{pluginEntry}",
                            Enabled = true,
                        }
                    );
                    break;
            }
            System.IO.File.WriteAllText("config.json", JsonSerializer.Serialize(currentConfig));

            if (!Directory.Exists("Plugins"))
                Directory.CreateDirectory("Plugins");
            var currentPluginDirectory = $"Plugins\\{singlePluginPackage.PluginIdentifier}";
            if (Directory.Exists(currentPluginDirectory))
                Directory.Delete(currentPluginDirectory, true);
            Directory.CreateDirectory(currentPluginDirectory);
            using (var stream = new MemoryStream(singlePluginPackage.ZipArchive.ToArray()))
            {
                using (var zip = new ZipArchive(stream))
                {
                    zip.ExtractToDirectory(currentPluginDirectory, true);
                }
            }
            Buttons.Visibility = Visibility.Hidden;
            DoneText.Visibility = Visibility.Visible;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Shutdown();
        }

        static void RecursiveCopy(string source, string target)
        {
            Directory.CreateDirectory(target);
            var directoryInfo = new DirectoryInfo(source);
            foreach (var subdirectory in directoryInfo.GetDirectories())
            {
                RecursiveCopy(subdirectory.FullName, Path.Combine(target, subdirectory.Name));
            }
            foreach (var file in directoryInfo.GetFiles())
            {
                File.Copy(file.FullName, Path.Combine(target, file.Name), true);
            }
        }
    }
}
