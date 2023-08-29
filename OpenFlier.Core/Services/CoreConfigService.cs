using log4net;
using OpenFlier.Plugin;
using System.Text.Json;

namespace OpenFlier.Core.Services;

public class CoreConfigService
{
    public void ReadConfig()
    {
        var localJsonContent = "";
        if (File.Exists("core.config.json"))
            localJsonContent = File.ReadAllText("core.config.json");
        if (!string.IsNullOrEmpty(localJsonContent))
        {
            try
            {
                CoreStorage.CoreConfig = JsonSerializer.Deserialize<CoreConfig>(localJsonContent) ?? new CoreConfig();

            }
            catch (Exception e)
            {
                LogManager.GetLogger(typeof(CoreConfigService)).Error("Error while parsing config.", e);
            }
        }

    }
    public void OutputDefaultConfig()
    {
        var config = new CoreConfig();
        config.MqttServicePlugins.Add(new LocalMqttServicePluginInfo()
        {
            MqttMessageType = 10006L,
            PluginAuthor = "The OpenFlier Authors",
            PluginDescription = "Emulating ZY ClassHelper's DeviceVerification Method.",
            PluginName = "VerificationPlugin",
            PluginNeedConfigEntry = true,
            PluginVersion = "1.0",
            PluginFilePath = "Plugins/VerificationPlugin.dll",
            PluginIdentifier = "openflier.def.verification",
        });
        File.WriteAllText("core.defconfig.json", JsonSerializer.Serialize(config));
    }
}
public class CoreConfig
{
    public int? UDPBroadcastPort { get; set; } = 33338;
    public int? MqttServerPort { get; set; } = 61136;
    public string? SpecifiedMachineIdentifier { get; set; }
    public string? SpecifiedConnectCode { get; set; }
    public string SpecifiedEmulatedVersion { get; set; } = "2.0.9";
    public List<LocalMqttServicePluginInfo> MqttServicePlugins { get; set; } = new();
    public string VerificationContent { get; set; } = "{\"type\":20007,\"data\":{\"topic\":\"Ec1xkK+uFtV/QO/8rduJ2A==\"}}";
}
public class LocalMqttServicePluginInfo : MqttServicePluginInfo
{
    public string? PluginFilePath { get; set; }
}
