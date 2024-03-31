using MQTTnet.Server;

namespace OpenFlier.Plugin;

public class PluginInfoBase
{
    public string PluginName { get; init; }
    public string PluginIdentifier { get; init; }
    public string PluginAuthor { get; init; }
    public string PluginVersion { get; init; }
    public bool PluginNeedsConfigEntry { get; init; }
    public string PluginDescription { get; init; }
    public bool PluginNeedsAdminPrivilege { get; init; }
}
public class MqttServicePluginInfo : PluginInfoBase
{
    public long MqttMessageType { get; init; }
}

public interface IMqttServicePlugin
{
    public MqttServicePluginInfo GetPluginInfo();
    public Task PluginMain(string clientID, IMqttServer mqttServer);
    public void PluginOpenConfig();
    public Task BeforeExit();
}

public class CommandInputPluginInfo : PluginInfoBase
{
    public IEnumerable<string> InvokeCommands { get; init; }
}

public class CommandInputPluginArgs
{
    public string ClientID { get; init; }
    public string InvokeCommand { get; init; }
    public string FullCommand { get; init; }
    public IMqttServer MqttServer { get; init; }
    public bool UsePng { get; init; }
    public string MachineIdentifier { get; init; }
    public string Version { get; init; }

}

public interface ICommandInputPlugin
{
    public CommandInputPluginInfo GetPluginInfo();
    public Task PluginMain(CommandInputPluginArgs args);
    public void PluginOpenConfig();
    public Task BeforeExit();
}
