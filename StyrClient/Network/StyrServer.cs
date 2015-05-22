using System;
using System.Net;

namespace StyrClient
{
	public class StyrServer
	{
		public IPEndPoint EndPoint { get; set; }
		public string ServerName { get; set; }
		public PlatformID Platform { get; set; }
		public string IP {
			get {
				return EndPoint.Address.ToString ();
			}
		}
		public ushort failedDiscoveries { get; set; }

		public StyrServer (IPEndPoint endPoint, string serverName, PlatformID platform)
		{
			EndPoint = endPoint;
			ServerName = serverName;
			Platform = platform;
			failedDiscoveries = 0;
		}
	}
}

