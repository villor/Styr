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
	class AckedPacket {
		public ushort ID;
		public double ElapsedTime;
	}

	public class Server
	{
		enum PacketType : byte {
			Discovery,
			Offer,
			Connection,
			Ack,
			AccessDenied,
			MouseMovement,
			MouseLeftClick
		};

		private UdpClient udpClient;
		private IPEndPoint groupEP;

		private IMouseDriver mouse;

		private List<Client> connectedClients;
		private List<AckedPacket> ackedPackets;

		public bool Running { get; set; }

		public Server (int port)
		{
			udpClient = new UdpClient (port);
			groupEP = new IPEndPoint (IPAddress.Any, port);

			connectedClients = new List<Client> ();
			ackedPackets = new List<AckedPacket> ();

			Running = true;

			mouse = new MouseDriverOSX ();

			Thread mainThread = new Thread (ServerLoop);
			mainThread.Start ();
			Console.WriteLine ("Server running on port {0}", port);
		}

		private void sendAck(ushort id) {
			byte[] ackPack = new byte[3];
			ackPack[0] = (byte)PacketType.Ack;
			byte[] idArr = BitConverter.GetBytes(id);

			if (BitConverter.IsLittleEndian) {
				Array.Reverse(idArr);
			}

			Array.Copy (idArr, 0, ackPack, 1, idArr.Length);
			udpClient.Send (ackPack, ackPack.Length, groupEP);
			ackedPackets.Add(new AckedPacket { ID = id, ElapsedTime = 0 });
		}

		private void ServerLoop()
		{
			byte[] receivedPacket;
			Stopwatch sw = new Stopwatch ();
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

						case (byte)PacketType.Connection:
							if (receivedPacket.Length == 3) {
								byte[] idArr = new byte[2];
								idArr [0] = receivedPacket [1];
								idArr [1] = receivedPacket [2];

								if (BitConverter.IsLittleEndian) {
									Array.Reverse (idArr);
								}

								ushort id = BitConverter.ToUInt16 (idArr, 0);

								Debug.WriteLine ("Connection received from {0}:{1}", groupEP.Address, groupEP.Port);
								if (!ackedPackets.Exists (p => p.ID == id)) {
									connectedClients.Add (new Client (groupEP));
								}
								sendAck (id);
								Debug.WriteLine ("Ack sent to {0}", groupEP.Address);
							}
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

						case (byte)PacketType.MouseLeftClick:
							if (connectedClients.Exists (p => p.EndPoint.Equals (groupEP)) && receivedPacket.Length == 3) {

								byte[] idArr = new byte[2];
								idArr [0] = receivedPacket [1];
								idArr [1] = receivedPacket [2];

								if (BitConverter.IsLittleEndian) {
									Array.Reverse (idArr);
								}

								ushort id = BitConverter.ToUInt16 (idArr, 0);
								if (!ackedPackets.Exists (p => p.ID == id)) {
									Debug.WriteLine ("Click!");
									mouse.LeftButtonClick ();
								}
								sendAck (id);
							}
							break;

						default:
							Debug.WriteLine ("Unknown packet received from {0}:\n0x{1}", groupEP.Address, BitConverter.ToString (receivedPacket).Replace ("-", string.Empty));
							break;
						}
					}
				}

				sw.Stop ();
				for (int i = ackedPackets.Count - 1; i >= 0; i--) {
					ackedPackets[i].ElapsedTime += sw.Elapsed.TotalMilliseconds;
					if (ackedPackets[i].ElapsedTime > 1000) {
						ackedPackets.RemoveAt (i);
					}
				}
				sw.Restart ();

				Thread.Sleep (1);
			}
			udpClient.Close ();
		}
	}
}


