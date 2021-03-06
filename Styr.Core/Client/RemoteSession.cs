﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Styr.Core.Input;

namespace Styr.Core.Client
{
	public delegate void OnSessionTimeoutEventHandler();

	public class RemoteSession
	{
		public event OnSessionTimeoutEventHandler Timeout;

		private UdpClient udpClient;
		private IPEndPoint remoteEndPoint;
		private List<ReliablePacket> sentPackList;
		private ushort reliablePacketID;
		private bool connected;

		public RemoteSession(IPEndPoint endPoint)
		{
			remoteEndPoint = endPoint;
			udpClient = new UdpClient();
			sentPackList = new List<ReliablePacket>();
			reliablePacketID = 0;
		}

		public RemoteSession(IPAddress ipAddress) : this(new IPEndPoint(ipAddress, 1337))
		{
		}

		public void Disconnect()
		{
			connected = false;
		}

		public Task Connect()
		{
			return Task.Run(() =>
			{
				Disconnect();

				// sending connection request
				byte[] packet = { (byte)PacketType.Connection };
				SendUnreliablePacket(packet);
				SendUnreliablePacket(packet); // just because

				var asyncResult = udpClient.BeginReceive(null, null);
				asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(4));
				if (asyncResult.IsCompleted)
				{
					try
					{
						byte[] receivedPacket = udpClient.EndReceive(asyncResult, ref remoteEndPoint);
						// receiving connection reply
						if (receivedPacket.Length != 0)
						{
							if (receivedPacket[0] == (byte)PacketType.Ack)
							{
								connected = true;
								Thread backgroundLoop = new Thread(() =>
								{
									Debug.WriteLine("receiverLoop is live!");
									ReceiveResendRemove();
								});
								backgroundLoop.Start();
							}
							else if (receivedPacket[0] == (byte)PacketType.AccessDenied)
							{
								throw new UnauthorizedAccessException("Unauthorized access");
							}
							else
							{
								throw new Exception("Unknown error, connection not established");
							}
						}
						else
						{
							throw new Exception("Received packet contained no data");
						}
						// EndReceive worked and we have received data and remote endpoint
					}
					catch (Exception ex)
					{
						throw ex;
					}
				}
				else
				{
					throw new TimeoutException("Connection Timed Out");
				}
			});
		}

		public void SendMouseMovement(float x, float y)
		{
			byte[] packet = new byte[9];
			byte[] xArray = BitConverter.GetBytes(x);
			byte[] yArray = BitConverter.GetBytes(y);
			packet[0] = (byte)PacketType.MouseMovement;

			PacketConverter.FixEndianess(xArray);
			PacketConverter.FixEndianess(yArray);

			Array.Copy(xArray, 0, packet, 1, xArray.Length);
			Array.Copy(yArray, 0, packet, 5, yArray.Length);
			SendUnreliablePacket(packet);
		}

		public void SendMouseScroll(float x, float y)
		{
			byte[] packet = new byte[9];
			byte[] xArray = BitConverter.GetBytes(x);
			byte[] yArray = BitConverter.GetBytes(y);
			packet[0] = (byte)PacketType.MouseScroll;

			PacketConverter.FixEndianess(xArray);
			PacketConverter.FixEndianess(yArray);

			Array.Copy(xArray, 0, packet, 1, xArray.Length);
			Array.Copy(yArray, 0, packet, 5, yArray.Length);
			SendUnreliablePacket(packet);
		}

		public void SendLeftClick()
		{
			byte[] packet = { (byte)PacketType.MouseLeftClick };
			SendReliablePacket(packet);
		}

		public void SendLeftUp()
		{
			byte[] packet = { (byte)PacketType.MouseLeftUp };
			SendReliablePacket(packet);
		}

		public void SendLeftDown()
		{
			byte[] packet = { (byte)PacketType.MouseLeftDown };
			SendReliablePacket(packet);
		}

		public void SendRightClick()
		{
			byte[] packet = { (byte)PacketType.MouseRightClick };
			SendReliablePacket(packet);
		}

