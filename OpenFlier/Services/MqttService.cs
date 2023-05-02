using log4net;
using MQTTnet;
using MQTTnet.Server;
using OpenFlier.SpecialChannels;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OpenFlier.Services
{
    public static class MqttService
    {
        public static IMqttServer? MqttServer { get; set; }
        public static List<User> Users { get; set; } = new List<User>();
        public static ILog MqttLogger { get; set; } = LogManager.GetLogger(typeof(MqttService));
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
            return Task.Run(()=>
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
            return Task.Run(() =>
            {
                MessageBox.Show(arg.ApplicationMessage.Topic);
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
