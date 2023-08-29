using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace OpenFlier.Core.Services
{
    public class UdpService
    {
        private bool flag = true;
        public void Initialize()
        {
            UpdateConnectCode();
            StartUdpBroadcast();
        }
        public void StartUdpBroadcast()
        {
            flag = true;
            Thread udpBroadcastThread = new Thread(UdpBroadcast);
            udpBroadcastThread.IsBackground = true;
            udpBroadcastThread.Start();
        }
        public void StopUdpBroadcast()
        {
            flag = false;
            Thread.Sleep(800);
        }
        public void UdpBroadcast()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Broadcast, CoreStorage.CoreConfig.UDPBroadcastPort ?? 33338);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            string udpContent = CoreStorage.IPAddress + "::" + CoreStorage.ConnectCode;
            byte[] udpContentBytes = Encoding.UTF8.GetBytes(udpContent);
            ILog logger = LogManager.GetLogger(typeof(UdpService));
            logger.Info($"Begin UDP broadcast with content {udpContent}");
            while (flag)
            {
                socket.SendTo(udpContentBytes, remoteEP);
                Thread.Sleep(800);
            }
            socket.Close();
            logger.Info("UDP broadcast ended.");
        }
        public void UpdateConnectCode()
        {
            Regex regex = new Regex("^\\d{4}$");
            string specifiedConnectCode = CoreStorage.CoreConfig.SpecifiedConnectCode ?? "";
            if (regex.IsMatch(specifiedConnectCode))
            {
                CoreStorage.ConnectCode = specifiedConnectCode;
                return;
            }
            string connectCode = Random.Shared.Next(0, 10000).ToString("D4");
            CoreStorage.ConnectCode = connectCode;
        }
    }
}
