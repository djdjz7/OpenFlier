using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenFlier.Core;
using OpenFlier.Plugin;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Input;
using OpenFlier.Desktop.Localization;
using System.Text.RegularExpressions;

namespace OpenFlier.Desktop
{
    public partial class ConfigControlModel : ObservableObject
    {
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
        }

        public ConfigControlModel(Config currentConfig)
        {
            this.currentConfig = currentConfig;
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
            ApplyCommand = new RelayCommand(Apply);
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

        public ICommand ApplyCommand { get; }
        private void Apply()
        {
            Regex regex = new("^\\d{4}$");
            if (!regex.IsMatch(SpecifiedConnectCode.Trim()) && !string.IsNullOrEmpty(SpecifiedConnectCode.Trim()))
            {
                MessageBox.Show(Backend.ConnectCodeFormatError, Backend.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (MessageBox.Show(Backend.OverwriteConfigWarning, Backend.Warning, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

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
        }
    }
}
