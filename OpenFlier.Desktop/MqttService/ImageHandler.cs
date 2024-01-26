using log4net;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Newtonsoft.Json;
using OpenFlier.Core;
using OpenFlier.Core.Services;
using OpenFlier.Desktop.Localization;
using OpenFlier.Desktop.Services;
using OpenFlier.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static PageSnapshot.Types;

namespace OpenFlier.Desktop.MqttService
{
    internal class ImageHandler
    {
        private readonly HttpClient httpClient = new HttpClient();
        private readonly ILog imageHandlerLogger = LogManager.GetLogger(typeof(ImageHandler));
        private readonly Font _msyh = new Font("Microsoft Yahei", 20);

        public Dictionary<string, ICommandInputPlugin> LoadedCommandInputPlugins = new();

        public ImageHandler()
        {
            httpClient.BaseAddress = new Uri("http://friday-note.oss-cn-hangzhou.aliyuncs.com/");
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
            src.Dispose();
        }

        public async Task HandleSpecialChannels(
            User user,
            bool usePng,
            IMqttServer mqttServer,
            string? fullCommand = null
        )
        {
            try
            {
                if (fullCommand is null)
                {
                    if (string.IsNullOrEmpty(user.CommandInputSource))
                        throw new ArgumentException(Backend.CommandInputSourceNotSpecified);
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
                    string payload = JsonConvert.SerializeObject(
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
                            Payload = Encoding.Default.GetBytes(payload),
                            QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce
                        }
                    );
                    return;
                }

                if (user.IsBusy && !forceFlag)
                    throw new UserBusyException(Backend.UserBusy);

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
                                LoadedCommandInputPlugins.Add(
                                    plugin.PluginInfo.PluginIdentifier,
                                    commandInputPlugin
                                );
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
                    user.IsBusy = false;
                }
                else
                    throw new NoCompatiblePluginException(
                        string.Format(Backend.NoCompatiblePlugin, invokeCommand)
                    );
            }
            catch (UserBusyException e)
            {
                ThrowExceptionToUser(e, mqttServer, usePng, user.CurrentClientId!);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                imageHandlerLogger.Error(e.Message, e);
                ThrowExceptionToUser(e, mqttServer, usePng, user.CurrentClientId!);
                user.IsBusy = false;
            }
        }

        private static List<string> GetAllStringFromSnapshot(GraphSnapshot graphSnapshot)
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

        public static void PrintImage(
            string filename,
            string content,
            bool usePng,
            Brush foreground,
            Brush background,
            Font font
        )
        {
            Bitmap bitmap = new Bitmap(1920, 1080);
            Graphics graphics = Graphics.FromImage(bitmap);
            SizeF sf = graphics.MeasureString(content, font, 1920);
            if (sf.Height >= 1080)
            {
                graphics.Dispose();
                bitmap.Dispose();
                bitmap = new Bitmap(1920, (int)Math.Ceiling(sf.Height));
                graphics = Graphics.FromImage(bitmap);
            }
            graphics.FillRectangle(background, 0, 0, 1920, bitmap.Height);
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            graphics.DrawString(
                content,
                font,
                foreground,
                new RectangleF(0, 0, sf.Width, sf.Height)
            );
            graphics.Save();
            bitmap.Save(
                $"Screenshots\\{filename}.{(usePng ? "png" : "jpeg")}",
                usePng ? ImageFormat.Png : ImageFormat.Jpeg
            );
            graphics.Dispose();
            bitmap.Dispose();
        }

        public async void ThrowExceptionToUser(
            Exception exception,
            IMqttServer mqttServer,
            bool usePng,
            string clientID
        )
        {
            var filename = Guid.NewGuid().ToString();
            var content = string.Format(
                Backend.ExceptionCaught,
                exception.GetType().FullName,
                exception.Message,
                exception.StackTrace
            );
            PrintImage(filename, content, usePng, Brushes.OrangeRed, Brushes.White, _msyh);
            string payload = JsonConvert.SerializeObject(
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
                    Topic = clientID + "/REQUEST_SCREEN_CAPTURE",
                    Payload = Encoding.Default.GetBytes(payload),
                    QualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce
                }
            );
            return;
        }
    }
}
