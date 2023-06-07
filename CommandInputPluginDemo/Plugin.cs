using System.Diagnostics;
using OpenFlier.Plugin;

namespace CommandInputPluginDemo;

public class Plugin : ICommandInputPlugin
{
    public CommandInputPluginInfo GetCommandInputPluginInfo()
    {
        return new CommandInputPluginInfo
        {
            CommandInputCallerNames = new string[] { "demo" },
            PluginNeedConfigEntry = false,
            PluginAuthor = "The OpenFlier Contributors",
            PluginDescription = "Testing CommandInputPlugin functions.",
            PluginIdentifier = "openflier.dev.cmddemo",
            PluginName = "CommandInputPlugin Demo",
            PluginVersion = "1.0",
            RequestedMinimumOpenFlierVersion = "0.1",
        };
    }
    public void PluginMain(string clientID, bool isUserBusy, bool isUserForcing, string imagePath)
    {
        Debug.WriteLine("OK");
    }
    public void PluginOpenConfig() => throw new NotImplementedException();
}
