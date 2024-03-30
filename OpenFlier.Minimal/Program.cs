using OpenFlier.Core;
using OpenFlier.Core.Services;
namespace OpenFlier.Minimal
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var serviceManager = new ServiceManager(new CoreConfig());
            serviceManager.OnLoadCompleted += ServiceManager_OnLoadCompleted;
            serviceManager.BeginLoad();
            while (true)
            { }
        }

        private static void ServiceManager_OnLoadCompleted(bool isReloaded)
        {
            Console.WriteLine($"IP Address:   {CoreStorage.IPAddresses.Select(x => x.ToString())}");
            Console.WriteLine($"Connect code: {CoreStorage.ConnectCode}");
            Console.WriteLine($"Machine Id.:  {CoreStorage.MachineIdentifier}");
        }
    }
}
