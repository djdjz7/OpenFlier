﻿using log4net;
using log4net.Plugin;
using OpenFlier.Core;
using OpenFlier.Plugin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace OpenFlier.Desktop;

public class ConfigService
{
    public static Config ReadConfig()
    {
        var localJsonContent = "";
        if (File.Exists("config.json"))
            localJsonContent = File.ReadAllText("config.json");
        if (!string.IsNullOrEmpty(localJsonContent))
        {
            try
            {
                LocalStorage.Config = JsonSerializer.Deserialize<Config>(localJsonContent) ?? new Config();
                return LocalStorage.Config;
            }
            catch (Exception e)
            {
                LogManager.GetLogger(typeof(Config)).Error("Error while parsing config.", e);
            }
        }
        return new Config();

    }
    public static void OutputDefaultConfig()
    {
        var config = new Config();
        File.WriteAllText("defconfig.json", JsonSerializer.Serialize(config));
    }
}
public class Config: CoreConfig
{
    public Appearances Appearances { get; set; } = new Appearances();
    public General General { get; set; } = new General();

    public List<LocalPluginInfo<MqttServicePluginInfo>> MqttServicePlugins = new List<LocalPluginInfo<MqttServicePluginInfo>>();

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
    public bool UsePng { get; set; } = false;
}

public class LocalPluginInfo<T>
{
    public T? PluginInfo { get; set; }
    public string? LocalFilePath { get; set; }
}