using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Styr.Core.Client
{
	public class Discovery
	{
		private ObservableServerCollection<Server> discoveredServers;
		private IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Broadcast, 1337);
		private UdpClient udpClient = new UdpClient();

		public Discovery(ObservableServerCollection<Server> servers)
		{
			discoveredServers = servers;
		}

		public Task DiscoverHostsAsync()
		{
			return Task.Run(() =>
			{
				Debug.WriteLine(string.Format("Discovered servers: {0}", discoveredServers.InnerCollection.Count));
				byte[] packet = { (byte)PacketType.Discovery };
				udpClient.Send(packet, packet.Length, remoteEndPoint);

				IPEndPoint ep = new IPEndPoint(IPAddress.Any, 1337);

				var latestOffers = new List<IPEndPoint>();
				while (udpClient.Available > 0)
				{
					byte[] receivedData = udpClient.Receive(ref ep);
					latestOffers.Add(ep);
					if (receivedData.Length != 0)
					{
						if (receivedData[0] == (byte)PacketType.Offer)
						{
							bool duplicate = false;
							foreach (var ss in discoveredServers.InnerCollection)
							{
								if (ep.Equals(ss.EndPoint))
								{
									duplicate = true;
								}
							}
							if (!duplicate)
							{
								Platform platform = (Platform)PacketConverter.GetUShort(receivedData, 1);
								ushort nameLength = PacketConverter.GetUShort(receivedData, 3);
								string serverName = PacketConverter.GetUTF8String(receivedData, 5, nameLength);
								discoveredServers.Add(new Server(ep, serverName, platform));
								Debug.WriteLine("Server discovered: " + serverName + " (" + ep.Address + ") Platform: " + platform);
							}
							duplicate = false;
						}
					}
				}

				// Clear list of dead servers
				var tempList = new List<Server>(discoveredServers.InnerCollection);
				foreach (var ss in tempList)
				{
					bool matchFound = false;
					foreach (IPEndPoint iep in latestOffers)
					{
						if (ss.EndPoint.Equals(iep))
						{
							matchFound = true;
							break;
						}
					}
					if (!matchFound)
					{
						ss.FailedDiscoveries++;
						if (ss.FailedDiscoveries >= 2)
						{
							discoveredServers.Remove(ss);
						}
					}
					else
					{
						ss.FailedDiscoveries = 0;
					}
				}
			});
		}

	}

	public class ObservableServerCollection<T>
	{
		public ObservableCollection<T> InnerCollection;

		public event EventHandler CollectionChanged;

		public ObservableServerCollection(ObservableCollection<T> collection)
		{
			InnerCollection = collection;
		}

		public ObservableServerCollection()
		{
			InnerCollection = new ObservableCollection<T>();
		}

		public void Add(T item)
		{
			InnerCollection.Add(item);
			FireCollectionChanged(EventArgs.Empty);
		}

		public void Remove(T item)
		{
			InnerCollection.Remove(item);
			FireCollectionChanged(EventArgs.Empty);
		}

		public void FireCollectionChanged(EventArgs e)
		{
			CollectionChanged?.Invoke(this, e);
		}
	}
}
