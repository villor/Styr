﻿using System;
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

			if (PlatformDetector.CurrentPlatform == PlatformID.Win32NT) {
				mouse = new MouseDriverWindows();
			}
			else {
				mouse = new MouseDriverOSX();
			}

			Thread mainThread = new Thread (ServerLoop);
			mainThread.Start ();
			Console.WriteLine ("Server running on port {0}", port);
		}

		private void sendAck(ushort id) {
			byte[] ackPack = new byte[3];
			ackPack[0] = (byte)PacketType.Ack;
			PacketConverter.PutUShort (id, ackPack, 1);

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
								ushort id = PacketConverter.GetUShort(receivedPacket, 1);

								Debug.WriteLine ("Connection received from {0}:{1}", groupEP.Address, groupEP.Port);
								if (!ackedPackets.Exists (p => p.ID == id)) {
									connectedClients.Add (new Client (groupEP));
								}
								sendAck (id);
								Debug.WriteLine ("Ack sent to {0}", groupEP.Address);
							}
							break;

						case (byte)PacketType.Ping:
							if (connectedClients.Exists (p => p.EndPoint.Equals (groupEP)) && receivedPacket.Length == 3) {
								Debug.WriteLine ("Ping!");
								ushort id = PacketConverter.GetUShort(receivedPacket, 1);
								connectedClients.Find (p => p.EndPoint.Equals (groupEP)).TimeSinceLastPing = TimeSpan.Zero;
								sendAck (id);
							}
							break;

						case (byte)PacketType.MouseMovement:
							if (connectedClients.Exists (p => p.EndPoint.Equals(groupEP)) && receivedPacket.Length == 9) {
								mouse.RelativeMove (PacketConverter.GetFloat(receivedPacket, 1), PacketConverter.GetFloat(receivedPacket, 5));
							}
							break;

						case (byte)PacketType.MouseLeftClick:
							if (connectedClients.Exists (p => p.EndPoint.Equals (groupEP)) && receivedPacket.Length == 3) {
								ushort id = PacketConverter.GetUShort(receivedPacket, 1);
								if (!ackedPackets.Exists (p => p.ID == id)) {
									Debug.WriteLine ("LeftClick!");
									mouse.LeftButtonClick ();
								}
								sendAck (id);
							}
							break;

						case (byte)PacketType.MouseLeftDown:
							if (connectedClients.Exists (p => p.EndPoint.Equals (groupEP)) && receivedPacket.Length == 3) {
								ushort id = PacketConverter.GetUShort (receivedPacket, 1);
								if (!ackedPackets.Exists (p => p.ID == id)) {
									Debug.WriteLine ("LeftDown!");
									mouse.LeftButtonDown ();
								}
								sendAck (id);
							}
							break;

						case (byte)PacketType.MouseLeftUp:
							if (connectedClients.Exists (p => p.EndPoint.Equals (groupEP)) && receivedPacket.Length == 3) {
								ushort id = PacketConverter.GetUShort (receivedPacket, 1);
								if (!ackedPackets.Exists (p => p.ID == id)) {
									Debug.WriteLine ("LeftUp!");
									mouse.LeftButtonUp ();
								}
								sendAck (id);
							}
							break;

						case (byte)PacketType.MouseRightClick:
							if (connectedClients.Exists (p => p.EndPoint.Equals (groupEP)) && receivedPacket.Length == 3) {
								ushort id = PacketConverter.GetUShort(receivedPacket, 1);
								if (!ackedPackets.Exists (p => p.ID == id)) {
									Debug.WriteLine ("RightClick!");
									mouse.RightButtonClick ();
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
				for (int i = connectedClients.Count - 1; i >= 0; i--) {
					connectedClients[i].TimeSinceLastPing += sw.Elapsed;
					if (connectedClients[i].TimeSinceLastPing.TotalMilliseconds > 5000) {
						Debug.WriteLine ("Client timed out and was disconnected (" + connectedClients [i].IP.ToString () + ")");
						connectedClients.RemoveAt (i);
					}
				}
				sw.Restart ();

				Thread.Sleep (1);
			}
			udpClient.Close ();
		}
	}
}


