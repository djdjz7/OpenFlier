using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Google.Protobuf;
using OpenFlier.Plugin;

namespace OpenFlier.DevUtils;
/// <summary>
/// PackSave.xaml 的交互逻辑
/// </summary>
public partial class PackSummary : Page
{
    ObservableCollection<PluginFileModel> PluginFiles;
    PluginType? pluginType;
    MqttServicePluginInfo? mqttServicePluginInfo;
    CommandInputPluginInfo? commandInputPluginInfo;
    public enum PluginType
    {
        MqttServicePlugin = 0,
        CommandInputPlugin = 1,
    }
    public PackSummary(ObservableCollection<PluginFileModel> pluginFiles)
    {
        InitializeComponent();
        PluginFiles = pluginFiles;
        PluginFileModel mainFile = new PluginFileModel();
        foreach (var i in PluginFiles)
        {
            if (i.IsPluginMain)
            {
                mainFile = i;
                try
                {
                    FileInfo assemblyFileInfo = new FileInfo(i.FilePath);
                    var assembly = Assembly.LoadFrom(assemblyFileInfo.FullName);
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (type.GetInterface("IMqttServicePlugin") == null)
                            continue;
                        if (type.FullName == null)
                            continue;
                        IMqttServicePlugin? mqttServicePlugin = (IMqttServicePlugin?)assembly.CreateInstance(type.FullName);
                        if (mqttServicePlugin == null)
                            continue;
                        mqttServicePluginInfo = mqttServicePlugin.GetPluginInfo();
                        pluginType = PluginType.MqttServicePlugin;
                        break;
                    }
                    if (pluginType != null)
                        break;
                    foreach (var type in types)
                    {
                        if (type.GetInterface("ICommandInputPlugin") == null)
                            continue;
                        if (type.FullName == null)
                            continue;
                        ICommandInputPlugin? commandInputPlugin = (ICommandInputPlugin?)assembly.CreateInstance(type.FullName);
                        if (commandInputPlugin == null)
                            continue;
                        commandInputPluginInfo = commandInputPlugin.GetPluginInfo();
                        pluginType = PluginType.CommandInputPlugin;
                        break;
                    }
                    if (pluginType != null)
                        break;
                    i.IsPluginMain = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                }
            }
        }
        if (pluginType == null)
        {
            MessageBox.Show("Not a valid plugin file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            var packSelectMain = new PackSelectMain(PluginFiles);
            NavigationService.Navigate(packSelectMain);
            return;
        }

        PluginFilesList.ItemsSource = PluginFiles;
        PluginFilesList.SelectedItem = mainFile;

        if (pluginType == PluginType.MqttServicePlugin)
        {
            MqttPluginSummary.Visibility = Visibility.Visible;
            CommandInputPluginSummary.Visibility = Visibility.Collapsed;
            PluginNameTextBlock.Text = mqttServicePluginInfo.PluginName;
            PluginVersionTextBlock.Text = mqttServicePluginInfo.PluginVersion;
            PluginAuthorTextBlock.Text = mqttServicePluginInfo.PluginAuthor;
            PluginTypeTextBlock.Text = "OpenFlier.Plugin.IMqttServicePlugin";
            PluginDescriptionTextBlock.Text = mqttServicePluginInfo.PluginDescription;
            PluginIdentifierTextBlock.Text = mqttServicePluginInfo.PluginIdentifier;
            PluginNeedConfigEntryTextBlock.Text = mqttServicePluginInfo.PluginNeedsConfigEntry.ToString();
            MqttMessageTypeTextBlock.Text = mqttServicePluginInfo.MqttMessageType.ToString();
            return;
        }
        if (pluginType == PluginType.CommandInputPlugin)
        {
            CommandInputPluginSummary.Visibility = Visibility.Visible;
            MqttPluginSummary.Visibility = Visibility.Collapsed;
            PluginNameTextBlock.Text = commandInputPluginInfo.PluginName;
            PluginVersionTextBlock.Text = commandInputPluginInfo.PluginVersion;
            PluginAuthorTextBlock.Text = commandInputPluginInfo.PluginAuthor;
            PluginTypeTextBlock.Text = "OpenFlier.Plugin.ICommandInputPlugin";
            PluginDescriptionTextBlock.Text = commandInputPluginInfo.PluginDescription;
            PluginIdentifierTextBlock.Text = commandInputPluginInfo.PluginIdentifier;
            PluginNeedConfigEntryTextBlock.Text = commandInputPluginInfo.PluginNeedsConfigEntry.ToString();
            CommanInputCallerNamesTextBlock.Text = string.Join(", ", commandInputPluginInfo.InvokeCommands);
            return;
        }
    }

    private void Next_Click(object sender, RoutedEventArgs e)
    {
        SinglePluginPackage singlePluginPackage = new SinglePluginPackage();
        switch (pluginType)
        {
            case PluginType.MqttServicePlugin:
                singlePluginPackage.PluginName = mqttServicePluginInfo.PluginName;
                singlePluginPackage.PluginVersion = mqttServicePluginInfo.PluginVersion;
                singlePluginPackage.PluginAuthor = mqttServicePluginInfo.PluginAuthor;
                singlePluginPackage.PluginType = Plugin.PluginType.MqttServicePlugin;
                singlePluginPackage.PluginDescription = mqttServicePluginInfo.PluginDescription;
                singlePluginPackage.PluginIdentifier = mqttServicePluginInfo.PluginIdentifier;
                singlePluginPackage.PluginNeedConfigEntry = mqttServicePluginInfo.PluginNeedsConfigEntry;
                singlePluginPackage.MqttMessageType = (int)mqttServicePluginInfo.MqttMessageType;
                foreach (var i in PluginFiles)
                {
                    singlePluginPackage.Files.Add(new Plugin.PluginFile()
                    {
                        IsPluginMain = i.IsPluginMain,
                        Filename = i.FileName,
                        FileData = ByteString.CopyFrom(System.IO.File.ReadAllBytes(i.FilePath))
                    });
                }
                break;
            case PluginType.CommandInputPlugin:
                singlePluginPackage.PluginName = commandInputPluginInfo.PluginName;
                singlePluginPackage.PluginVersion = commandInputPluginInfo.PluginVersion;
                singlePluginPackage.PluginAuthor = commandInputPluginInfo.PluginAuthor;
                singlePluginPackage.PluginType = Plugin.PluginType.CommandInputPlugin;
                singlePluginPackage.PluginDescription = commandInputPluginInfo.PluginDescription;
                singlePluginPackage.PluginIdentifier = commandInputPluginInfo.PluginIdentifier;
                singlePluginPackage.PluginNeedConfigEntry = commandInputPluginInfo.PluginNeedsConfigEntry;
                singlePluginPackage.InvokeCommands.Add(commandInputPluginInfo.InvokeCommands);
                foreach (var i in PluginFiles)
                {
                    singlePluginPackage.Files.Add(new Plugin.PluginFile()
                    {
                        IsPluginMain = i.IsPluginMain,
                        Filename = i.FileName,
                        FileData = ByteString.CopyFrom(System.IO.File.ReadAllBytes(i.FilePath))
                    });
                }
                break;
            default:
                return;
        }
        Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
        dialog.Filter = "OpenFlier Single Plugin Package (.ofspp)|*.ofspp";
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
        foreach (var i in PluginFiles)
        {
            i.IsPluginMain = false;
        }
        NavigationService.Navigate(new PackSelectMain(PluginFiles));
        return;
    }
}