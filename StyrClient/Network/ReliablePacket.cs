using System;

namespace StyrClient.Network
{
	public class ReliablePacket
	{
		public byte[] Packet;
		public ushort ID;
		public double ElapsedTime;
		public double SendTimer;

		public ReliablePacket ()
		{
		}
	}
}

