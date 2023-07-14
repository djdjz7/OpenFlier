using System.IO;
using System.Text;
using System.Windows;
using MQTTnet;
using OpenFlier.Plugin;
using File = System.IO.File;

namespace VerificationPlugin
{
    public class Plugin : IMqttServicePlugin
    {
        public MqttServicePluginInfo GetMqttServicePluginInfo()
        {
            return new MqttServicePluginInfo
            {
                MqttMessageType = 10006L,
                PluginAuthor = "The OpenFlier Authors",
                PluginDescription = "Emulating ZY ClassHelper's DeviceVerification Method.",
                PluginName = "VerificationPlugin",
                PluginIdentifier = "openflier.def.verification",
                PluginNeedConfigEntry = true,
                PluginVersion = "1.0",
                RequestedMinimumOpenFlierVersion = "0.1"
            };
        }

        public MqttApplicationMessage PluginMain(string clientID)
        {
            string payloadString;
            if (File.Exists("Plugins\\VerificationContent"))
                payloadString = File.ReadAllText("Plugin\\VerificationContent");
            else
                payloadString = "{\"type\":20007,\"data\":{\"topic\":\"Ec1xkK+uFtV/QO/8rduJ2A==\"}}";
            return new MqttApplicationMessage
            {
                Topic = clientID,
                Payload = Encoding.Default.GetBytes(payloadString),
                QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce,
            };
        }

        public void PluginOpenConfig()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new PluginConfigWindow().ShowDialog();
            });

        }
    }
}