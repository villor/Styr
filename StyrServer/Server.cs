using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace StyrServer
{
	public class Server
	{
		enum PacketType : byte {
			Discovery,
			Offer,
			ConnectionReq,
			ConnectionAck,
			AccessDenied,
			MouseMovement
		};

		private UdpClient udpClient;
		private IPEndPoint groupEP;

		public bool Running { get; set; }

		private List<Client> connectedClients;

		public Server (int port)
		{
			udpClient = new UdpClient (port);
			groupEP = new IPEndPoint (IPAddress.Any, port);

			connectedClients = new List<Client> ();

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
							Debug.WriteLine ("Discovery received from {0}:{1}", groupEP.Address, groupEP.Port);
							byte[] offer = { (byte)PacketType.Offer };
							udpClient.Send (offer, offer.Length, groupEP);
							break;

						case (byte)PacketType.ConnectionReq:
							Debug.WriteLine ("Connection received from {0}:{1}", groupEP.Address, groupEP.Port);
							connectedClients.Add(new Client (groupEP));
							byte[] ack = { (byte)PacketType.ConnectionAck };
							udpClient.Send (ack, ack.Length, groupEP);
							Debug.WriteLine ("Ack sent to {0}", groupEP.Address);
							break;

						case (byte)PacketType.MouseMovement:
							if (connectedClients.Exists (p => p.EndPoint == groupEP)) {
								Debug.WriteLine ("Mouse moved!");
							}
							break;

						default:
							Debug.WriteLine ("Unknown packet received");
							break;
						}
					}
				}
				Thread.Sleep (1);
			}
			udpClient.Close ();
		}
	}
}


