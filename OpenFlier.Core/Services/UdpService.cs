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
using System.Diagnostics;

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
            ILog logger = LogManager.GetLogger(typeof(UdpService));
            logger.Info($"Begin UDP broadcast with following IPs:\n {string.Join('\n', CoreStorage.IPAddresses)}");
            while (flag)
            {
                foreach (var ipAddress in CoreStorage.IPAddresses)
                {
                    string udpContent = ipAddress + "::" + CoreStorage.ConnectCode;
                    byte[] udpContentBytes = Encoding.UTF8.GetBytes(udpContent);
                    socket.SendTo(udpContentBytes, remoteEP);
                }
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
