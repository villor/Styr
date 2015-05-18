using System;
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

				Stopwatch offersTimer = new Stopwatch ();
				Stopwatch resendTimer = new Stopwatch ();
				offersTimer.Start ();
				resendTimer.Start ();


				if (resendTimer.Elapsed.TotalSeconds > 1){
					udpClient.Send (packet, packet.Length, remoteEndPoint);
					resendTimer.Restart();
				}

				/* Check if already discovered servers are still live.
				Remove from list if not. */
				var copy = discoveredServers;
				foreach (StyrServer ss in copy){
					if (udpClient.Available > 0){
						Console.WriteLine("This server is avail: {0}", ss.EndPoint.Address.ToString());
					}else{
						discoveredServers.Remove(ss);
					}
				}

				if (udpClient.Available > 0) {
					byte[] receivedData = udpClient.Receive (ref ep);
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
			});
		}
	}
}

