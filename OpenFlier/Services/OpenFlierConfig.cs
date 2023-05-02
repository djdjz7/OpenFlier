using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Web;

namespace OpenFlier.Services
{
    public class OpenFlierConfig
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
                    LogManager.GetLogger(typeof(OpenFlierConfig)).Error("Error while parsing config.", e);
                }
            }
        }
        public static void OutputDefaultConfig()
        {
            var config = new Config();
            config.MqttServicePlugins.Add(new MqttServicePlugin()
            {
                MqttMessageType = 30000L,
                PluginFilePath = "Plugins/DemoPlugin.dll",
            });
            File.WriteAllText("defconfig.json", JsonSerializer.Serialize(config));
        }
    }
    public class Config
    {
        public Appearances Appearances { get; set; } = new Appearances();
        public General General { get; set; } = new General();
        public SpecialChannels SpecialChannels { get; set; } = new SpecialChannels();
        public List<MqttServicePlugin> MqttServicePlugins { get; set; } = new();
        public List<CommandInputPlugin> CommandInputPlugins { get; set; } = new();
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
        public string? UDPBroadcastFormat { get; set; }
        public int? UDPBroadcastPort { get; set; }
        public int? MqttServerPort { get; set; }
        public string? SpecifiedMachineIdentifier { get; set; }
        public string? SpecifiedConnectCode { get; set; }
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
}
