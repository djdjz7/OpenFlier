using log4net;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Newtonsoft.Json;
using OpenFlier.Core;
using OpenFlier.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OpenFlier.Desktop.Services
{
    public static class MqttService
    {
        public static IMqttServer? MqttServer
        {
            get; set;
        }
        public static List<User> Users { get; set; } = new List<User>();
        public static ILog MqttLogger { get; set; } = LogManager.GetLogger(typeof(MqttService));

        public static string MainTopic { get; } = Guid.NewGuid().ToString("N");

        public static Task OnClientDisConnectedAsync(MqttServerClientDisconnectedEventArgs arg)
        {
            return Task.Run(() =>
            {
                string Username = arg.ClientId.Split('_')[2];
                User connectedUser = Users.First(x => x.Username == Username);
                if (DateTime.Now - connectedUser.LastUpdateTime >= TimeSpan.FromSeconds(5))
                {
                    connectedUser.SwitchImageProvider();
                    connectedUser.LastUpdateTime = DateTime.Now;
                }
                connectedUser.CurrentClientId = "";
            });
        }

        public static Task OnClientConnectedAsync(MqttServerClientConnectedEventArgs arg)
        {
            return Task.Run(() =>
            {
                //A typical clientId: sxz_00000_XXX_1_XXXXXXXXXXXXXXXXXXXXXXXX
                string Username = arg.ClientId.Split('_')[2];
                if (!Users.Contains(new User { Username = Username }, new UserEqualityComparer()))
                {
                    Users.Add(new User
                    {
                        Username = Username,
                        UserId = arg.ClientId.Split('_')[1],
                        CurrentImageProvider = 0,
                        CurrentClientId = arg.ClientId,
                    });
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

        public static async Task OnAppMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
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
                    deviceCode = CoreStorage.MachineIdentifier,
                    versionCode = CoreStorage.Version
                }
            });
            await MqttServer.PublishAsync(new MqttApplicationMessage
            {
                Topic = arg.ClientId + "/REQUEST_SCREEN_CAPTURE",
                Payload = Encoding.Default.GetBytes(s5),
                QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce
            });
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
