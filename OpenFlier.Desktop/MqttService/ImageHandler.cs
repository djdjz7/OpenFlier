using MQTTnet.Server;
using OpenFlier.Desktop.Services;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http;
using System.Threading.Tasks;
using OpenFlier.Desktop.Localization;
using static PageSnapshot.Types;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using OpenFlier.Plugin;
using System.IO;
using System.Reflection;
using OpenFlier.Core;
using MQTTnet.Protocol;
using MQTTnet;
using Newtonsoft.Json;
using OpenFlier.Core.Services;
using System.Text;
using log4net;

namespace OpenFlier.Desktop.MqttService
{
    internal class ImageHandler
    {
        private HttpClient httpClient = new HttpClient();
        private ILog imageHandlerLogger = LogManager.GetLogger(typeof(ImageHandler));
        public ImageHandler()
        {
            httpClient.BaseAddress = new System.Uri(
                "http://friday-note.oss-cn-hangzhou.aliyuncs.com/"
            );
        }

        public void FetchScreenshot(string filename, bool usePng)
        {
            Rectangle rect = LocalStorage.ScreenSize;
            Bitmap src = new Bitmap(rect.Width, rect.Height);
            Graphics graphics = Graphics.FromImage(src);
            graphics.CopyFromScreen(0, 0, 0, 0, rect.Size);
            graphics.Save();
            graphics.Dispose();
            src.Save(
                $"Screenshots\\{filename}.{(usePng ? "png" : "jpeg")}",
                usePng ? ImageFormat.Png : ImageFormat.Jpeg
            );
        }

        public async Task HandleSpecialChannels(User user, bool usePng, IMqttServer mqttServer)
        {
            try
            {
                if (string.IsNullOrEmpty(user.CommandInputSource))
                    throw new ArgumentException(Backend.CommandInputSourceNotSpecified);
                var snapshotFile = await httpClient.GetByteArrayAsync(user.CommandInputSource);
                var snapshot = PageSnapshot.Parser.ParseFrom(snapshotFile);
                var stringList = GetAllStringFromSnapshot(snapshot.GraphSnapshot);
                string fullCommand = string.Join(' ', stringList).Trim();
                var commands = new List<string>(fullCommand.Split(" "));
                if (commands.Count == 0)
                    throw new ArgumentException(Backend.CommandEmpty);

                bool forceFlag = false;
                if (commands[0].ToLower() == "force")
                {
                    forceFlag = true;
                    commands.RemoveAt(0);
                }
                if (commands.Count == 0)
                    throw new ArgumentException(Backend.CommandEmpty);

                var invokeCommand = commands[0];

                if (invokeCommand.ToLower() == "ss" || invokeCommand.ToLower() == "screenshot")
                {
                    string filename = Guid.NewGuid().ToString("N");
                    FetchScreenshot(filename, usePng);
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
                    await mqttServer.PublishAsync(
                        new MqttApplicationMessage
                        {
                            Topic = user.CurrentClientId + "/REQUEST_SCREEN_CAPTURE",
                            Payload = Encoding.Default.GetBytes(s5),
                            QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce
                        }
                    );
                    return;
                }

                if (user.IsBusy && !forceFlag)
                    throw new Exception(Backend.UserBuzy);

                user.IsBusy = true;
                bool success = false;
                foreach (var plugin in LocalStorage.Config.CommandInputPlugins)
                {
                    if (!plugin.Enabled || plugin.TempDisabled)
                        continue;
                    if (
                        plugin.PluginInfo.InvokeCommands.Contains(
                            invokeCommand.ToLower(),
                            new InvokeCommandEqualityComparer()
                        )
                    )
                    {
                        FileInfo assemblyFileInfo = new FileInfo(plugin.LocalFilePath);
                        var assembly = Assembly.LoadFrom(assemblyFileInfo.FullName);
                        Type[] types = assembly.GetTypes();
                        foreach (Type type in types)
                        {
                            if (type.GetInterface("ICommandInputPlugin") == null)
                                continue;
                            if (type.FullName == null)
                                continue;
                            var commandInputPlugin = (ICommandInputPlugin?)
                                assembly.CreateInstance(type.FullName);
                            if (commandInputPlugin == null)
                                continue;
                            await commandInputPlugin.PluginMain(
                                new CommandInputPluginArgs
                                {
                                    ClientID = user.CurrentClientId!,
                                    InvokeCommand = invokeCommand,
                                    FullCommand = fullCommand,
                                    MqttServer = mqttServer,
                                    UsePng = usePng,
                                    MachineIdentifier = CoreStorage.MachineIdentifier,
                                    Version = CoreStorage.Version,
                                }
                            );
                            success = true;
                        }
                    }
                }
                user.IsBusy = false;
                if (!success)
                    throw new Exception(Backend.UserCommandFailed);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                imageHandlerLogger.Error(e.Message, e);
            }
        }

        private List<string> GetAllStringFromSnapshot(GraphSnapshot graphSnapshot)
        {
            var list = new List<string> { graphSnapshot.Content };
            foreach (var childGraph in graphSnapshot.ChildGraph)
            {
                foreach (var childContent in GetAllStringFromSnapshot(childGraph))
                {
                    list.Add(childContent);
                }
            }
            return list;
        }

        private class InvokeCommandEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string? x, string? y)
            {
                if (x == y)
                    return true;
                if (x?.ToLower() == y?.ToLower())
                    return true;
                return false;
            }

            public int GetHashCode([DisallowNull] string obj)
            {
                return obj.ToLower().GetHashCode();
            }
        }
    }
}
