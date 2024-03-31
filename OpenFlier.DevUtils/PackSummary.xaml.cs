using Google.Protobuf;
using OpenFlier.Plugin;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace OpenFlier.DevUtils;

/// <summary>
/// PackSave.xaml 的交互逻辑
/// </summary>
public partial class PackSummary : Page
{
    private string _zipPath;
    private string _zipExtractedPath;
    private string? _pluginIdentifier;
    private FileSystemEntry _pluginEntry;
    private MqttServicePluginInfo? _mqttServicePluginInfo;
    private CommandInputPluginInfo? _commandInputPluginInfo;
    private PluginType? _pluginType = null;

    public enum PluginType
    {
        MqttServicePlugin = 0,
        CommandInputPlugin = 1,
    }

    public PackSummary(string zipPath, string zipExtractedPath, FileSystemEntry pluginEntry)
    {
        InitializeComponent();
        _zipPath = zipPath;
        _zipExtractedPath = zipExtractedPath;
        _pluginEntry = pluginEntry;
        var loadContext = new PluginLoadContext(_pluginEntry.FullName);
        try
        {
            FileInfo assemblyFileInfo = new FileInfo(_pluginEntry.FullName);
            var assembly = loadContext.LoadFromAssemblyPath(assemblyFileInfo.FullName);
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.GetInterface("IMqttServicePlugin") == null)
                    continue;
                if (type.FullName == null)
                    continue;
                IMqttServicePlugin? mqttServicePlugin = (IMqttServicePlugin?)
                    assembly.CreateInstance(type.FullName);
                if (mqttServicePlugin == null)
                    continue;
                _mqttServicePluginInfo = mqttServicePlugin.GetPluginInfo();
                _pluginIdentifier = _mqttServicePluginInfo.PluginIdentifier;
                _pluginType = PluginType.MqttServicePlugin;
                break;
            }
            if (_pluginType != null)
                goto typeJudgeDone;
            foreach (var type in types)
            {
                if (type.GetInterface("ICommandInputPlugin") == null)
                    continue;
                if (type.FullName == null)
                    continue;
                ICommandInputPlugin? commandInputPlugin = (ICommandInputPlugin?)
                    assembly.CreateInstance(type.FullName);
                if (commandInputPlugin == null)
                    continue;
                _commandInputPluginInfo = commandInputPlugin.GetPluginInfo();
                _pluginIdentifier = _commandInputPluginInfo.PluginIdentifier;
                _pluginType = PluginType.CommandInputPlugin;
                break;
            }
            if (_pluginType != null)
                goto typeJudgeDone;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
        }

    typeJudgeDone:
        loadContext.Unload();
        if (_pluginType == null)
        {
            MessageBox.Show(
                "Not a valid plugin file.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
            NavigationService.GoBack();
            return;
        }

        if (_pluginType == PluginType.MqttServicePlugin)
        {
            MqttPluginSummary.Visibility = Visibility.Visible;
            CommandInputPluginSummary.Visibility = Visibility.Collapsed;
            PluginNameTextBlock.Text = _mqttServicePluginInfo.PluginName;
            PluginVersionTextBlock.Text = _mqttServicePluginInfo.PluginVersion;
            PluginAuthorTextBlock.Text = _mqttServicePluginInfo.PluginAuthor;
            PluginTypeTextBlock.Text = "OpenFlier.Plugin.IMqttServicePlugin";
            PluginDescriptionTextBlock.Text = _mqttServicePluginInfo.PluginDescription;
            PluginIdentifierTextBlock.Text = _mqttServicePluginInfo.PluginIdentifier;
            PluginNeedConfigEntryTextBlock.Text =
                _mqttServicePluginInfo.PluginNeedsConfigEntry.ToString();
            MqttMessageTypeTextBlock.Text = _mqttServicePluginInfo.MqttMessageType.ToString();
            return;
        }
        if (_pluginType == PluginType.CommandInputPlugin)
        {
            CommandInputPluginSummary.Visibility = Visibility.Visible;
            MqttPluginSummary.Visibility = Visibility.Collapsed;
            PluginNameTextBlock.Text = _commandInputPluginInfo.PluginName;
            PluginVersionTextBlock.Text = _commandInputPluginInfo.PluginVersion;
            PluginAuthorTextBlock.Text = _commandInputPluginInfo.PluginAuthor;
            PluginTypeTextBlock.Text = "OpenFlier.Plugin.ICommandInputPlugin";
            PluginDescriptionTextBlock.Text = _commandInputPluginInfo.PluginDescription;
            PluginIdentifierTextBlock.Text = _commandInputPluginInfo.PluginIdentifier;
            PluginNeedConfigEntryTextBlock.Text =
                _commandInputPluginInfo.PluginNeedsConfigEntry.ToString();
            CommanInputCallerNamesTextBlock.Text = string.Join(
                ", ",
                _commandInputPluginInfo.InvokeCommands
            );
            return;
        }
    }

    private void Next_Click(object sender, RoutedEventArgs e)
    {
        SinglePluginPackage singlePluginPackage = new SinglePluginPackage();
        var entry = _pluginEntry.FullName.Replace(
            _zipExtractedPath,
            string.Empty,
            StringComparison.OrdinalIgnoreCase
        );
        if (entry.StartsWith("\\"))
            entry = entry[1..];
        switch (_pluginType)
        {
            case PluginType.MqttServicePlugin:
                singlePluginPackage.PluginName = _mqttServicePluginInfo.PluginName;
                singlePluginPackage.PluginVersion = _mqttServicePluginInfo.PluginVersion;
                singlePluginPackage.PluginAuthor = _mqttServicePluginInfo.PluginAuthor;
                singlePluginPackage.PluginType = Plugin.PluginType.MqttServicePlugin;
                singlePluginPackage.PluginDescription = _mqttServicePluginInfo.PluginDescription;
                singlePluginPackage.PluginIdentifier = _mqttServicePluginInfo.PluginIdentifier;
                singlePluginPackage.PluginNeedsConfigEntry =
                    _mqttServicePluginInfo.PluginNeedsConfigEntry;
                singlePluginPackage.MqttMessageType = (int)_mqttServicePluginInfo.MqttMessageType;
                singlePluginPackage.PluginEntry = entry;
                singlePluginPackage.ZipArchive = ByteString.CopyFrom(File.ReadAllBytes(_zipPath));
                break;
            case PluginType.CommandInputPlugin:
                singlePluginPackage.PluginName = _commandInputPluginInfo.PluginName;
                singlePluginPackage.PluginVersion = _commandInputPluginInfo.PluginVersion;
                singlePluginPackage.PluginAuthor = _commandInputPluginInfo.PluginAuthor;
                singlePluginPackage.PluginType = Plugin.PluginType.CommandInputPlugin;
                singlePluginPackage.PluginDescription = _commandInputPluginInfo.PluginDescription;
                singlePluginPackage.PluginIdentifier = _commandInputPluginInfo.PluginIdentifier;
                singlePluginPackage.PluginNeedsConfigEntry =
                    _commandInputPluginInfo.PluginNeedsConfigEntry;
                singlePluginPackage.InvokeCommands.Add(_commandInputPluginInfo.InvokeCommands);
                singlePluginPackage.PluginEntry = entry;
                singlePluginPackage.ZipArchive = ByteString.CopyFrom(File.ReadAllBytes(_zipPath));
                break;
            default:
                return;
        }
        Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
        dialog.Filter = "OpenFlier Single Plugin Package (.ofspp)|*.ofspp";
        dialog.FileName = $"{_pluginIdentifier}.ofspp";
        dialog.AddExtension = true;
        if (dialog.ShowDialog() == true)
        {
            using (FileStream s = new FileStream(dialog.FileName, FileMode.Create))
            {
                singlePluginPackage.WriteTo(s);
            }
        }
    }

    private void Previous_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.GoBack();
    }
}
