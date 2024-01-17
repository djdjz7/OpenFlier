using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

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
