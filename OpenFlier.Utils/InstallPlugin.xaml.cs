using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using OpenFlier.Plugin;

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
            var pluginData = System.IO.File.ReadAllBytes(filePath);
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
                    CommandInputCallerNamesTextBlock.Text = string.Join(", ", singlePluginPackage.CommandInputCallerNames);
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
            Config? currentConfig;
            var mainFileName = "";
            foreach (var file in singlePluginPackage.Files)
            {
                if (file.IsPluginMain)
                {
                    mainFileName = file.Filename;
                    break;
                }
            }
            if (System.IO.File.Exists("config.json"))
            {
                var configContent = System.IO.File.ReadAllText("config.json");
                currentConfig = JsonSerializer.Deserialize<Config>(configContent);
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
                        if (mqttPlugin.PluginIdentifier == singlePluginPackage.PluginIdentifier)
                        {
                            currentConfig.MqttServicePlugins.Remove(mqttPlugin);
                            break;
                        }
                    }
                    currentConfig.MqttServicePlugins.Add(new MqttServicePlugin
                    {
                        PluginAuthor = singlePluginPackage.PluginAuthor,
                        MqttMessageType = singlePluginPackage.MqttMessageType,
                        PluginDescription = singlePluginPackage.PluginDescription,
                        PluginFilePath = $"Plugins\\{singlePluginPackage.PluginIdentifier}\\{mainFileName}",
                        PluginIdentifier = singlePluginPackage.PluginIdentifier,
                        PluginName = singlePluginPackage.PluginName,
                        PluginNeedConfigEntry = singlePluginPackage.PluginNeedConfigEntry,
                        PluginVersion = singlePluginPackage.PluginVersion,
                        RequestedMinimumOpenFlierVersion = singlePluginPackage.RequestedMinimumOpenflierVersion
                    });
                    break;
                case PluginType.CommandInputPlugin:
                    foreach (var cmdPlugin in currentConfig.CommandInputPlugins)
                    {
                        if (cmdPlugin.PluginIdentifier == singlePluginPackage.PluginIdentifier)
                        {
                            currentConfig.CommandInputPlugins.Remove(cmdPlugin);
                        }
                    }
                    currentConfig.CommandInputPlugins.Add(new CommandInputPlugin
                    {
                        PluginAuthor = singlePluginPackage.PluginAuthor,
                        PluginCallerNames = singlePluginPackage.CommandInputCallerNames.ToArray(),
                        PluginDescription = singlePluginPackage.PluginDescription,
                        PluginFilePath = $"Plugins\\{singlePluginPackage.PluginIdentifier}\\{mainFileName}",
                        PluginIdentifier = singlePluginPackage.PluginIdentifier,
                        PluginName = singlePluginPackage.PluginName,
                        PluginNeedConfigEntry = singlePluginPackage.PluginNeedConfigEntry,
                        PluginVersion = singlePluginPackage.PluginVersion,
                        RequestedMinimumOpenFlierVersion = singlePluginPackage.RequestedMinimumOpenflierVersion
                    });
                    break;
            }
            System.IO.File.WriteAllText("config.json", JsonSerializer.Serialize(currentConfig));
            
            if (!Directory.Exists("Plugins"))
                Directory.CreateDirectory("Plugins");
            var currentPluginDirectory = $"Plugins\\{singlePluginPackage.PluginIdentifier}";
            if (Directory.Exists(currentPluginDirectory))
                Directory.Delete(currentPluginDirectory, true);
            Directory.CreateDirectory(currentPluginDirectory);
            foreach (var pluginFile in singlePluginPackage.Files)
            {
                System.IO.File.WriteAllBytes($"{currentPluginDirectory}\\{pluginFile.Filename}", pluginFile.FileData.ToArray());
            }
            Buttons.Visibility = Visibility.Hidden;
            DoneText.Visibility = Visibility.Visible;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Shutdown();
        }
    }

}


public class Config
{
    public Appearances Appearances { get; set; } = new Appearances();
    public General General { get; set; } = new General();
    public SpecialChannels SpecialChannels { get; set; } = new SpecialChannels();
    public ObservableCollection<MqttServicePlugin> MqttServicePlugins { get; set; } = new();
    public ObservableCollection<CommandInputPlugin> CommandInputPlugins { get; set; } = new();
}
public class Appearances
{
    public string? WindowTitle
    {
        get; set;
    }
    public string? PrimaryColor
    {
        get; set;
    }
    public string? SecondaryColor
    {
        get; set;
    }
    public string? BackgroundImage
    {
        get; set;
    }
    public bool? EnableWindowEffects
    {
        get; set;
    }
    public bool? SyncColorWithSystem
    {
        get; set;
    }
}
public class General
{
    public string? DefaultUpdateCheckURL
    {
        get; set;
    }
    public int? UDPBroadcastPort { get; set; } = 33338;
    public int? MqttServerPort { get; set; } = 61136;
    public string? SpecifiedMachineIdentifier
    {
        get; set;
    }
    public string? SpecifiedConnectCode
    {
        get; set;
    }
    public bool UsePng { get; set; } = false;
    public string SpecifiedEmulatedVersion { get; set; } = "2.0.9";
}
public class MqttServicePlugin
{
    public string? PluginFilePath
    {
        get; set;
    }
    public string? PluginName
    {
        get; set;
    }
    public string? PluginAuthor
    {
        get; set;
    }
    public string? PluginVersion
    {
        get; set;
    }
    public string? RequestedMinimumOpenFlierVersion
    {
        get; set;
    }
    public long? MqttMessageType
    {
        get; set;
    }
    public bool? PluginNeedConfigEntry
    {
        get; set;
    }
    public string? PluginDescription
    {
        get; set;
    }
    public string? PluginIdentifier
    {
        get; set;
    }
}
public class CommandInputPlugin
{
    public string? PluginFilePath
    {
        get; set;
    }
    public string? PluginName
    {
        get; set;
    }
    public string? PluginAuthor
    {
        get; set;
    }
    public string? PluginVersion
    {
        get; set;
    }
    public string? RequestedMinimumOpenFlierVersion
    {
        get; set;
    }
    public string[]? PluginCallerNames
    {
        get; set;
    }
    public bool? PluginNeedConfigEntry
    {
        get; set;
    }
    public string? PluginIdentifier
    {
        get; set;
    }
    public string? PluginDescription
    {
        get; set;
    }
}
public class SpecialChannels
{
    public string LocalRandomSource { get; set; } = "";
    public string LocalRandomAllowedUsers { get; set; } = "";
    public string RemoteRandomSource { get; set; } = "";
    public string RemoteRandomAllowedUsers { get; set; } = "";
    public string CommandInputAllowedUsers { get; set; } = "";
}
