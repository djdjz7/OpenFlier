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
    public PackageSource MainPackSource { get; set; }
    public PackageSource FullPackSource { get; set; }
}
public class PackageSource
{
    public string VersionUrl { get; set; }
    public string Url { get; set; }
    public string ChecksumUrl { get; set; }
}
public enum InstallerMode
{
    OnlineInstall = 0,
    OfflineInstall = 1,
    Update = 2,
    Uninstall = 3,
}