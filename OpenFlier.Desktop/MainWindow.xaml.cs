using CommunityToolkit.Mvvm.Messaging;
using log4net;
using OpenFlier.Controls;
using OpenFlier.Core;
using OpenFlier.Core.Services;
using OpenFlier.Desktop.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenFlier.Desktop;

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow : Window
{
    private ServiceManager serviceManager = LocalStorage.ServiceManager;
    private ILog _logger = LogManager.GetLogger(nameof(MainWindow));

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var config = ConfigService.ReadConfig();
        if (config.Appearances.EnableWindowEffects ?? true)
        {
            WindowEffects.EnableWindowEffects(this);
        }

        await ValidatePluginPrivilege(config);

        List<IPAddress> ipAddresses = Dns.GetHostEntry(Dns.GetHostName())
            .AddressList.Where(
                x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
            )
            .ToList();
        if (ipAddresses.Count == 0)
        {
            MessageBox.Show("No available IP address.");
            Application.Current.Shutdown();
            return;
        }

        serviceManager = new ServiceManager(config);
        serviceManager.OnLoadCompleted += ServiceManager_LoadCompleted;
        serviceManager.BeginLoad();
        LoadingScreen.Visibility = Visibility.Visible;
        MainScreen.Visibility = Visibility.Hidden;

        ConfigTab.Content = new ConfigControl(
            config,
            serviceManager,
            async (newConfig) =>
            {
                await ValidatePluginPrivilege(newConfig);
                this.LoadingTextBlock.Text = Localization.UI.ReloadingText;
                this.LoadingScreen.Visibility = Visibility.Visible;
                this.MainScreen.Visibility = Visibility.Hidden;
            }
        );
    }

    private void ServiceManager_LoadCompleted(bool isReloaded)
    {
        Application.Current.Dispatcher.Invoke(async () =>
        {
            if (isReloaded)
            {
                ConfigTab.Content = new ConfigControl(
                    LocalStorage.Config,
                    serviceManager,
                    async (newConfig) =>
                    {
                        await ValidatePluginPrivilege(newConfig);
                        this.LoadingTextBlock.Text = Localization.UI.ReloadingText;
                        this.LoadingScreen.Visibility = Visibility.Visible;
                        this.MainScreen.Visibility = Visibility.Hidden;
                    }
                );
            }
            if (!isReloaded)
            {
                LocalStorage.DesktopMqttService = new(serviceManager.MqttService.MqttServer);

                serviceManager.MqttService.OnClientConnected += LocalStorage
                    .DesktopMqttService
                    .OnClientConnectedAsync;
                serviceManager.MqttService.OnClientDisconnected += LocalStorage
                    .DesktopMqttService
                    .OnClientDisConnectedAsync;
                serviceManager.MqttService.OnScreenshotRequestReceived += LocalStorage
                    .DesktopMqttService
                    .OnScreenshotRequestReceivedAsync;
                serviceManager.MqttService.OnTestRequestReceived += LocalStorage
                    .DesktopMqttService
                    .OnTestMessageReceivedAsync;

                if (CoreStorage.IPAddresses?.Count > 1)
                {
                    IPAddress.Text = Backend.MultipleAddresses;
                    IPAddress.ToolTip = string.Join('\n', CoreStorage.IPAddresses);
                }
            }
            ConnectCode.Text = CoreStorage.ConnectCode;
            WeakReferenceMessenger.Default.Send<ConnectionCodeUpdatedMessage>(new(CoreStorage.ConnectCode));
            MachineIdentifier.Text = CoreStorage.MachineIdentifier;
            LoadingScreen.Visibility = Visibility.Hidden;
            MainScreen.Visibility = Visibility.Visible;
            var presentationSource = PresentationSource.FromVisual(this);
            var scale = 1.0;
            if (presentationSource != null)
                scale = presentationSource.CompositionTarget.TransformToDevice.M11;
            LocalStorage.ScreenSize.Width = (int)(SystemParameters.PrimaryScreenWidth * scale);
            LocalStorage.ScreenSize.Height = (int)(SystemParameters.PrimaryScreenHeight * scale);
            CheckForUpdates();
        });
    }

    private void Window_MouseLeftButtonDown(
        object sender,
        System.Windows.Input.MouseButtonEventArgs e
    )
    {
        this.DragMove();
    }

    private void TestButton_Click(object sender, RoutedEventArgs e)
    {
        (new Video()).ShowDialog();
    }

    private static bool IsApplicationRunningAsAdmin()
    {
        WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
        WindowsPrincipal windowsPrincipal = new WindowsPrincipal(windowsIdentity);
        return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    private async Task ValidatePluginPrivilege(Config config)
    {
        bool isAdmin = IsApplicationRunningAsAdmin();
        foreach (var plugin in config.MqttServicePlugins)
        {
            plugin.TempDisabled = false;
        }
        foreach (var plugin in config.CommandInputPlugins)
        {
            plugin.TempDisabled = false;
        }
        if (isAdmin)
            AdminText.Text = Backend.WithAdminPrivilege;
        if (!isAdmin)
        {
            bool requireAdmin =
                config.MqttServicePlugins
                    .Where(x => x.PluginInfo.PluginNeedsAdminPrivilege == true && x.Enabled == true)
                    .Count()
                    + config.CommandInputPlugins
                        .Where(
                            x => x.PluginInfo.PluginNeedsAdminPrivilege == true && x.Enabled == true
                        )
                        .Count()
                > 0;
            if (requireAdmin)
            {
                var result = await new PrivilegeErrorWindow().ShowDialog();
                switch (result)
                {
                    case PrivilegeErrorWindow.PrivilegeResult.DisableAndContinue:
                        foreach (var plugin in config.MqttServicePlugins)
                        {
                            plugin.TempDisabled = false;
                            if (plugin.PluginInfo.PluginNeedsAdminPrivilege && plugin.Enabled)
                                plugin.TempDisabled = true;
                        }
                        foreach (var plugin in config.CommandInputPlugins)
                        {
                            plugin.TempDisabled = false;
                            if (plugin.PluginInfo.PluginNeedsAdminPrivilege && plugin.Enabled)
                                plugin.TempDisabled = true;
                        }
                        break;
                    case PrivilegeErrorWindow.PrivilegeResult.RestartAsAdmin:

                        var path = Process.GetCurrentProcess().MainModule?.FileName;
                        var directory = Path.GetDirectoryName(path);
                        ProcessStartInfo startInfo =
                            new()
                            {
                                UseShellExecute = true,
                                WorkingDirectory = directory,
                                FileName = path,
                                Verb = "runas"
                            };
                        Process.Start(startInfo);
                        Application.Current.Shutdown();
                        return;
                }
            }
        }
    }

    private void RepositoryHyperLink_Click(object sender, RoutedEventArgs e)
    {
        Process.Start("explorer.exe", "https://github.com/djdjz7/OpenFlier");
    }

    private async void CheckForUpdates()
    {
        UpdateProgressBar.Foreground = FindResource("PrimaryBrush") as SolidColorBrush;
        UpdateProgressBar.IsIndeterminate = true;
        UpdateProgressBar.Value = 0;
        RetryUpdateButton.Visibility = Visibility.Collapsed;
        UpdateStatus.Text = Backend.CheckingForUpdates;
        var needsUpdate = false;
        var newVersionString = "";
        var newMD5 = "";
        var httpClient = new HttpClient();
        try
        {
            newVersionString = await httpClient.GetStringAsync(
                "https://openflier.top/update/latest-version"
            );
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            VersionText.Text = currentVersion?.ToString(3) ?? "Unknown Version";
            var newVersion = Version.Parse(newVersionString);
            var compareResult = currentVersion?.CompareTo(newVersion);
            if (compareResult < 0)
                needsUpdate = true;
            else
            {
                UpdateStatus.Text = Backend.UpToDate;
                UpdateProgressBar.Visibility = Visibility.Collapsed;
            }
        }
        catch (Exception e)
        {
            UpdateProgressBar.Value = 100;
            UpdateProgressBar.IsIndeterminate = false;
            UpdateProgressBar.Foreground = FindResource("SystemCriticalBrush") as SolidColorBrush;
            UpdateStatus.Text = Backend.ErrorCheckingForUpdates;
            _logger.Error("Error checking for update: ", e);
            RetryUpdateButton.Visibility = Visibility.Visible;
        }
        if (!needsUpdate)
            return;
        try
        {
            newMD5 = await httpClient.GetStringAsync("https://openflier.top/update/latest-md5");
            CancelUpdateButton.Visibility = Visibility.Visible;
            UpdateStatus.Text = string.Format(Backend.DownloadingUpdate, newVersionString);
            var needsDownload = true;
            byte[] data;
            if (File.Exists("latest-package"))
            {
                data = File.ReadAllBytes("latest-package");
                var localMD5 = ComputeMD5(data);
                if (string.Compare(localMD5, newMD5, true) == 0)
                    needsDownload = false;
            }
            if (needsDownload)
            {
                data = await DownloadWithProgress(
                    "https://openflier.top/update/latest-package",
                    UpdateProgressBar
                );
                var localMD5 = ComputeMD5(data);
                if (string.Compare(localMD5, newMD5, true) == 0)
                    throw new Exception("MD5 mismatch.");
                File.WriteAllBytes("latest-package", data);
            }
            else
            {
                UpdateProgressBar.IsIndeterminate = false;
                UpdateProgressBar.Value = 100;
            }
            UpdateStatus.Text = string.Format(Backend.ReadyToRestart, newVersionString);
            CancelUpdateButton.Visibility = Visibility.Collapsed;
            RestartForUpdateButton.Visibility = Visibility.Visible;
        }
        catch (Exception e)
        {
            CancelUpdateButton.Visibility = Visibility.Collapsed;
            UpdateProgressBar.Value = 100;
            UpdateProgressBar.IsIndeterminate = false;
            UpdateProgressBar.Foreground = FindResource("SystemCriticalBrush") as SolidColorBrush;
            UpdateStatus.Text = string.Format(Backend.ErrorDownloadingUpdate, newVersionString);
            _logger.Error("Error downloading update: ", e);
            RetryUpdateButton.Visibility = Visibility.Visible;
        }
        finally
        {
            httpClient.Dispose();
        }
    }

    private bool _canceled = false;

    private async Task<byte[]> DownloadWithProgress(string url, ProgressBar progressBar)
    {
        byte[] finalResult;
        var client = new HttpClient();
        using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
        {
            var contentLenght = response.Content.Headers.ContentLength;
            var bufferSize = 8192;
            if (contentLenght is not null)
            {
                progressBar.IsIndeterminate = false;
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var buffer = new byte[bufferSize];
                    finalResult = new byte[(int)contentLenght];
                    var pointer = 0;
                    var reachedEnd = false;
                    do
                    {
                        var i = await stream.ReadAsync(buffer.AsMemory(0, bufferSize));
                        Array.Copy(buffer, 0, finalResult, pointer, i);
                        pointer += i;
                        if (i == 0)
                            reachedEnd = true;
                        if (contentLenght is not null)
                            progressBar.Value = (double)(pointer * 1.0 / contentLenght * 100);
                    } while (!reachedEnd && !_canceled);
                }
            }
            else
            {
                progressBar.IsIndeterminate = true;
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var buffer = new byte[bufferSize];
                    byte[] result = Array.Empty<byte>();
                    var pointer = 0;
                    var reachedEnd = false;
                    do
                    {
                        var i = await stream.ReadAsync(buffer.AsMemory(0, bufferSize));
                        pointer += i;
                        var resultBuffer = (byte[])result.Clone();
                        result = new byte[pointer];
                        resultBuffer.CopyTo(result, 0);
                        Array.Copy(buffer, 0, result, resultBuffer.Length, i);
                        if (i == 0)
                            reachedEnd = true;
                    } while (!reachedEnd && !_canceled);
                    finalResult = result;
                    progressBar.Value = 100;
                    progressBar.IsIndeterminate = false;
                }
            }
        }
        return finalResult;
    }

    private string ComputeMD5(byte[] data)
    {
        MD5 md5 = MD5.Create();
        var md5Data = md5.ComputeHash(data);
        StringBuilder sb = new StringBuilder();
        foreach (byte b in md5Data)
        {
            sb.Append(b.ToString("X2"));
        }
        return sb.ToString();
    }

    private void RestartForUpdateButton_Click(object sender, RoutedEventArgs e)
    {
        Process.Start("OpenFlier.Updater.exe", "latest-package");
        Application.Current.Shutdown();
    }

    private void CancelUpdateButton_Click(object sender, RoutedEventArgs e)
    {
        _canceled = true;
        UpdateStatus.Text = Backend.UpdateCancelled;
        UpdateProgressBar.Foreground = FindResource("SystemCautionBrush") as SolidColorBrush;
    }

    private void RetryUpdateButton_Click(object sender, RoutedEventArgs e)
    {
        CheckForUpdates();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        this.Hide();
        e.Cancel = true;
    }
}
