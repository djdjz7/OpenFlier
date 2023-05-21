using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Web;
using System.Windows;

namespace OpenFlier.Services
{
    public class ConfigService
    {
        public static void ReadConfig()
        {
            string localJsonContent = "";
            if (File.Exists("config.json"))
                localJsonContent = File.ReadAllText("config.json");
            if (!string.IsNullOrEmpty(localJsonContent))
            {
                try
                {
                    LocalStorage.Config = JsonSerializer.Deserialize<Config>(localJsonContent) ?? new Config();

                }
                catch (Exception e)
                {
                    LogManager.GetLogger(typeof(ConfigService)).Error("Error while parsing config.", e);
                }
            }
            
        }
        public static void OutputDefaultConfig()
        {
            var config = new Config();
            config.MqttServicePlugins.Add(new MqttServicePlugin()
            {
                MqttMessageType = 30000L,
                PluginAuthor = "The OpenFlier Authors",
                PluginName = "DemoPlugin",
                PluginNeedConfigEntry = false,
                PluginVersion = "0.1",
                RequestedMinimumOpenFlierVersion = "0.1",
                PluginDescription = "Testing plugin functionality.",
                PluginFilePath = "Plugins/DemoPlugin.dll",
            });
            config.MqttServicePlugins.Add(new MqttServicePlugin()
            {
                MqttMessageType = 10006L,
                PluginAuthor = "The OpenFlier Authors",
                PluginDescription = "Emulating ZY ClassHelper's DeviceVerification Method.",
                PluginName = "VerificationPlugin",
                PluginNeedConfigEntry = true,
                PluginVersion = "1.0",
                RequestedMinimumOpenFlierVersion = "0.1",
                PluginFilePath = "Plugins/VerificationPlugin.dll"
            });
            File.WriteAllText("defconfig.json", JsonSerializer.Serialize(config));
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
        public string? WindowTitle { get; set; }
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public string? BackgroundImage { get; set; }
        public bool? EnableWindowEffects { get; set; }
        public bool? SyncColorWithSystem { get; set; }
    }
    public class General
    {
        public string? DefaultUpdateCheckURL { get; set; }
        public int? UDPBroadcastPort { get; set; } = 33338;
        public int? MqttServerPort { get; set; } = 61136;
        public string? SpecifiedMachineIdentifier { get; set; }
        public string? SpecifiedConnectCode { get; set; }
        public bool UsePng { get; set; } = false;
        public string SpecifiedEmulatedVersion { get; set; } = "2.0.9";
    }
    public class MqttServicePlugin
    {
        public string? PluginFilePath { get; set; }
        public string? PluginName { get; set; }
        public string? PluginAuthor { get; set; }
        public string? PluginVersion { get; set; }
        public string? RequestedMinimumOpenFlierVersion { get; set; }
        public long? MqttMessageType { get; set; }
        public bool? PluginNeedConfigEntry { get; set; }
        public string? PluginDescription { get; set; }
    }
    public class CommandInputPlugin
    {
        public string? PluginFilePath { get; set; }
        public string? PluginName { get; set; }
        public string? PluginAuthor { get; set; }
        public string? PluginVersion { get; set; }
        public string? RequestedMinimumOpenFlierVersion { get; set; }
        public string[]? PluginCallerNames { get; set; }
        public bool? PluginNeedConfigEntry { get; set; }
    }
    public class SpecialChannels
    {
        public string LocalRandomSource { get; set; } = "";
        public string LocalRandomAllowedUsers { get; set; } = "";
        public string RemoteRandomSource { get; set; } = "";
        public string RemoteRandomAllowedUsers { get; set; } = "";
        public string CommandInputAllowedUsers { get; set; } = "";
    }

    public class MqttPluginsList : ObservableCollection<MqttServicePlugin> { }
}
