using log4net;
using OpenFlier.Plugin;
using System.Text.Json;

namespace OpenFlier.Core;
public class CoreConfig
{
    public int? UDPBroadcastPort { get; set; } = 33338;
    public int? MqttServerPort { get; set; } = 61136;
    public string? SpecifiedMachineIdentifier { get; set; }
    public string? SpecifiedConnectCode { get; set; }
    public string SpecifiedEmulatedVersion { get; set; } = "2.0.9";
    public List<LocalPluginInfo<MqttServicePluginInfo>> MqttServicePlugins { get; set; } = new();
    public string VerificationContent { get; set; } = "{\"type\":20007,\"data\":{\"topic\":\"Ec1xkK+uFtV/QO/8rduJ2A==\"}}";
    public string? FtpDirectory { get; set; } = "Screenshots";
}

public class LocalPluginInfo<T>
{
    public T PluginInfo { get; set; }
    public string? LocalFilePath { get; set; }
    public bool Enabled { get; set; }
}
