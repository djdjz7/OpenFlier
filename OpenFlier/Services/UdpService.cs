using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenFlier.Services
{
    public static class UdpService
    {
        private static bool flag = true;
        public static void StartUdpBroadcast()
        {
            flag = true;
            Thread udpBroadcastThread = new Thread(UdpBroadcast);
            udpBroadcastThread.IsBackground = true;
        }
        public static void StopUdpBroadcast()
        {
            flag = false;
        }
        public static void UdpBroadcast()
        {

        }
    }
}
