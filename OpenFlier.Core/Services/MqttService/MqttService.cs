using log4net;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Newtonsoft.Json;
using OpenFlier.Plugin;
using System.Text;

namespace OpenFlier.Core.Services
{
    public class MqttService
    {
        public Dictionary<string, IMqttServicePlugin> LoadedMqttServicePlugins = new();
        public IMqttServer? MqttServer { get; set; }
        public ILog MqttLogger { get; set; } = LogManager.GetLogger(typeof(MqttService));
        public string MainTopic { get; } = Guid.NewGuid().ToString("N");

        public delegate Task MqttServerClientConnectedDelegate(
            MqttServerClientConnectedEventArgs arg
        );
        public event MqttServerClientConnectedDelegate? OnClientConnected;

        public delegate Task MqttApplicationMessageReceivedDelegate(
            MqttApplicationMessageReceivedEventArgs arg
        );
        public event MqttApplicationMessageReceivedDelegate? OnScreenshotRequestReceived;
        public event MqttApplicationMessageReceivedDelegate? OnTestRequestReceived;

        public delegate Task MqttServerClientDisconnectedDelegate(
            MqttServerClientDisconnectedEventArgs arg
        );
        public event MqttServerClientDisconnectedDelegate? OnClientDisconnected;

        public async void Initialize()
        {
            MqttServerOptionsBuilder optionsBuilder = new MqttServerOptionsBuilder()
                .WithConnectionBacklog(2000)
                .WithDefaultEndpointPort(CoreStorage.CoreConfig.MqttServerPort ?? 61136);
            MqttServer = new MqttFactory().CreateMqttServer();
            MqttServer.UseApplicationMessageReceivedHandler(OnAppMessageReceivedAsync);
            MqttServer.UseClientConnectedHandler(OnClientConnectedAsync);
            MqttServer.UseClientDisconnectedHandler(OnClientDisConnectedAsync);
            await Task.Run(() => MqttServer.StartAsync(optionsBuilder.Build()));
        }

        private async Task OnClientDisConnectedAsync(MqttServerClientDisconnectedEventArgs arg)
        {
            OnClientDisconnected?.Invoke(arg);
        }

        private async Task OnClientConnectedAsync(MqttServerClientConnectedEventArgs arg)
        {
            OnClientConnected?.Invoke(arg);
        }

        private async Task OnAppMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            string clientID = arg.ClientId;
            byte[] payload = arg.ApplicationMessage.Payload;
            if (payload.Length == 0)
                return;
            MqttMessage<object>? message = JsonConvert.DeserializeObject<MqttMessage<object>>(
                Encoding.Default.GetString(payload)
            );
            if (message == null)
                return;
            MqttMessageType messageType = message.Type;

            switch (messageType)
            {
                case MqttMessageType.ScreenCaptureResp:
                    break;
                case MqttMessageType.StudentTopic:
                    string s3 = JsonConvert.SerializeObject(
                        new
                        {
                            type = MqttMessageType.TeacherScreenCastResp,
                            data = new { topic = MainTopic }
                        }
                    );
                    await MqttServer.PublishAsync(
                        new MqttApplicationMessage
                        {
                            Topic = arg.ClientId,
                            Payload = Encoding.Default.GetBytes(s3),
                            QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce
                        }
                    );
                    break;

                case MqttMessageType.ScreenCaptureReq:
                    OnScreenshotRequestReceived?.Invoke(arg);
                    break;

                case MqttMessageType.DeviceVerificationReq:
                    string s4 = CoreStorage.CoreConfig.VerificationContent;
                    await MqttServer.PublishAsync(
                        new MqttApplicationMessage
                        {
                            Topic = clientID,
                            Payload = Encoding.Default.GetBytes(s4),
                            QualityOfServiceLevel = MQTTnet
                                .Protocol
                                .MqttQualityOfServiceLevel
                                .ExactlyOnce,
                        }
                    );
                    break;

                case MqttMessageType.TestReq:
                    OnTestRequestReceived?.Invoke(arg);
                    break;

                default:
                    IMqttServicePlugin? mqttServicePlugin = null;
                    foreach (var plugin in CoreStorage.CoreConfig.MqttServicePlugins)
                    {
                        if (plugin.PluginInfo.MqttMessageType != (long)messageType)
                            continue;
                        if (!plugin.Enabled || plugin.TempDisabled)
                            continue;
                        if (!File.Exists(plugin.LocalFilePath))
                        {
                            MqttLogger.Warn(
                                $"Got message {messageType}, attempt to load pluginInfo {plugin.LocalFilePath} failed: File not found."
                            );
                            continue;
                        }
                        try
                        {
                            if (
                                !LoadedMqttServicePlugins.TryGetValue(
                                    plugin.PluginInfo.PluginIdentifier,
                                    out mqttServicePlugin
                                )
                            )
                            {
                                var loadContext = new PluginLoadContext(plugin.LocalFilePath);
                                var assemblyFileInfo = new FileInfo(plugin.LocalFilePath);
                                var assembly = loadContext.LoadFromAssemblyPath(assemblyFileInfo.FullName);
                                var types = assembly.GetTypes();
                                foreach (Type type in types)
                                {
                                    if (type.GetInterface("IMqttServicePlugin") == null)
                                        continue;
                                    if (type.FullName == null)
                                        continue;
                                    mqttServicePlugin = (IMqttServicePlugin?)
                                        assembly.CreateInstance(type.FullName);
                                    if (mqttServicePlugin == null)
                                        continue;

                                    LoadedMqttServicePlugins.Add(plugin.PluginInfo.PluginIdentifier, mqttServicePlugin);
                                    MqttLogger.Info($"Loaded plugin {plugin.LocalFilePath}");
                                    goto endOfPluginSearch;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            MqttLogger.Error(
                                $"Got message {messageType}, attempt to load pluginInfo {plugin.LocalFilePath} failed.",
                                e
                            );
                        }
                    }
                endOfPluginSearch:
                    if (mqttServicePlugin is not null)
                        await mqttServicePlugin.PluginMain(arg.ClientId, MqttServer!);
                    else
                        MqttLogger.Info($"No compatible plugin for message {messageType}");
                    break;
            }
        }
    }
}
