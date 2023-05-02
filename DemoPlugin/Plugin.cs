using MQTTnet;
using OpenFlier.Plugin;
using System.Windows;

namespace DemoPlugin
{
    public class DemoPlugin : IMqttServicePlugin
    {
        public MqttServicePluginInfo GetMqttServicePluginInfo()
        {
            return new MqttServicePluginInfo
            {
                MqttMessageType = 30000L,
                PluginAuthor = "The OpenFlier Authors",
                PluginName = "DemoPlugin",
                PluginNeedConfigEntry = false,
                PluginVersion = "0.1",
                RequestedMinimumOpenFlierVersion = "0.1",
                PluginDescription = "Testing plugin functionality."
            };
        }

        public MqttApplicationMessage PluginMain()
        {
            MessageBox.Show("A message from DemoPlugin");
            MqttApplicationMessage mqttApplicationMessage = new MqttApplicationMessage();
            mqttApplicationMessage.Topic = "test";
            mqttApplicationMessage.Payload = Array.Empty<byte>();
            mqttApplicationMessage.QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce;
            return mqttApplicationMessage;
        }

        public void PluginOpenConfig()
        {
            return;
        }
    }
}