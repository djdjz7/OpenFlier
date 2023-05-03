using MQTTnet;

namespace OpenFlier.Plugin
{
    public class MqttServicePluginInfo
    {
        required public string PluginName { get; init; }
        required public string PluginAuthor { get; init; }
        required public string PluginVersion { get; init; }
        required public string RequestedMinimumOpenFlierVersion { get; init; }
        required public long MqttMessageType { get; init; }
        required public bool PluginNeedConfigEntry { get; init; }
        required public string PluginDescription { get; init; }
    }
    public interface IMqttServicePlugin
    {
        public MqttServicePluginInfo GetMqttServicePluginInfo();
        public MqttApplicationMessage PluginMain(string clientID);
        public void PluginOpenConfig();
    }
}
