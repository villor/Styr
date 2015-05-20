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
				Debug.WriteLine ("sending Discovery to address: {0} port {1}", remoteEndPoint.Address, remoteEndPoint.Port);

				IPEndPoint ep = new IPEndPoint (IPAddress.Any, 1337);

				var latestOffers = new List<StyrServer> ();
				while (udpClient.Available > 0) {
					byte[] receivedData = udpClient.Receive (ref ep);
					latestOffers.Add(new StyrServer (ep, "temp"));
					if (receivedData.Length != 0) {
						if (receivedData [0] == (byte)PacketType.Offer) {
							bool duplicate = false;
							foreach (StyrServer ss in discoveredServers){
								if (ep.Equals(ss.EndPoint)){
									duplicate = true;
								}
							}
							if(!duplicate){
								Debug.WriteLine ("Adding Address {0} to list", ep.Address);
								ushort nameLength = PacketConverter.GetUShort(receivedData, 1);
								string serverName = PacketConverter.getUTF8String(receivedData, 3, nameLength);
								Console.WriteLine(serverName);
								discoveredServers.Add (new StyrServer (ep, serverName));
							}
							duplicate = false;
						}
					}
				}

				// Clear list of dead servers
				var tempList = new List<StyrServer> (discoveredServers);
				Console.WriteLine(tempList.Count);
				Console.WriteLine(latestOffers.Count);
				foreach (StyrServer s in latestOffers){
					Console.WriteLine(s.EndPoint);
				}
				foreach (StyrServer ss in tempList){
					bool matchFound = false;
						foreach (StyrServer ss2 in latestOffers){
						if (ss.EndPoint.Equals(ss2.EndPoint)){
							Debug.WriteLine("Match\t {0} - {1}", ss.EndPoint.Address, ss2.EndPoint.Address);
							matchFound = true;
							break;
						}
					}
					if (!matchFound){
						ss.failedDiscoveries++;
						if (ss.failedDiscoveries >= 2)
						{
							discoveredServers.Remove(ss);
							Debug.WriteLine("Removing server {0}", ss.EndPoint.Address);
						}
					}else{
						ss.failedDiscoveries = 0;
					}
				}
			});
		}
	}
}

