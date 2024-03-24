using Hardcodet.Wpf.TaskbarNotification;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace OpenFlier.Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        XmlConfigurator.Configure();
        Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var processes = Process.GetProcesses();
        var currentId = Process.GetCurrentProcess().Id;
        var conflicts = new List<Process>();
        foreach (var process in processes)
        {
            if (
                process.ProcessName == "CloudRoom.Main"
                || (process.ProcessName == "OpenFlier.Desktop" && process.Id != currentId)
            )
                conflicts.Add(process);
        }
        if (conflicts.Count >= 1)
        {
            var result = await new ProcessConflictWindow().ShowDialog();
            switch (result)
            {
                case ProcessConflictWindow.ProcessConflictResolution.KillAndContinue:
                    foreach (var process in conflicts)
                    {
                        process.Kill();
                    }
                    break;
                case ProcessConflictWindow.ProcessConflictResolution.QuitCurrent:
                default:
                    Application.Current.Shutdown();
                    return;
            }
        }
        var taskbarIcon = (TaskbarIcon)FindResource("TrayMenu");
        var config = ConfigService.ReadConfig();
        if (!string.IsNullOrEmpty(config.General.Locale))
        {
            try
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(
                    config.General.Locale
                );
            }
            catch (Exception) { }
        }
        if(config.Appearances.RevertTextColor)
            Resources["TextColorOnBase"] = new SolidColorBrush(Colors.White);
        new MainWindow(config).Show();
    }
}
