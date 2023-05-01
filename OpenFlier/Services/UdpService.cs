using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace OpenFlier.Services
{
    public static class UdpService
    {
        private static bool flag = true;
        public static void InitializeUdpService()
        {
            UpdateConnectCode();
            StartUdpBroadcast();
        }
        public static void StartUdpBroadcast()
        {
            flag = true;
            Thread udpBroadcastThread = new Thread(UdpBroadcast);
            udpBroadcastThread.IsBackground = true;
            udpBroadcastThread.Start();
        }
        public static void StopUdpBroadcast()
        {
            flag = false;
            Thread.Sleep(800);
        }
        public static void UdpBroadcast()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Broadcast, LocalStorage.Config.General.UDPBroadcastPort ?? 33338);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            string udpContent = LocalStorage.IPAddress + "::" + LocalStorage.ConnectCode;
            byte[] udpContentBytes = Encoding.UTF8.GetBytes(udpContent);
            while (flag)
            {
                socket.SendTo(udpContentBytes, remoteEP);
                Thread.Sleep(800);
            }
            socket.Close();
        }
        public static void UpdateConnectCode()
        {
            Regex regex = new Regex("^\\d{4}$");
            string specifiedConnectCode = LocalStorage.Config.General.SpecifiedConnectCode ?? "";
            if (regex.IsMatch(specifiedConnectCode))
            {
                LocalStorage.ConnectCode = specifiedConnectCode;
                return;
            }
            string connectCode = Random.Shared.Next(0, 10000).ToString("D4");
            LocalStorage.ConnectCode = connectCode;
        }
    }
}
