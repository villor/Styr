using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using StyrServer.Input;

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

		private IMouseDriver mouse;

		public bool Running { get; set; }

		private List<Client> connectedClients;

		public Server (int port)
		{
			udpClient = new UdpClient (port);
			groupEP = new IPEndPoint (IPAddress.Any, port);

			connectedClients = new List<Client> ();

			Running = true;

			mouse = new MouseDriverOSX ();

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
							if (connectedClients.Exists (p => p.EndPoint.Equals(groupEP)) && receivedPacket.Length == 9) {
								byte[] xArr = new byte[4];
								byte[] yArr = new byte[4];

								Array.Copy (receivedPacket, 1, xArr, 0, 4);
								Array.Copy (receivedPacket, 5, yArr, 0, 4);

								if (BitConverter.IsLittleEndian) {
									Array.Reverse (xArr);
									Array.Reverse (yArr);
								}

								Debug.WriteLine ("X: {0}, Y: {1}", BitConverter.ToSingle (xArr, 0), BitConverter.ToSingle (yArr, 0));
								mouse.RelativeMove (BitConverter.ToSingle (xArr, 0), BitConverter.ToSingle (yArr, 0));
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


