using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;


namespace StyrClient
{
	public class DiscoveryPage : ContentPage
	{
		enum PacketType : byte
		{
			Discovery,
			Offer
		}

		struct StyrServer {
			public string IP { get; set;}
		}

		//Class members
		List<StyrServer> _availList = new List<StyrServer> ();

		public DiscoveryPage()
		{
			BuildPageGUI ();
			DiscoverHosts ();
		}

		public void BuildPageGUI(){
			Title = "Available Hosts";

			Content = new ScrollView {

				Content = new ListView {

					ItemsSource = _availList,
					ItemTemplate = new DataTemplate (() => {
						Label ipLabel = new Label ();
						ipLabel.SetBinding (Label.TextProperty, "IP");

						return new ViewCell {
							View = new Frame {
								Content = ipLabel
							}
						};
					})

				}
			};
		}

		public void DiscoverHosts ()
		{
			IPAddress send_to_address = IPAddress.Parse ("81.224.126.151");

			IPEndPoint sending_end_point = new IPEndPoint (send_to_address, 1337);

			Debug.WriteLine ("sending Discovery Message");

			byte[] send_buffer = { (byte)PacketType.Discovery };

			UdpClient udpClient = new UdpClient ();
			udpClient.Send (send_buffer, send_buffer.Length, sending_end_point);

			Debug.WriteLine ("sending to address: {0} port {1}", 
				sending_end_point.Address, sending_end_point.Port);


			UdpClient threadClient = udpClient;
			List<StyrServer> threadList = _availList;

			Thread offerListener = new Thread (() => {
				System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch ();
				sw.Start ();

				IPEndPoint ep = new IPEndPoint (IPAddress.Any, 1337);

				while (true) {

					if (threadClient.Available > 0) {
						byte[] receivedData = threadClient.Receive (ref ep);
						if (receivedData.Length != 0) {
							if (receivedData [0] == (byte)PacketType.Offer) {
								Debug.WriteLine ("Adding Address {0} to list", ep.Address.ToString ());
								threadList.Add (new StyrServer{IP = ep.Address.ToString()});
							}
						}
					}else if (sw.Elapsed.TotalSeconds > 10){
						Debug.WriteLine("Exiting thread 'offerListener'");
						break;
					}
				}
			});

			offerListener.Start ();
		}

	}
}

