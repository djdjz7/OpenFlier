using System.Text;
using System.Windows;
using MQTTnet;
using MQTTnet.Server;
using OpenFlier.Plugin;

namespace DemoPlugin
{
    public class DemoPlugin : IMqttServicePlugin
    {
        public MqttServicePluginInfo GetPluginInfo()
        {
            return new MqttServicePluginInfo
            {
                MqttMessageType = 30000L,
                PluginAuthor = "The OpenFlier Authors",
                PluginName = "DemoPlugin",
                PluginIdentifier = "openflier.dev.demo",
                PluginNeedsConfigEntry = false,
                PluginVersion = "0.1",
                PluginDescription = "Testing plugin functionality."
            };
        }


        public async Task PluginMain(string clientID, IMqttServer mqttServer)
        {
            MessageBox.Show("A message from DemoPlugin");
            string payloadString = "{\"data\":null,\"type\":30001}";
            await mqttServer.PublishAsync(new MqttApplicationMessage
            {
                Topic = "test",
                Payload = Encoding.Default.GetBytes(payloadString),
                QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce
            });
        }

        public void PluginOpenConfig()
        {
            return;
        }
    }
}