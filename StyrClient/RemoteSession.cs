using System;
using System.Net.Sockets;
using System.Net;

namespace StyrClient
{
	public class RemoteSession
	{
		private UdpClient udpClient;
		private IPEndPoint remoteEndPoint;

		public RemoteSession (IPEndPoint endPoint)
		{
			remoteEndPoint = endPoint;
			udpClient = new UdpClient ();

			Reconnect (); // <----- Temporary, need try or some shit
		}

		public RemoteSession (string ipAddress, int port) : this (new IPEndPoint (IPAddress.Parse (ipAddress), port)) // <--- No TryParse here(?)
		{
		}

		public RemoteSession (IPAddress ipAddress, int port) : this (new IPEndPoint (ipAddress, port))
		{
		}

		public RemoteSession (string ipAddress) : this (new IPEndPoint (IPAddress.Parse (ipAddress), 1337))
		{
		}

		public bool Reconnect()
		{
			// sending connection request
			byte[] packet = { (byte)PacketType.ConnectionReq };
			udpClient.Send (packet, packet.Length, remoteEndPoint);

			// receiving connection reply
			byte[] receivedPacket = udpClient.Receive (ref remoteEndPoint);
			if (receivedPacket.Length != 0) {
				if (receivedPacket [0] == (byte)PacketType.ConnectionAck) {
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

		public void SendMouseMovement(float x, float y)
		{
			byte[] packet = new byte[9];
			byte[] xArray = BitConverter.GetBytes(x);
			byte[] yArray = BitConverter.GetBytes(y);
			packet[0] = (byte) PacketType.MouseMovement;
			if (BitConverter.IsLittleEndian){
				Array.Reverse(xArray);
				Array.Reverse(yArray);
			}

			Array.Copy(xArray, 0, packet, 1, xArray.Length);
			Array.Copy(yArray, 0, packet, 5, yArray.Length);
			udpClient.Send(packet, packet.Length, remoteEndPoint);
		}

		public void SendLeftClick()
		{
			byte [] packet = {(byte)PacketType.MouseLeftClick};
			udpClient.Send(packet, packet.Length, remoteEndPoint);
		}


	}
}

