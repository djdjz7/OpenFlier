﻿using MQTTnet.Server;
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

namespace OpenFlier.Desktop.MqttService
{
    internal class ImageHandler
    {
        private HttpClient httpClient = new HttpClient();

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

        public async Task HandleSpecialChannels(
            User user,
            string filename,
            bool usePng,
            IMqttServer mqttServer
        )
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
            if (commands[0].ToLower()== "force")
            {
                forceFlag = true;
                commands.RemoveAt(0);
            }
            if (commands.Count == 0)
                throw new ArgumentException(Backend.CommandEmpty);

            var invokeCommand = commands[0];

            if (user.IsBusy && !forceFlag)
                throw new Exception(Backend.UserBuzy);

            user.IsBusy = true;
            bool success = false;
            foreach (var plugin in LocalStorage.Config.CommandInputPlugins)
            {
                if (plugin.PluginInfo.InvokeCommands.Contains(invokeCommand.ToLower(), new InvokeCommandEqualityComparer()))
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
                        var commandInputPlugin = (ICommandInputPlugin?)assembly.CreateInstance(type.FullName);
                        if (commandInputPlugin == null)
                            continue;
                        commandInputPlugin.PluginMain(user.CurrentClientId!, invokeCommand, mqttServer, usePng);
                        success = true;
                    }
                }
            }
            user.IsBusy = false;
            if (!success)
                throw new Exception(Backend.UserCommandFailed);
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
                if(x == y) return true;
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
