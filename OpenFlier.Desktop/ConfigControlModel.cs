using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using OpenFlier.Core;
using OpenFlier.Core.Services;
using OpenFlier.Desktop.Localization;
using OpenFlier.Plugin;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenFlier.Desktop
{
    public partial class ConfigControlModel : ObservableObject
    {
        private ServiceManager serviceManager;
        private Action<Config> preReloadAction;
        public ConfigControlModel()
        {
            this.currentConfig = new Config();
            CommandInputPlugins = new ObservableCollection<LocalPluginInfo<CommandInputPluginInfo>>(
                currentConfig.CommandInputPlugins
            );
            CommandInputUsers = new ObservableCollection<CommandInputUser>(
                currentConfig.CommandInputUsers
            );
            WindowTitle = currentConfig.Appearances.WindowTitle ?? "OpenFlier";
            PrimaryColor = currentConfig.Appearances.PrimaryColor ?? "";
            SecondaryColor = currentConfig.Appearances.SecondaryColor ?? "";
            BackgroundImage = currentConfig.Appearances.BackgroundImage ?? "";
            EnableWindowEffects = currentConfig.Appearances.EnableWindowEffects ?? true;
            SyncColorWithSystem = currentConfig.Appearances.SyncColorWithSystem ?? false;
            SpecifiedConnectCode = currentConfig.SpecifiedConnectCode ?? "";
            SpecifiedEmulatedVersion = currentConfig.SpecifiedEmulatedVersion ?? "";

            DefaultUpdateCheckURL = currentConfig.General.DefaultUpdateCheckURL;
            UsePng = currentConfig.General.UsePng;

            UdpBroadcastPort = currentConfig.UDPBroadcastPort ?? 33338;
            MqttServerPort = currentConfig.MqttServerPort ?? 61136;
            SpecifiedMachineIdentifier = currentConfig.SpecifiedMachineIdentifier ?? "";
            SpecifiedConnectCode = currentConfig.SpecifiedConnectCode ?? "";
            SpecifiedEmulatedVersion = currentConfig.SpecifiedEmulatedVersion ?? "2.1.1";
            MqttServicePlugins = new ObservableCollection<LocalPluginInfo<MqttServicePluginInfo>>(
                currentConfig.MqttServicePlugins
            );
            VerificationContent = currentConfig.VerificationContent;
            FtpDirectory = currentConfig.FtpDirectory ?? "Screenshots";
            ApplyCommand = new RelayCommand(Apply);
        }

        public ConfigControlModel(Config currentConfig, ServiceManager serviceManager, Action<Config> preReloadAction)
        {
            this.currentConfig = currentConfig;
            this.serviceManager = serviceManager;
            this.preReloadAction = preReloadAction;
            CommandInputPlugins = new ObservableCollection<LocalPluginInfo<CommandInputPluginInfo>>(
                currentConfig.CommandInputPlugins
            );
            CommandInputUsers = new ObservableCollection<CommandInputUser>(
                currentConfig.CommandInputUsers
            );
            WindowTitle = currentConfig.Appearances.WindowTitle ?? "OpenFlier";
            PrimaryColor = currentConfig.Appearances.PrimaryColor ?? "";
            SecondaryColor = currentConfig.Appearances.SecondaryColor ?? "";
            BackgroundImage = currentConfig.Appearances.BackgroundImage ?? "";
            EnableWindowEffects = currentConfig.Appearances.EnableWindowEffects ?? true;
            SyncColorWithSystem = currentConfig.Appearances.SyncColorWithSystem ?? false;
            SpecifiedConnectCode = currentConfig.SpecifiedConnectCode ?? "";
            SpecifiedEmulatedVersion = currentConfig.SpecifiedEmulatedVersion ?? "";

            DefaultUpdateCheckURL = currentConfig.General.DefaultUpdateCheckURL;
            UsePng = currentConfig.General.UsePng;

            UdpBroadcastPort = currentConfig.UDPBroadcastPort ?? 33338;
            MqttServerPort = currentConfig.MqttServerPort ?? 61136;
            SpecifiedMachineIdentifier = currentConfig.SpecifiedMachineIdentifier ?? "";
            SpecifiedConnectCode = currentConfig.SpecifiedConnectCode ?? "";
            SpecifiedEmulatedVersion = currentConfig.SpecifiedEmulatedVersion ?? "2.1.1";
            MqttServicePlugins = new ObservableCollection<LocalPluginInfo<MqttServicePluginInfo>>(
                currentConfig.MqttServicePlugins
            );
            VerificationContent = string.IsNullOrEmpty(currentConfig.VerificationContent) ? "{\"type\":20007,\"data\":{\"topic\":\"Ec1xkK+uFtV/QO/8rduJ2A==\"}}" : currentConfig.VerificationContent;
            FtpDirectory = currentConfig.FtpDirectory ?? "Screenshots";
            ApplyCommand = new RelayCommand(Apply, () => !ConfirmPopupOpened);
            ApplyCancelCommand = new RelayCommand(ApplyCancel);
            ApplyConfirmCommand = new RelayCommand(ApplyConfirm);
            RemoveCommandInputUserCommand = new RelayCommand(RemoveCommandInputUser, () => SelectedCommandInputUser != null);
            AddCommandInputUserCommand = new RelayCommand(AddCommandInputUser);
            RestoreDefaultConfigCommand = new RelayCommand(RestoreDefaultConfig);
            ExportDefaultConfigCommand = new RelayCommand(ExportDefaultConfig);
            ConfigMqttServicePluginCommand = new RelayCommand<string?>(ConfigMqttServicePlugin);
            ConfigCommandInputPluginCommand = new RelayCommand<string?>(ConfigCommandInputPlugin);
        }

        private Config currentConfig;

        #region CommandInput
        [ObservableProperty]
        private ObservableCollection<LocalPluginInfo<CommandInputPluginInfo>> commandInputPlugins;

        [ObservableProperty]
        private ObservableCollection<CommandInputUser> commandInputUsers;

        #endregion
        #region Appereances
        [ObservableProperty]
        private string windowTitle;

        [ObservableProperty]
        private string primaryColor;

        [ObservableProperty]
        private string secondaryColor;

        [ObservableProperty]
        private string backgroundImage;

        [ObservableProperty]
        private bool enableWindowEffects;

        [ObservableProperty]
        private bool syncColorWithSystem;

        #endregion
        #region General
        [ObservableProperty]
        private string? defaultUpdateCheckURL;

        [ObservableProperty]
        private bool usePng;
        #endregion
        #region CoreConfig
        [ObservableProperty]
        private int udpBroadcastPort;

        [ObservableProperty]
        private int mqttServerPort;

        [ObservableProperty]
        private string specifiedMachineIdentifier;

        [ObservableProperty]
        private string specifiedConnectCode;

        [ObservableProperty]
        private string specifiedEmulatedVersion;

        [ObservableProperty]
        private ObservableCollection<LocalPluginInfo<MqttServicePluginInfo>> mqttServicePlugins;

        [ObservableProperty]
        private string verificationContent;

        [ObservableProperty]
        private string ftpDirectory;
        #endregion
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ApplyCommand))]
        private bool confirmPopupOpened = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveCommandInputUserCommand))]
        private CommandInputUser? selectedCommandInputUser;

        public IRelayCommand ApplyCommand { get; }
        public ICommand ApplyConfirmCommand { get; }
        public ICommand ApplyCancelCommand { get; }
        public IRelayCommand AddCommandInputUserCommand { get; }
        public IRelayCommand RemoveCommandInputUserCommand { get; }
        public ICommand RestoreDefaultConfigCommand { get; }
        public ICommand ExportDefaultConfigCommand { get; }
        public ICommand ConfigMqttServicePluginCommand { get; }
        public ICommand ConfigCommandInputPluginCommand { get; }

        private void Apply()
        {
            Regex regex = new("^\\d{4}$");
            if (!regex.IsMatch(SpecifiedConnectCode.Trim()) && !string.IsNullOrEmpty(SpecifiedConnectCode.Trim()))
            {
                MessageBox.Show(Backend.ConnectCodeFormatError, Backend.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ConfirmPopupOpened = true;
        }
        private void ApplyCancel()
        {
            ConfirmPopupOpened = false;
        }
        private void ApplyConfirm()
        {
            ConfirmPopupOpened = false;
            var newConfig = new Config
            {
                Appearances = new Appearances
                {
                    BackgroundImage = BackgroundImage,
                    EnableWindowEffects = EnableWindowEffects,
                    PrimaryColor = PrimaryColor,
                    SecondaryColor = SecondaryColor,
                    SyncColorWithSystem = SyncColorWithSystem,
                    WindowTitle = WindowTitle,
                },
                CommandInputPlugins = CommandInputPlugins.ToList(),
                CommandInputUsers = CommandInputUsers.ToList(),
                FtpDirectory = FtpDirectory,
                General = new General
                {
                    DefaultUpdateCheckURL = DefaultUpdateCheckURL,
                    UsePng = UsePng,
                },
                MqttServerPort = MqttServerPort,
                MqttServicePlugins = MqttServicePlugins.ToList(),
                SpecifiedConnectCode = SpecifiedConnectCode.Trim(),
                SpecifiedEmulatedVersion = SpecifiedEmulatedVersion,
                SpecifiedMachineIdentifier = SpecifiedMachineIdentifier,
                UDPBroadcastPort = UdpBroadcastPort,
                VerificationContent = VerificationContent,
            };
            File.WriteAllText("config.json", JsonSerializer.Serialize(newConfig, new JsonSerializerOptions
            {
                WriteIndented = true,
            }));

            preReloadAction.Invoke(newConfig);
            LocalStorage.Config = newConfig;
            serviceManager.RestartAllServices(newConfig);
        }

        private void RemoveCommandInputUser()
        {
            if (SelectedCommandInputUser is null)
                return;
            CommandInputUsers.Remove(SelectedCommandInputUser);
        }
        private void AddCommandInputUser()
        {
            CommandInputUsers.Add(new CommandInputUser());
        }

        private void RestoreDefaultConfig()
        {

        }

        private void ExportDefaultConfig()
        {
            var dialog = new SaveFileDialog()
            {
                Filter = Backend.ConfigFileFilter,
                DefaultExt = ".json",
            };
            var result = dialog.ShowDialog();
            if(result == true)
                ConfigService.OutputDefaultConfig(dialog.FileName);
        }

        private void ConfigMqttServicePlugin(string? localFilePath)
        {
            if (localFilePath == null)
                return;
            try
            {
                FileInfo assemblyFileInfo = new FileInfo(localFilePath);
                var assembly = Assembly.LoadFrom(assemblyFileInfo.FullName);

                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if (type.GetInterface("IMqttServicePlugin") == null)
                        continue;
                    if (type.FullName == null)
                        continue;
                    IMqttServicePlugin? mqttServicePlugin = (IMqttServicePlugin?)
                        assembly.CreateInstance(type.FullName);
                    if (mqttServicePlugin == null)
                        continue;
                    mqttServicePlugin.PluginOpenConfig();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void ConfigCommandInputPlugin(string? localFilePath)
        {
            if (localFilePath == null)
                return;
            try
            {
                FileInfo assemblyFileInfo = new FileInfo(localFilePath);
                var assembly = Assembly.LoadFrom(assemblyFileInfo.FullName);

                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if (type.GetInterface("ICommandInputPlugin") == null)
                        continue;
                    if (type.FullName == null)
                        continue;
                    ICommandInputPlugin? commandInputPlugin = (ICommandInputPlugin?)
                        assembly.CreateInstance(type.FullName);
                    if (commandInputPlugin == null)
                        continue;
                    commandInputPlugin.PluginOpenConfig();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}
