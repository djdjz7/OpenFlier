using log4net;
using log4net.Plugin;
using OpenFlier.Plugin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace OpenFlier.Services;

public class ConfigService
{
    public static void ReadConfig()
    {
        var localJsonContent = "";
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
        File.WriteAllText("defconfig.json", JsonSerializer.Serialize(config));
    }
}
public class Config
{
    public Appearances Appearances { get; set; } = new Appearances();
    public General General { get; set; } = new General();
    public List<LocalImageProviderPluginInfo> ImageProviderPlugins = new List<LocalImageProviderPluginInfo>();
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

public class LocalImageProviderPluginInfo : ImageProviderPluginInfo
{
    public string? PluginFilePath { get; set; }
}