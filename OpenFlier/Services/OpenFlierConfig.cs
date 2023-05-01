﻿using log4net;
using System;
using System.IO;
using System.Text.Json;

namespace OpenFlier.Services
{
    public class OpenFlierConfig
    {
        public static Config LocalConfig { get; set; } = new Config();
        public static void ReadConfig()
        {
            string localJsonContent = "";
            if (File.Exists("config.json"))
                localJsonContent = File.ReadAllText("config.json");
            if (!string.IsNullOrEmpty(localJsonContent))
            {
                try
                {
                    LocalConfig = JsonSerializer.Deserialize<Config>(localJsonContent) ?? new Config();

                }
                catch (Exception e)
                {
                    LogManager.GetLogger(typeof(OpenFlierConfig)).Error("Error while parsing config.", e);
                }
            }
        }
        public static void OutputDefaultConfig()
        {
            File.WriteAllText("defconfig.json", JsonSerializer.Serialize(new Config()));
        }
    }
    public class Config
    {
        public Appearances Appearances { get; set; } = new Appearances();
        public General General { get; set; } = new General();
        public MqttServicePlugin[] MqttServicePlugins { get; set; } = Array.Empty<MqttServicePlugin>();
        public CommandInputPlugin[] CommandInputPlugins { get; set; } = Array.Empty<CommandInputPlugin>();
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
        public string? UDPBroadcastPort { get; set; }
        public string? MqttServicePort { get; set; }
        public string? SpecifiedMachineIdentifier { get; set; }
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

}