using MQTTnet;

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

public class ImageProviderPluginInfo : PluginInfoBase
{

}

public interface IMqttServicePlugin
{
    public MqttServicePluginInfo GetMqttServicePluginInfo();
    public MqttApplicationMessage PluginMain(string clientID);
    public void PluginOpenConfig();
}

public interface IImageProviderPlugin
{
    public ImageProviderPluginInfo GetImageProviderPluginInfo();
    public object PluginMain(string clientID);
    public void PluginOpenConfig();
}