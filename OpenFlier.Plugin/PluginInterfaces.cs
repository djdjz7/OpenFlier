using MQTTnet;
using MQTTnet.Server;

namespace OpenFlier.Plugin;

public class PluginInfoBase
{
    public required string PluginName { get; init; }
    public required string PluginIdentifier { get; init; }
    public required string PluginAuthor { get; init; }
    public required string PluginVersion { get; init; }
    public required bool PluginNeedConfigEntry { get; init; }
    public required string PluginDescription { get; init; }
}
public class MqttServicePluginInfo : PluginInfoBase
{
    public required long MqttMessageType { get; init; }
}

public interface IMqttServicePlugin
{
    public MqttServicePluginInfo GetPluginInfo();
    public MqttApplicationMessage PluginMain(string clientID, IMqttServer mqttServer);
    public void PluginOpenConfig();
}

public class CommandInputPluginInfo: PluginInfoBase
{
    public required List<string> InvokeCommands { get; init; }
}

public interface ICommandInputPlugin
{
    public CommandInputPluginInfo GetPluginInfo();
    public MqttApplicationMessage PluginMain(string clientID, string invokeCommand, IMqttServer mqttServer, bool usePng);
    public void PluginOpenConfig();
}
