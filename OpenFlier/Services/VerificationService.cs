using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace OpenFlier.Services
{
    public static class VerificationService
    {
        public static void InitializeVerificationService()
        {
            string specifiedMI = LocalStorage.Config.General.SpecifiedMachineIdentifier ?? "";
            string localMIFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "29fe3a4f-c75a-560f-5459-2ad69c9187d0");
            if (specifiedMI.Length == 32)
            {
                LocalStorage.MachineIdentifier = specifiedMI;
                return;
            }
            if (File.Exists(localMIFilePath))
            {
                byte[] localBIBytes = File.ReadAllBytes(localMIFilePath);
                if (localBIBytes.Length == 16)
                {
                    LocalStorage.MachineIdentifier = string.Concat(localBIBytes.Select(x => x.ToString("X2")));
                    return;
                }
                File.Delete(localMIFilePath);
            }
            LocalStorage.MachineIdentifier = CreateMachineIdentifier(localMIFilePath);
        }
        public static string CreateMachineIdentifier(string localMIPath)
        {
            string sourceGuid = Guid.NewGuid().ToString() + ";7kM`=&Cuce:&E";
            byte[] MI = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(sourceGuid));
            File.WriteAllBytes(localMIPath, MI);
            return string.Concat(MI.Select(x => x.ToString("X2")));
        }
    }
}