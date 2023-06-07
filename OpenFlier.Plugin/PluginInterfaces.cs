using MQTTnet;

namespace OpenFlier.Plugin;

public class MqttServicePluginInfo
{
    public required string PluginName
    {
        get; init;
    }
    public required string PluginIdentifier
    {
        get; init;
    }
    public required string PluginAuthor
    {
        get; init;
    }
    public required string PluginVersion
    {
        get; init;
    }
    public required string RequestedMinimumOpenFlierVersion
    {
        get; init;
    }
    public required long MqttMessageType
    {
        get; init;
    }
    public required bool PluginNeedConfigEntry
    {
        get; init;
    }
    public required string PluginDescription
    {
        get; init;
    }
}
public class CommandInputPluginInfo
{
    public required string PluginName
    {
        get; init;
    }
    public required string PluginIdentifier
    {
        get; init;
    }
    public required string PluginAuthor
    {
        get; init;
    }
    public required string PluginVersion
    {
        get; init;
    }
    public required string RequestedMinimumOpenFlierVersion
    {
        get; init;
    }
    public required string[] CommandInputCallerNames
    {
        get; init;
    }
    public required bool PluginNeedConfigEntry
    {
        get; init;
    }
    public required string PluginDescription
    {
        get; init;
    }
}
public interface IMqttServicePlugin
{
    public MqttServicePluginInfo GetMqttServicePluginInfo();
    public MqttApplicationMessage PluginMain(string clientID);
    public void PluginOpenConfig();
}
public interface ICommandInputPlugin
{
    public CommandInputPluginInfo GetCommandInputPluginInfo();
    public void PluginMain(string clientID, bool isUserBusy, bool isUserForcing, string imagePath);
    public void PluginOpenConfig();
}
