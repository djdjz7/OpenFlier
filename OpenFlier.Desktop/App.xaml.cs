using Hardcodet.Wpf.TaskbarNotification;
using log4net.Config;
using System;
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
        var taskbarIcon = (TaskbarIcon)FindResource("TrayMenu");
    }
}
