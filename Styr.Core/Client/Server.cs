using System;
using System.Net;

namespace Styr.Core.Client
{
	public class Server
	{
		public IPEndPoint EndPoint { get; set; }
		public string ServerName { get; set; }
		public Platform Platform { get; set; }

		public string PlatformImageSource
		{
			get
			{
				// TEMPORARY FIX
				switch (Platform)
				{
					case Platform.Windows:
						return "Win_Client100.png";
					case Platform.MacOS:
					case Platform.Other:
					default:
						return "Mac_Client100.png";
				}
			}
		}

		public string IP
		{
			get
			{
				return EndPoint.Address.ToString();
			}
		}

		public ushort FailedDiscoveries { get; set; }
		public string ImageSource { get; }

		public Server(IPEndPoint endPoint, string serverName, Platform platform)
		{
			EndPoint = endPoint;
			ServerName = serverName;
			Platform = platform;
			FailedDiscoveries = 0;
		}
	}
}
