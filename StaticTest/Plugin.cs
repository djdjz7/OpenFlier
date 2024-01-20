using OpenFlier.Plugin;
using System.Runtime.InteropServices;

namespace StaticTest
{
    public class StaticTestPlugin : ICommandInputPlugin
    {
        public CommandInputPluginInfo GetPluginInfo()
        {
            return new CommandInputPluginInfo()
            {
                InvokeCommands = new List<string>{ "st" },
                PluginAuthor = "",
                PluginDescription = "",
                PluginIdentifier = "openflier.dev.statictest",
                PluginName = "Static Test",
                PluginNeedsAdminPrivilege = false,
                PluginNeedsConfigEntry = false,
                PluginVersion = "0.0.1"
            };
        }

        public async Task PluginMain(CommandInputPluginArgs args)
        {
            StaticClass.SomeValue++;
            MessageBox(IntPtr.Zero, StaticClass.SomeValue.ToString(), StaticClass.SomeValue.ToString(), 0);
        }

        public void PluginOpenConfig()
        {
            throw new NotImplementedException();
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int MessageBox(IntPtr hwnd, string lpText, string lpCaption, uint uType);
    }
}
