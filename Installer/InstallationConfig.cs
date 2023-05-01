using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installer
{
    public static class InstallationConfig
    {
        public static InstallerMode InstallerMode = InstallerMode.OnlineInstall;
        public static string DownloadSource="";
        public static bool CreateDesktopShortcut = true;
        public static bool KeepPreviousConfig = true;
        public static bool AddToStartup = false;
        public static bool DownloadCefSharpPack = false;
    }
    public enum InstallerMode
    {
        OnlineInstall=0,
        OfflineInstall=1,
        Update=2,
        Uninstall=3,
    }
}