		public void SendCharacter(char character)
		{
			byte[] packet = new byte[3];
			byte[] charBytes = BitConverter.GetBytes(character);
			packet[0] = (byte)PacketType.InputCharacter;

			PacketConverter.FixEndianess(charBytes);

			Array.Copy(charBytes, 0, packet, 1, charBytes.Length);
			SendReliablePacket(packet);
		}

		public void SendKeyPress(KeyboardKey key)
		{
			byte[] packet = { (byte)PacketType.KeyPress, (byte)key };
			SendReliablePacket(packet);
		}

		public void Ping()
		{
			byte[] packet = { (byte)PacketType.Ping };
			SendReliablePacket(packet);
		}

		private void SendReliablePacket(byte[] packet, bool isNewPacket = true, ushort id = 0)
		{

			if (isNewPacket)
			{
				id = reliablePacketID++;

				lock (sentPackList)
				{
					sentPackList.Add(new ReliablePacket
					{
						Packet = packet,
						ID = id,
						ElapsedTime = 0,
						SendTimer = 0
					});
				}
			}

			byte[] idArray = BitConverter.GetBytes(id);

			PacketConverter.FixEndianess(idArray);

			byte[] newPacket = new byte[packet.Length + idArray.Length];
			Array.Copy(packet, 0, newPacket, 0, packet.Length);
			Array.Copy(idArray, 0, newPacket, packet.Length, idArray.Length);

			udpClient.Send(newPacket, newPacket.Length, remoteEndPoint);
		}

		private void SendUnreliablePacket(byte[] packet)
		{
			udpClient.Send(packet, packet.Length, remoteEndPoint);
		}

		private void ReceiveResendRemove()
		{
			Stopwatch loopTime = new Stopwatch();
			Stopwatch pingTimer = new Stopwatch();
			Stopwatch timeoutTimer = new Stopwatch();
			pingTimer.Start();
			timeoutTimer.Start();

			while (connected)
			{

				if (udpClient.Available > 0)
				{
					byte[] receivedPacket = udpClient.Receive(ref remoteEndPoint);

					if (receivedPacket.Length != 0)
					{
						if (receivedPacket[0] == (byte)PacketType.Ack)
						{
							timeoutTimer.Restart();

							byte[] idArray = new byte[2];
							idArray[0] = receivedPacket[1];
							idArray[1] = receivedPacket[2];

							PacketConverter.FixEndianess(idArray);
							ushort idCtrl = BitConverter.ToUInt16(idArray, 0);

							for (int i = sentPackList.Count - 1; i >= 0; i--)
							{
								if (sentPackList[i].ID == idCtrl)
								{
									sentPackList.RemoveAt(i);
								}
							}
						}
						else if (receivedPacket[0] == (byte)PacketType.AccessDenied)
						{
							throw new UnauthorizedAccessException("Unauthorized access");
						}
						else
						{
							throw new Exception("Unknown error, connection not established");
						}
					}
				}

				loopTime.Stop();
				lock (sentPackList)
				{
					for (int i = sentPackList.Count - 1; i >= 0; i--)
					{
						sentPackList[i].ElapsedTime += loopTime.Elapsed.TotalMilliseconds;
						sentPackList[i].SendTimer += loopTime.Elapsed.TotalMilliseconds;
						if (sentPackList[i].ElapsedTime > 1000)
						{
							sentPackList.Remove(sentPackList[i]);   // <------------------ Sessionen ska avbrytas här?
						}
						else if (sentPackList[i].SendTimer > 75)
						{
							Debug.WriteLine("Resending packet with elapsed time: {0}", sentPackList[i].ElapsedTime);
							SendReliablePacket(sentPackList[i].Packet, false, sentPackList[i].ID);
							sentPackList[i].SendTimer = 0;
						}
					}
				}
				loopTime.Restart();

				if (pingTimer.Elapsed.TotalSeconds > 2)
				{
					Ping();
					Debug.WriteLine("Ping sent");
					pingTimer.Restart();
				}

				if (timeoutTimer.Elapsed.TotalSeconds > 10)
				{
					Disconnect();
					if (Timeout != null)
					{
						Timeout();
						Debug.WriteLine("Session timed out");
					}
					break;
				}

				Thread.Sleep(1);
			}
		}
	}
}
