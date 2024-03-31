﻿using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace OpenFlier.Desktop.MqttService
{
    internal class PluginLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath)
        {
            var fileInfo = new FileInfo(pluginPath);
            _resolver = new AssemblyDependencyResolver(fileInfo.FullName);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath is null)
                return null;
            return LoadFromAssemblyPath(assemblyPath);
        }
    }
}
