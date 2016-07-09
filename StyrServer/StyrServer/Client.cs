using System;
using System.Net;

namespace StyrServer
{
	public class Client
	{
		public Client(IPEndPoint ep)
		{
			EndPoint = ep;
			TimeSinceLastPing = TimeSpan.Zero;
		}

		public IPEndPoint EndPoint { get; set; }
		public IPAddress IP { get; set; }
		public int Port { get; set; }
		public TimeSpan TimeSinceLastPing { get; set; }
	}
}

