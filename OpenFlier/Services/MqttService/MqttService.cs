using log4net;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Newtonsoft.Json;
using OpenFlier.Plugin;
using OpenFlier.SpecialChannels;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OpenFlier.Services
{
    public static class MqttService
    {
        public static IMqttServer? MqttServer { get; set; }
        public static List<User> Users { get; set; } = new List<User>();
        public static ILog MqttLogger { get; set; } = LogManager.GetLogger(typeof(MqttService));

        public static string MainTopic { get; } = Guid.NewGuid().ToString("N");
        public async static void Initialize()
        {
            MqttServerOptionsBuilder optionsBuilder = new MqttServerOptionsBuilder().WithConnectionBacklog(2000).WithDefaultEndpointPort(LocalStorage.Config.General.MqttServerPort ?? 61136);
            MqttServer = new MqttFactory().CreateMqttServer();
            MqttServer.UseApplicationMessageReceivedHandler(OnAppMessageReceivedAsync);
            MqttServer.UseClientConnectedHandler(OnClientConnectedAsync);
            MqttServer.UseClientDisconnectedHandler(OnClientDisConnectedAsync);
            await Task.Run(() => MqttServer.StartAsync(optionsBuilder.Build()));
        }

        private static Task OnClientDisConnectedAsync(MqttServerClientDisconnectedEventArgs arg)
        {
            return Task.Run(() =>
            {
                string userName = arg.ClientId.Split('_')[2];
                User connectedUser = Users.First(x => x.UserName == userName);
                if (DateTime.Now - connectedUser.LastUpdateTime >= TimeSpan.FromSeconds(5))
                {
                    connectedUser.SwitchChannel();
                    connectedUser.LastUpdateTime = DateTime.Now;
                }
            });
        }

        private static Task OnClientConnectedAsync(MqttServerClientConnectedEventArgs arg)
        {
            return Task.Run(() =>
            {
                //A typical clientId: sxz_00000_XXX_1_XXXXXXXXXXXXXXXXXXXXXXXX
                string userName = arg.ClientId.Split('_')[2];
                if (!Users.Contains(new User { UserName = userName }, new UserEqualityComparer()))
                {
                    Users.Add(new User
                    {
                        UserName = userName,
                        UserID = arg.ClientId.Split('_')[1],
                        CurrentChannel = 0,
                        AllowLocalRandom = LocalStorage.Config.SpecialChannels.LocalRandomAllowedUsers.Contains(userName),
                        AllowRemoteRandom = LocalStorage.Config.SpecialChannels.RemoteRandomAllowedUsers.Contains(userName),
                        AllowCommandInput = LocalStorage.Config.SpecialChannels.CommandInputAllowedUsers.Contains(userName),
                    });
                    MqttLogger.Info($"New user connected: {arg.ClientId}");
                }
            });
        }

        private static Task OnAppMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            return Task.Factory.StartNew(async () =>
            {
                string clientID = arg.ClientId;
                byte[] payload = arg.ApplicationMessage.Payload;
                if (payload.Length == 0)
                    return;
                MqttMessage<object>? message = JsonConvert.DeserializeObject<MqttMessage<object>>(Encoding.Default.GetString(payload));
                if (message == null)
                    return;
                MqttMessageType messageType = message.Type;
                switch (messageType)
                {
                    case MqttMessageType.StudentTopic:
                        string s3 = JsonConvert.SerializeObject(new
                        {
                            type = MqttMessageType.TeacherScreenCastResp,
                            data = new
                            {
                                topic = MainTopic
                            }
                        });
                        await MqttServer.PublishAsync(new MqttApplicationMessage
                        {
                            Topic = arg.ClientId,
                            Payload = Encoding.Default.GetBytes(s3),
                            QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce
                        });
                        break;

                    case MqttMessageType.ScreenCaptureReq:
                        Rectangle rect = LocalStorage.ScreenSize;
                        Bitmap src = new Bitmap(rect.Width, rect.Height);
                        Graphics graphics = Graphics.FromImage(src);
                        graphics.CopyFromScreen(0, 0, 0, 0, rect.Size);
                        graphics.Save();
                        graphics.Dispose();
                        string filename = Guid.NewGuid().ToString("N");
                        if (LocalStorage.Config.General.UsePng)
                        {
                            filename = $"{filename}.png";
                            src.Save($"Screenshots\\{filename}", System.Drawing.Imaging.ImageFormat.Png);
                        }
                        else
                        {
                            filename = $"{filename}.jpeg";
                            src.Save($"Screenshots\\{filename}", System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                        string s5 = JsonConvert.SerializeObject(new
                        {
                            type = MqttMessageType.ScreenCaptureResp,
                            data = new
                            {
                                name = filename,
                                deviceCode = LocalStorage.MachineIdentifier,
                                versionCode = LocalStorage.Version
                            }
                        });
                        await MqttServer.PublishAsync(new MqttApplicationMessage
                        {
                            Topic = arg.ClientId + "/REQUEST_SCREEN_CAPTURE",
                            Payload = Encoding.Default.GetBytes(s5),
                            QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce
                        });
                        break;
                    default:
                        foreach (var pluginInfo in LocalStorage.Config.MqttServicePlugins)
                        {
                            if (pluginInfo.MqttMessageType != (long)messageType)
                                continue;
                            if (!File.Exists(pluginInfo.PluginFilePath))
                            {
                                MqttLogger.Warn($"Got message {messageType}, attempt to load pluginInfo {pluginInfo.PluginFilePath} failed: File not found.");
                                continue;
                            }
                            try
                            {
                                FileInfo assemblyFileInfo= new FileInfo(pluginInfo.PluginFilePath);
                                Assembly assembly = Assembly.LoadFrom(assemblyFileInfo.FullName);
                                Type[] types = assembly.GetTypes();
                                foreach (Type type in types)
                                {
                                    if (type.GetInterface("IMqttServicePlugin") == null)
                                        continue;
                                    if (type.FullName == null)
                                        continue;
                                    IMqttServicePlugin? mqttServicePlugin = (IMqttServicePlugin?)assembly.CreateInstance(type.FullName);
                                    if (mqttServicePlugin == null)
                                        continue;
                                    MqttApplicationMessage mqttMessage = mqttServicePlugin.PluginMain(arg.ClientId);
                                    await MqttServer.PublishAsync(mqttMessage);

                                }
                            }
                            catch (Exception e)
                            {
                                MqttLogger.Error($"Got message {messageType}, attempt to load pluginInfo {pluginInfo.PluginFilePath} failed.", e);
                            }

                        }
                        break;
                }
            });
        }
        private class UserEqualityComparer : IEqualityComparer<User>
        {
            public bool Equals(User? x, User? y)
            {
                if (x?.UserName == y?.UserName)
                    return true;
                return false;
            }

            public int GetHashCode([DisallowNull] User obj)
            {
                return obj.UserName.GetHashCode();
            }
        }
        private static void SwitchChannel(this User user)
        {
            //ComputerScreenShot is always allowed.
            //Following ifs are excuted in order, so that CurrentChannel can always fall back to 0.
            if (user.CurrentChannel == SpecialChannel.CommandInput)
            {
                return;
            }
            user.CurrentChannel = (SpecialChannel)((((int)user.CurrentChannel) + 1) % 4);
            if (user.CurrentChannel == SpecialChannel.LocalRandom && !user.AllowLocalRandom)
                user.CurrentChannel = SpecialChannel.RemoteRandom;
            if (user.CurrentChannel == SpecialChannel.RemoteRandom && !user.AllowRemoteRandom)
                user.CurrentChannel = SpecialChannel.CommandInput;
            if (user.CurrentChannel == SpecialChannel.CommandInput && !user.AllowCommandInput)
                user.CurrentChannel = SpecialChannel.Screenshot;
            user.LastUpdateTime = DateTime.Now;
        }

    }
}
