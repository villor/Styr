using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.ObjectModel;
using System.Threading;
using System.Diagnostics;

namespace StyrClient.Network
{
	public class Discovery
	{
		ObservableCollection<StyrServer> discoveredServers;

		public Discovery (ref ObservableCollection<StyrServer> servers)
		{
			discoveredServers = servers;
		}

		public void DiscoverHosts()
		{
			IPAddress remoteAddress = IPAddress.Parse("81.224.126.151"); // 81.224.126.151
			IPEndPoint remoteEndPoint = new IPEndPoint (remoteAddress, 1337);
			UdpClient udpClient = new UdpClient ();

			byte[] packet = { (byte)PacketType.Discovery };
			udpClient.Send (packet, packet.Length, remoteEndPoint);
			Debug.WriteLine ("sending Discovery to address: {0} port {1}", 
				remoteEndPoint.Address, remoteEndPoint.Port);

			UdpClient threadClient = udpClient;
			ObservableCollection<StyrServer> threadList = discoveredServers;

			Thread offerListener = new Thread (() => {
				System.Diagnostics.Stopwatch offersTimer = new System.Diagnostics.Stopwatch ();
				System.Diagnostics.Stopwatch resendTimer = new System.Diagnostics.Stopwatch ();
				offersTimer.Start ();
				resendTimer.Start ();

				IPEndPoint ep = new IPEndPoint (IPAddress.Any, 1337);

				while (true) {

					if (resendTimer.Elapsed.TotalSeconds > 1){
						udpClient.Send (packet, packet.Length, remoteEndPoint);
						resendTimer.Restart();
					}

					if (threadClient.Available > 0) {
						byte[] receivedData = threadClient.Receive (ref ep);
						if (receivedData.Length != 0) {
							if (receivedData [0] == (byte)PacketType.Offer) {
								bool duplicate = false;
								foreach (StyrServer ss in threadList){
									if (ep.Equals(ss.EndPoint)){
										duplicate = true;
									}
								}
								if(!duplicate){
									Debug.WriteLine ("Adding Address {0} to list", ep.Address);
									StyrServer Balle = new StyrServer{ EndPoint = ep };
									Debug.WriteLine (Balle.IP);
									threadList.Add (Balle);
								}
								duplicate = false;

							}
						}
					} else if (offersTimer.Elapsed.TotalSeconds > 10) {
						Debug.WriteLine ("Exiting thread 'offerListener'");
						break;
					}
				}
			});

			offerListener.Start ();
		}
	}
}

