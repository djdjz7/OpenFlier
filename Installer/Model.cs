namespace Installer;
public class InstallerConfig
{
    public InstallerMode InstallerMode { get; set; }
    public DownloadSource? DownloadSource { get; set; }
    public bool KeepPreviousConfig { get; set; }
    public bool AddToStartUp { get; set; }
    public bool CreateDesktopShortcut { get; set; }
}

public class DownloadSource
{
    required public PackageSource MainPackSource { get; set; }
    required public PackageSource FullPackSource { get; set; }
}
public class PackageSource
{
    required public string VersionUrl { get; set; }
    required public string Url { get; set; }
    required public string ChecksumUrl { get; set; }
}
public enum InstallerMode
{
    OnlineInstall = 0,
    OfflineInstall = 1,
    Update = 2,
    Uninstall = 3,
}