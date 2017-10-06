namespace Styr.Core.Client
{
	public class ReliablePacket
	{
		public byte[] Packet;
		public ushort ID;
		public double ElapsedTime;
		public double SendTimer;
	}
}
