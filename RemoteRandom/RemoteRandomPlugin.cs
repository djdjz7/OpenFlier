using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Newtonsoft.Json;
using OpenFlier.Plugin;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace RemoteRandom
{
    public class RemoteRandomPlugin : ICommandInputPlugin
    {
        public static HttpClient? client = null;
        private CommandInputPluginInfo pluginInfo = new()
        {
            InvokeCommands = new List<string>(new[] { "rr", "RemoteRandom" }),
            PluginDescription = "Fetches a random image from a remote source.",
            PluginVersion = "0.0.1",
            PluginIdentifier = "openflier.ext.remoterandom",
            PluginAuthor = "The OpenFlier Contributors",
            PluginName = "Remote Random",
            PluginNeedsConfigEntry = true,
        };

        public async Task BeforeExit()
        {
            return;
        }

        public CommandInputPluginInfo GetPluginInfo()
        {
            return pluginInfo;
        }

        public async Task PluginMain(CommandInputPluginArgs args)
        {
            if (client is null)
                client = new HttpClient();
            var filename = Guid.NewGuid().ToString("N");
            var usePng = args.UsePng;
            var imageStream = await client.GetStreamAsync("https://api.aixiaowai.cn/api/api.php");
            Image bitmap = Bitmap.FromStream(imageStream);
            bitmap.Save(
                $"Screenshots\\{filename}.{(usePng ? "png" : "jpeg")}",
                usePng ? ImageFormat.Png : ImageFormat.Jpeg
            );
            bitmap.Dispose();
            imageStream.Close();
            string payload = JsonConvert.SerializeObject(
                new
                {
                    type = 20005L,
                    data = new
                    {
                        name = $"{filename}.{(usePng ? "png" : "jpeg")}",
                        deviceCode = args.MachineIdentifier,
                        versionCode = args.Version,
                    }
                }
            );
            await args.MqttServer.PublishAsync(new MqttApplicationMessage
            {
                Topic = args.ClientID + "/REQUEST_SCREEN_CAPTURE",
                Payload = Encoding.Default.GetBytes(payload),
                QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce
            });
        }

        public void PluginOpenConfig()
        {
            throw new NotImplementedException();
        }
    }
}