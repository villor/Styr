﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Threading;

namespace StyrClient.Network
{
	public delegate void onSessionTimeoutEventHandler ();

	public class RemoteSession
	{
		public event onSessionTimeoutEventHandler Timeout;

		private UdpClient udpClient;
		private IPEndPoint remoteEndPoint;
		private List<ReliablePacket> sentPackList;
		private ushort reliablePacketID;

		public RemoteSession (IPEndPoint endPoint)
		{
			remoteEndPoint = endPoint;
			udpClient = new UdpClient ();
			sentPackList = new List<ReliablePacket> ();
			reliablePacketID = 0;

			Reconnect (); // <----- Temporary, need try or some shit

			Thread backgroundLoop = new Thread (() => {
				Debug.WriteLine("receiverLoop is live!");
				receiveResendRemove ();
			});
			backgroundLoop.Start ();
		}

		public RemoteSession (IPAddress ipAddress) : this (new IPEndPoint (ipAddress, 1337))
		{
		}

		public bool Reconnect ()
		{
			// sending connection request
			byte[] packet = { (byte)PacketType.Connection };
			sendReliablePacket (packet);

			// receiving connection reply
			byte[] receivedPacket = udpClient.Receive (ref remoteEndPoint);
			if (receivedPacket.Length != 0) {
				if (receivedPacket [0] == (byte)PacketType.Ack) {
					return true;
				} else if (receivedPacket [0] == (byte)PacketType.AccessDenied) {
					throw new UnauthorizedAccessException ("Unauthorized access");
				} else {
					throw new Exception ("Unknown error, connection not established");
				}
			} else {
				return false;
			}
		}

		public void SendMouseMovement (float x, float y)
		{
			byte[] packet = new byte[9];
			byte[] xArray = BitConverter.GetBytes (x);
			byte[] yArray = BitConverter.GetBytes (y);
			packet [0] = (byte)PacketType.MouseMovement;

			if (BitConverter.IsLittleEndian) {
				Array.Reverse (xArray);
				Array.Reverse (yArray);
			}

			Array.Copy (xArray, 0, packet, 1, xArray.Length);
			Array.Copy (yArray, 0, packet, 5, yArray.Length);
			sendUnreliablePacket (packet);
		}

		public void SendLeftClick ()
		{
			byte[] packet = { (byte)PacketType.MouseLeftClick };
			sendReliablePacket (packet);
		}

		public void SendLeftUp ()
		{
			byte[] packet = { (byte)PacketType.MouseLeftUp };
			sendReliablePacket (packet);
		}

		public void SendLeftDown ()
		{
			byte[] packet = { (byte)PacketType.MouseLeftDown };
			sendReliablePacket (packet);
		}

		public void SendRightClick ()
		{
			byte[] packet = { (byte)PacketType.MouseRightClick };
			sendReliablePacket (packet);
		}

		public void ping ()
		{
			byte [] packet = {(byte)PacketType.Ping };
			sendReliablePacket (packet);
		}

		private void sendReliablePacket (byte[] packet, bool isNewPacket = true, ushort id = 0)
		{
			if (isNewPacket) {
				id = reliablePacketID++;

				sentPackList.Add (new ReliablePacket {
					Packet = packet,
					ID = id,
					ElapsedTime = 0,
					SendTimer = 0
				});
			}

			byte[] idArray = BitConverter.GetBytes (id);

			if (BitConverter.IsLittleEndian) {
				Array.Reverse (idArray);
			}

			byte[] newPacket = new byte[packet.Length + idArray.Length];
			Array.Copy (packet, 0, newPacket, 0, packet.Length);
			Array.Copy (idArray, 0, newPacket, packet.Length, idArray.Length);

			udpClient.Send (newPacket, newPacket.Length, remoteEndPoint);
		}

		private void sendUnreliablePacket (byte[] packet)
		{
			udpClient.Send (packet, packet.Length, remoteEndPoint);
		}



		private void receiveResendRemove ()
		{
			Stopwatch loopTime = new Stopwatch ();
			Stopwatch pingTimer = new Stopwatch ();
			Stopwatch timeoutTimer = new Stopwatch ();
			pingTimer.Start ();
			timeoutTimer.Start ();
		
			while (true) {

				if (udpClient.Available > 0) {
					byte[] receivedPacket = udpClient.Receive (ref remoteEndPoint);

					if (receivedPacket.Length != 0) {
						if (receivedPacket [0] == (byte)PacketType.Ack) {
							//Debug.WriteLine ("Ack received");
							timeoutTimer.Restart ();

							byte[] idArr = new byte[2];
							idArr [0] = receivedPacket [1];
							idArr [1] = receivedPacket [2];

							if (BitConverter.IsLittleEndian) {
								Array.Reverse (idArr);
							}
							ushort idCtrl = BitConverter.ToUInt16 (idArr, 0);

							for (int i = sentPackList.Count - 1; i >= 0; i--) {
								if (sentPackList[i].ID == idCtrl) {
									sentPackList.RemoveAt(i);
									//Debug.WriteLine ("Tar bort ett jävla packe nu då");
								}
							}
						} else if (receivedPacket [0] == (byte)PacketType.AccessDenied) {
							throw new UnauthorizedAccessException ("Unauthorized access");
						} else {
							throw new Exception ("Unknown error, connection not established");
						}
					}
				}

				loopTime.Stop ();
				//Debug.WriteLine (loopTime.Elapsed.TotalMilliseconds);
				for (int i = sentPackList.Count - 1; i >= 0; i--){
					sentPackList[i].ElapsedTime += loopTime.Elapsed.TotalMilliseconds;
					//Debug.WriteLine (sentPackList[i].ElapsedTime);
					sentPackList[i].SendTimer += loopTime.Elapsed.TotalMilliseconds;
					if (sentPackList[i].ElapsedTime > 1000) {
						sentPackList.Remove (sentPackList[i]);	// <------------------ Sessionen ska avbrytas här?
					} else if (sentPackList[i].SendTimer > 75) {
						Debug.WriteLine ("Resending packet with elapsed time: {0}", sentPackList[i].ElapsedTime);
						sendReliablePacket (sentPackList[i].Packet, false, sentPackList[i].ID);
						sentPackList[i].SendTimer = 0;
					}
				}
				loopTime.Restart();

				if (pingTimer.Elapsed.TotalSeconds > 2) {
					ping ();
					Debug.WriteLine ("Ping sent");
					pingTimer.Restart ();
				}

				if (timeoutTimer.Elapsed.TotalSeconds > 10) {
					if (Timeout != null) {
						Timeout ();
						Debug.WriteLine ("Session timed out");
					}

					break;
				}

				Thread.Sleep (1);

		
			}
		}
	}
}

