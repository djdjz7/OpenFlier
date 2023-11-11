using log4net;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Newtonsoft.Json;
using OpenFlier.Core;
using OpenFlier.Core.Services;
using OpenFlier.Desktop.MqttService;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFlier.Desktop.Services
{
    public static class MqttService
    {
        public static IMqttServer? MqttServer { get; set; }
        public static List<User> Users { get; set; } = new List<User>();
        public static ILog MqttLogger { get; set; } = LogManager.GetLogger(typeof(MqttService));
        private static ImageHandler imageHandler = new();

        public static string MainTopic { get; } = Guid.NewGuid().ToString("N");

        public static Task OnClientDisConnectedAsync(MqttServerClientDisconnectedEventArgs arg)
        {
            return Task.Run(() =>
            {
                string Username = arg.ClientId.Split('_')[2];
                User connectedUser = Users.First(x => x.Username == Username);
                connectedUser.CurrentClientId = "";
            });
        }

        public static Task OnClientConnectedAsync(MqttServerClientConnectedEventArgs arg)
        {
            return Task.Run(() =>
            {
                //A typical clientId: sxz_00000_XXX_1_XXXXXXXXXXXXXXXXXXXXXXXX
                string clientID = arg.ClientId;
                string Username = clientID.Split('_')[2];
                bool allowCommandInput = false;
                string? commandInputSource = null;
                foreach (var cmdUser in LocalStorage.Config.CommandInputUsers)
                    if (
                        cmdUser.UserIdentifier != null
                        && cmdUser.UserIdentifier != ""
                        && clientID.Contains(cmdUser.UserIdentifier)
                    )
                    {
                        allowCommandInput = true;
                        commandInputSource = cmdUser.CommandInputSource;
                    }

                if (!Users.Contains(new User { Username = Username }, new UserEqualityComparer()))
                {
                    Users.Add(
                        new User
                        {
                            Username = Username,
                            UserId = clientID.Split('_')[1],
                            CurrentClientId = clientID,
                            AllowCommandInput = allowCommandInput,
                            CommandInputSource = commandInputSource,
                        }
                    );
                    MqttLogger.Info($"New user connected: {arg.ClientId}");
                }
                else
                {
                    var u = Users.FirstOrDefault(x => x.Username == Username);
                    if (u is not null)
                    {
                        u.CurrentClientId = arg.ClientId;
                    }
                }
            });
        }

        public static async Task OnAppMessageReceivedAsync(
            MqttApplicationMessageReceivedEventArgs arg
        )
        {
            bool usePng = LocalStorage.Config.General.UsePng;
            string Username = arg.ClientId.Split('_')[2];
            var user = Users.FirstOrDefault(x => x.Username == Username);
            string filename = Guid.NewGuid().ToString("N");
            if (!user!.AllowCommandInput)
            {
                imageHandler.FetchScreenshot(filename, usePng);
            }
            else
            {
                await imageHandler.HandleSpecialChannels(user, filename, usePng, MqttServer!);
            }

            string s5 = JsonConvert.SerializeObject(
                new
                {
                    type = MqttMessageType.ScreenCaptureResp,
                    data = new
                    {
                        name = $"{filename}.{(usePng ? "png" : "jpeg")}",
                        deviceCode = CoreStorage.MachineIdentifier,
                        versionCode = CoreStorage.Version
                    }
                }
            );
            await MqttServer.PublishAsync(
                new MqttApplicationMessage
                {
                    Topic = arg.ClientId + "/REQUEST_SCREEN_CAPTURE",
                    Payload = Encoding.Default.GetBytes(s5),
                    QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce
                }
            );
        }

        private class UserEqualityComparer : IEqualityComparer<User>
        {
            public bool Equals(User? x, User? y)
            {
                if (x?.Username == y?.Username)
                    return true;
                return false;
            }

            public int GetHashCode([DisallowNull] User obj)
            {
                return obj.Username.GetHashCode();
            }
        }
    }
}
