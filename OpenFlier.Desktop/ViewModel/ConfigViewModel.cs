using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using OpenFlier.Core.Services;
using OpenFlier.Desktop.Localization;
using OpenFlier.Plugin;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace OpenFlier.Desktop.ViewModel
{
    public partial class ConfigViewModel : ObservableObject
    {
        private readonly ServiceManager? serviceManager;
        private readonly Action<Config>? preReloadAction;

        public ConfigViewModel()
        {
            this._currentConfig = new Config();
            ModifiedConfig = _currentConfig;
        }

        public ConfigViewModel(
            Config currentConfig,
            ServiceManager serviceManager,
            Action<Config> preReloadAction
        )
        {
            this._currentConfig = currentConfig;
            this.serviceManager = serviceManager;
            this.preReloadAction = preReloadAction;
            ModifiedConfig = currentConfig;
        }

        private Config _currentConfig;

        [ObservableProperty]
        private Config _modifiedConfig;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ApplyCommand))]
        private bool _confirmApplyPopupOpened = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveCommandInputUserCommand))]
        private CommandInputUser? _selectedCommandInputUser;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RestoreDefaultConfigCommand))]
        private bool _confirmRestorePopupOpened = false;

        [RelayCommand(CanExecute = nameof(CanExecuteApply))]
        private void Apply()
        {
            Regex regex = new("^\\d{4}$");
            if (
                !string.IsNullOrEmpty(ModifiedConfig.SpecifiedConnectCode?.Trim())
                && !regex.IsMatch(ModifiedConfig.SpecifiedConnectCode.Trim())
            )
            {
                MessageBox.Show(
                    Backend.ConnectCodeFormatError,
                    Backend.Error,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }
            ConfirmApplyPopupOpened = true;
        }

        private bool CanExecuteApply() => !ConfirmApplyPopupOpened;

        [RelayCommand]
        private void ApplyCancel()
        {
            ConfirmApplyPopupOpened = false;
        }

        [RelayCommand]
        private void ApplyConfirm()
        {
            ConfirmApplyPopupOpened = false;
            File.WriteAllText(
                "config.json",
                JsonSerializer.Serialize(
                    ModifiedConfig,
                    new JsonSerializerOptions { WriteIndented = true, }
                )
            );

            preReloadAction?.Invoke(ModifiedConfig);
            LocalStorage.Config = ModifiedConfig;
            if (ModifiedConfig.Appearances.RevertTextColor)
                Application.Current.Resources["TextColorOnBase"] = new SolidColorBrush(
                    Colors.White
                );
            else
                Application.Current.Resources["TextColorOnBase"] = new SolidColorBrush(
                    Colors.Black
                );
            serviceManager?.RestartAllServices(ModifiedConfig);
            LocalStorage.DesktopMqttService?.RefreshCommandInputUserStatus();
            _currentConfig = ModifiedConfig;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteRemoveCommandInputUser))]
        private void RemoveCommandInputUser()
        {
            if (SelectedCommandInputUser is null)
                return;
            ModifiedConfig.CommandInputUsers.Remove(SelectedCommandInputUser);
        }

        private bool CanExecuteRemoveCommandInputUser() => SelectedCommandInputUser != null;

        [RelayCommand]
        private void AddCommandInputUser()
        {
            ModifiedConfig.CommandInputUsers.Add(new CommandInputUser());
        }

        [RelayCommand(CanExecute = nameof(CanExecuteRestoreDefaultConfig))]
        private void RestoreDefaultConfig()
        {
            ConfirmRestorePopupOpened = true;
        }

        private bool CanExecuteRestoreDefaultConfig() => !ConfirmRestorePopupOpened;

        [RelayCommand]
        private void ConfirmRestoreConfig()
        {
            if (File.Exists("config.json"))
                File.Delete("config.json");
            ConfirmRestorePopupOpened = false;
            var newConfig = new Config();
            preReloadAction?.Invoke(newConfig);
            LocalStorage.Config = newConfig;
            Application.Current.Resources["TextColorOnBase"] = new SolidColorBrush(Colors.Black);
            serviceManager?.RestartAllServices(newConfig);
            _currentConfig = new Config();
            ModifiedConfig = _currentConfig;
        }

        [RelayCommand]
        private void CancelRestoreConfig()
        {
            ConfirmRestorePopupOpened = false;
        }

        [RelayCommand]
        private void ExportDefaultConfig()
        {
            var dialog = new SaveFileDialog()
            {
                Filter = Backend.ConfigFileFilter,
                DefaultExt = ".json",
            };
            var result = dialog.ShowDialog();
            if (result == true)
                ConfigService.OutputDefaultConfig(dialog.FileName);
        }

        [RelayCommand]
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

        [RelayCommand]
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

        [RelayCommand]
        private void DiscardChanges()
        {
            ModifiedConfig = _currentConfig;
        }
    }
}
