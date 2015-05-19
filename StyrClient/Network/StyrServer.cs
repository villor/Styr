using System;
using System.Net;

namespace StyrClient
{
	public class StyrServer
	{
		public IPEndPoint EndPoint { get; set; }
		public string ServerName { get; set; }
		public byte ServerOS { get; set; }
		public string IP {
			get {
				return EndPoint.Address.ToString ();
			}
		}
		public ushort failedDiscoveries { get; set; }

		public StyrServer (IPEndPoint endPoint, string serverName)
		{
			EndPoint = endPoint;
			ServerName = serverName;
			failedDiscoveries = 0;
		}
	}
}

