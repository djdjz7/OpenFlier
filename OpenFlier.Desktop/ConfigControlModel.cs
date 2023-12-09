using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenFlier.Core;
using OpenFlier.Core.Services;
using OpenFlier.Desktop.Localization;
using OpenFlier.Plugin;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
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
            OutputDefaultConfigCommand = new RelayCommand(OutputDefaultConfig);
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
        public ICommand OutputDefaultConfigCommand { get; }

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

        private void OutputDefaultConfig()
        {
            ConfigService.OutputDefaultConfig();
        }
    }
}
