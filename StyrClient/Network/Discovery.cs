using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace StyrClient.Network
{
	public class Discovery
	{
		private ObservableCollection<StyrServer> discoveredServers;
		private IPEndPoint remoteEndPoint = new IPEndPoint (IPAddress.Broadcast, 1337);
		private UdpClient udpClient = new UdpClient ();

		public Discovery (ObservableCollection<StyrServer> servers)
		{
			discoveredServers = servers;
		}

		public Task DiscoverHostsAsync()
		{
			return Task.Run (() => {

				byte[] packet = { (byte)PacketType.Discovery };
				udpClient.Send (packet, packet.Length, remoteEndPoint);

				IPEndPoint ep = new IPEndPoint (IPAddress.Any, 1337);

				var latestOffers = new List<IPEndPoint> ();
				while (udpClient.Available > 0) {
					byte[] receivedData = udpClient.Receive (ref ep);
					latestOffers.Add(ep);
					if (receivedData.Length != 0) {
						if (receivedData [0] == (byte)PacketType.Offer) {
							bool duplicate = false;
							foreach (StyrServer ss in discoveredServers) {
								if (ep.Equals(ss.EndPoint)) {
									duplicate = true;
								}
							}
							if(!duplicate) {
								PlatformID platform = (PlatformID)PacketConverter.GetUShort(receivedData, 1);
								ushort nameLength = PacketConverter.GetUShort(receivedData, 3);
								string serverName = PacketConverter.getUTF8String(receivedData, 5, nameLength);
								discoveredServers.Add (new StyrServer (ep, serverName, platform));
								Debug.WriteLine ("Server discovered: " + serverName + " (" + ep.Address + ") Platform: " + platform);
							}
							duplicate = false;
						}
					}
				}

				// Clear list of dead servers
				var tempList = new List<StyrServer> (discoveredServers);
				foreach (StyrServer ss in tempList) {
					bool matchFound = false;
						foreach (IPEndPoint iep in latestOffers) {
						if (ss.EndPoint.Equals(iep)) {
							matchFound = true;
							break;
						}
					}
					if (!matchFound){
						ss.failedDiscoveries++;
						if (ss.failedDiscoveries >= 2) {
							discoveredServers.Remove(ss);
						}
					} else {
						ss.failedDiscoveries = 0;
					}
				}
			});
		}
	}
}

