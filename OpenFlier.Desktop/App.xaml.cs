using Hardcodet.Wpf.TaskbarNotification;
using log4net.Config;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;

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

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var processes = Process.GetProcesses();
        var count = 0;
        foreach (var process in processes)
        {
            if (
                process.ProcessName == "OpenFlier.Desktop"
                || process.ProcessName == "CloudRoom.Main"
            )
                count++;
        }
        if (count > 1)
        {
            new ProcessConflictWindow().ShowDialog();
            Application.Current.Shutdown();
            return;
        }
        var taskbarIcon = (TaskbarIcon)FindResource("TrayMenu");
        var config = ConfigService.ReadConfig();
        if (!string.IsNullOrEmpty(config.General.Locale))
        {
            try
            {

                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(config.General.Locale);
            }
            catch (Exception)
            {
            }
        }
        new MainWindow(config).Show();
    }
}
