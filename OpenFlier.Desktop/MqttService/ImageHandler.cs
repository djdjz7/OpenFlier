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

        public Dictionary<string, ICommandInputPlugin> LoadedCommandInputPlugins = new();

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

        public async Task HandleSpecialChannels(User user, bool usePng, IMqttServer mqttServer, string? fullCommand = null)
        {
            try
            {
                if (string.IsNullOrEmpty(user.CommandInputSource))
                    throw new ArgumentException(Backend.CommandInputSourceNotSpecified);

                if (fullCommand is null)
                {
                    var snapshotFile = await httpClient.GetByteArrayAsync(user.CommandInputSource);
                    var snapshot = PageSnapshot.Parser.ParseFrom(snapshotFile);
                    var stringList = GetAllStringFromSnapshot(snapshot.GraphSnapshot);
                    fullCommand = string.Join(' ', stringList).Trim();
                }

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
                ICommandInputPlugin? commandInputPlugin = null;
                foreach (var plugin in LocalStorage.Config.CommandInputPlugins)
                {
                    if (!File.Exists(plugin.LocalFilePath))
                        continue;
                    if (!plugin.Enabled || plugin.TempDisabled)
                        continue;
                    if (
                        plugin.PluginInfo.InvokeCommands.Contains(
                            invokeCommand.ToLower(),
                            new InvokeCommandEqualityComparer()
                        )
                    )
                    {
                        if (
                            !LoadedCommandInputPlugins.TryGetValue(
                                plugin.PluginInfo.PluginIdentifier,
                                out commandInputPlugin
                            )
                        )
                        {
                            var fileInfo = new FileInfo(plugin.LocalFilePath);
                            var loadContext = new PluginLoadContext(plugin.LocalFilePath);
                            var assembly = loadContext.LoadFromAssemblyPath(fileInfo.FullName);
                            var types = assembly.GetTypes();
                            foreach (Type type in types)
                            {
                                if (type.GetInterface("ICommandInputPlugin") == null)
                                    continue;
                                if (type.FullName == null)
                                    continue;
                                commandInputPlugin = (ICommandInputPlugin?)
                                    assembly.CreateInstance(type.FullName);
                                if (commandInputPlugin == null)
                                    continue;
                                LoadedCommandInputPlugins.Add(plugin.PluginInfo.PluginIdentifier, commandInputPlugin);
                                goto endOfPluginSearch;
                            }
                        }

                    }
                }
            endOfPluginSearch:
                if (commandInputPlugin is not null)
                {
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
                }
                else
                    throw new Exception(string.Format(Backend.NoCompatiblePlugin, invokeCommand));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                imageHandlerLogger.Error(e.Message, e);
            }
            finally
            {
                user.IsBusy = false;
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
