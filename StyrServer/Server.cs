using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace StyrServer
{
	public class Server
	{
		enum PacketType : byte {
			Discovery,
			Offer,
			Connection
		};

		private UdpClient udpClient;
		private IPEndPoint groupEP;

		public bool Running { get; set; }

		public Server (int port)
		{
			udpClient = new UdpClient (port);
			groupEP = new IPEndPoint (IPAddress.Any, port);

			Running = true;

			Thread mainThread = new Thread (ServerLoop);
			mainThread.Start ();
			Console.WriteLine ("Server running on port {0}", port);
		}

		private void ServerLoop()
		{
			byte[] receivedPacket;
			while (Running) {
				if (udpClient.Available > 0) {
					receivedPacket = udpClient.Receive (ref groupEP);
					if (receivedPacket.Length > 0) {
						switch (receivedPacket [0]) {
						case (byte)PacketType.Discovery:
							Console.WriteLine ("Discovery received from {0}:{1}", groupEP.Address, groupEP.Port);
							byte[] offer = { (byte)PacketType.Offer };
							udpClient.Send (offer, offer.Length, groupEP);
							break;
						case (byte)PacketType.Connection:
							Console.WriteLine ("Connection received");
							break;
						default:
							Console.WriteLine ("Unknown packet received");
							break;
						}
					}
				}
			}
			udpClient.Close ();
		}
	}
}


