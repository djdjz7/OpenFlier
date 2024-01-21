using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows;
using System.Security.AccessControl;
using Microsoft.Win32;

namespace OpenFlier.Utils;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var args = e.Args;
        if (args.Length == 0)
        {
            Application.Current.Shutdown();
            return;
        }
        switch (args[0].ToLower())
        {
            case ".ofspp":
                if (args.Length == 1)
                {
                    MessageBox.Show("No .ofspp file specified.");
                    Application.Current.Shutdown();
                    break;
                }
                InstallPlugin installPlugin = new InstallPlugin(args[1]);
                installPlugin.Show();
                break;
            case "register":
                EnsureAdminPrivilege();
                LinkFileExtensions();
                Application.Current.Shutdown();
                break;
            case "post-install":
                EnsureAdminPrivilege();
                LinkFileExtensions();
                SetWritePermission();
                Application.Current.Shutdown();
                break;
            default:
                Application.Current.Shutdown();
                break;
        }
    }

    public static bool IsAdmin()
    {
        WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
        WindowsPrincipal windowsPrincipal = new WindowsPrincipal(windowsIdentity);
        return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    [DllImport("shell32.dll")]
    public static extern void SHChangeNotify(
        uint wEventId,
        uint uFlags,
        IntPtr dwItem1,
        IntPtr dwItem2
    );

    public void EnsureAdminPrivilege()
    {
        if (!IsAdmin())
        {
            ProcessStartInfo startInfo = new("OpenFlier.Utils.exe", "register");
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.Verb = "runas";
            Process.Start(startInfo);
            Application.Current.Shutdown();
            return;
        }
    }

    public void LinkFileExtensions()
    {
        try
        {
            RegistryKey root = Registry.ClassesRoot;
            using var progidSub = root.CreateSubKey("OpenFlier.Utils.ofspp", true);
            using var extSub = root.CreateSubKey(".ofspp", true);
            progidSub.SetValue("", Utils.Resources.OfsppDescription);

            using var iconSub = progidSub.CreateSubKey("DefaultIcon", true);
            iconSub.SetValue("", Path.Combine(Environment.CurrentDirectory, "Assets\\ofspp.ico"));

            using var commandSub = progidSub.CreateSubKey("shell\\open\\command", true);
            commandSub.SetValue(
                "",
                Path.Combine(Environment.CurrentDirectory, "OpenFlier.Utils.exe") + " .ofspp \"%1\""
            );

            using var openWithSub = extSub.CreateSubKey("OpenWithProgids", true);
            openWithSub.SetValue("OpenFlier.Utils.ofspp", "");
            SHChangeNotify(0x8000000, 0, IntPtr.Zero, IntPtr.Zero);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    public void SetWritePermission()
    {
        var rootDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        var accessControl = rootDirectoryInfo.GetAccessControl();
        var everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
        accessControl.AddAccessRule(
            new FileSystemAccessRule(
                everyone,
                FileSystemRights.FullControl,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.None,
                AccessControlType.Allow
            )
        );
        rootDirectoryInfo.SetAccessControl(accessControl);
    }

}
