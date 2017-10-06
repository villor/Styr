using System;
using System.Net;

namespace Styr.Core.Client
{
	public class Server
	{
		public IPEndPoint EndPoint { get; set; }
		public string ServerName { get; set; }
		public PlatformID Platform { get; set; }

		public string IP
		{
			get
			{
				return EndPoint.Address.ToString();
			}
		}

		public ushort FailedDiscoveries { get; set; }
		public string ImageSource { get; }

		public Server(IPEndPoint endPoint, string serverName, PlatformID platform)
		{
			EndPoint = endPoint;
			ServerName = serverName;
			Platform = platform;
			FailedDiscoveries = 0;
		}
	}
}
