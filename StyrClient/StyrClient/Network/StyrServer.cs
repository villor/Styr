using System;
using System.Net;

namespace StyrClient
{
	public class StyrServer
	{
		public IPEndPoint EndPoint { get; set; }
		public string ServerName { get; set; }
		public PlatformID Platform { get; set; }
		public string PlatformImageSource
		{
			get
			{
				switch (Platform)
				{
					case PlatformID.Win32NT:
					case PlatformID.Win32S:
					case PlatformID.Win32Windows:
					case PlatformID.WinCE:
						return DesignConstants.WinImageSource;
					case PlatformID.MacOSX:
					case PlatformID.Unix:
					default:
						return DesignConstants.MacImageSource;
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

		public StyrServer(IPEndPoint endPoint, string serverName, PlatformID platform)
		{
			EndPoint = endPoint;
			ServerName = serverName;
			Platform = platform;
			FailedDiscoveries = 0;
		}
	}
}
