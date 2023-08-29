using log4net;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace OpenFlier.Core.Services
{
    public class VerificationService
    {
        public ILog VerificationServiceLogger = LogManager.GetLogger(typeof(VerificationService));
        public void Initialize()
        {
            try
            {
                string specifiedMI = CoreStorage.CoreConfig.SpecifiedMachineIdentifier ?? "";
                string localMIFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "29fe3a4f-c75a-560f-5459-2ad69c9187d0");
                if (specifiedMI.Length == 32)
                {
                    CoreStorage.MachineIdentifier = specifiedMI;
                    return;
                }
                if (File.Exists(localMIFilePath))
                {
                    byte[] localBIBytes = File.ReadAllBytes(localMIFilePath);
                    if (localBIBytes.Length == 16)
                    {
                        CoreStorage.MachineIdentifier = string.Concat(localBIBytes.Select(x => x.ToString("X2")));
                        return;
                    }
                    File.Delete(localMIFilePath);
                }
                CoreStorage.MachineIdentifier = CreateMachineIdentifier(localMIFilePath);
                VerificationServiceLogger.Info($"Successfully initialized verification service with machine identifier {CoreStorage.MachineIdentifier}");
            }
            catch (Exception e)
            {
                VerificationServiceLogger.Error("Failed to initialize machine identifier.", e);
            }
            Version? SpecifiedVersion;
            bool success=Version.TryParse(CoreStorage.CoreConfig.SpecifiedEmulatedVersion, out SpecifiedVersion);
            if (success&&SpecifiedVersion?.CompareTo(Version.Parse("2.0.9")) > 0)
                CoreStorage.Version = CoreStorage.CoreConfig.SpecifiedEmulatedVersion;
            VerificationServiceLogger.Info($"Emulating version {CoreStorage.Version}");
        }
        public string CreateMachineIdentifier(string localMIPath)
        {
            string sourceGuid = Guid.NewGuid().ToString() + ";7kM`=&Cuce:&E";
            byte[] MI = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(sourceGuid));
            File.WriteAllBytes(localMIPath, MI);
            return string.Concat(MI.Select(x => x.ToString("X2")));
        }
    }
}