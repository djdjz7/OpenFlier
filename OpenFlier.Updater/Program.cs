using System.Diagnostics;
using System.IO.Compression;
using System.Security.AccessControl;
using System.Security.Principal;

namespace OpenFlier.Updater
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            // Application.Run(new Form1());
            if (args.Length == 1)
            {
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var appName = Process.GetCurrentProcess().ProcessName;
                var exePath = baseDirectory + appName + ".exe";
                var appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "OpenFlier"
                );
                if (!Directory.Exists(appDataPath))
                    Directory.CreateDirectory(appDataPath);
                var destPath = Path.Combine(appDataPath, "OpenFlier.Updater.exe");
                var destPackPath = Path.Combine(appDataPath, "full-package");
                File.Copy(exePath, destPath, true);
                File.Copy(args[0], destPackPath, true);
                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = destPath,
                        UseShellExecute = true,
                        Verb = "RunAs",
                    }
                );
                Application.Exit();
                return;
            }
            try
            {
                foreach(var i in Process.GetProcessesByName("OpenFlier.Desktop"))
                    i.Kill();
                foreach (var i in Process.GetProcessesByName("OpenFlier.Utils"))
                    i.Kill();
                Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (!File.Exists("full-package"))
                    return;
                if (Directory.Exists("full-package-extracted"))
                    Directory.Delete("full-package-extracted", true);
                using (var fileStream = File.OpenRead("full-package"))
                {
                    using var zipArchive = new ZipArchive(fileStream);
                    zipArchive.ExtractToDirectory("full-package-extracted", true);
                }
                var targetDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    "OpenFlier"
                );
                RecursiveCopy("full-package-extracted", targetDir);
                Directory.Delete("full-package-extracted", true);
                File.Delete("full-package");
                SetWritePermission(targetDir);
                Process.Start("explorer", Path.Combine(targetDir, "OpenFlier.Desktop.exe"));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        static void RecursiveCopy(string source, string target)
        {
            Directory.CreateDirectory(target);
            var directoryInfo = new DirectoryInfo(source);
            foreach (var subdirectory in directoryInfo.GetDirectories())
            {
                RecursiveCopy(subdirectory.FullName, Path.Combine(target, subdirectory.Name));
            }
            foreach (var file in directoryInfo.GetFiles())
            {
                File.Copy(file.FullName, Path.Combine(target, file.Name), true);
            }
        }
        static void SetWritePermission(string path)
        {
            var rootDirectoryInfo = new DirectoryInfo(path);
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
}
