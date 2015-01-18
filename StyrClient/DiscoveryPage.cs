using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;


namespace StyrClient
{

	enum PacketType : byte
	{
		Discovery,
		Offer,
		ConnectionReq,
		ConnectionAck,
		AccessDenied,
		MouseMovement,
		MouseLeftClick
	}

	public class DiscoveryPage : ContentPage
	{
		struct StyrServer
		{
			public string IP { get; set; }
		}

		//Class members
		List<StyrServer> _availList = new List<StyrServer> ();

		public DiscoveryPage ()
		{
			BuildPageGUI ();
			DiscoverHosts ();
		}

		public void BuildPageGUI ()
		{
			Title = "Available Hosts";

			var listView = new ListView {

				ItemsSource = _availList,
				ItemTemplate = new DataTemplate (typeof(TextCell)),
			};
			listView.ItemTemplate.SetBinding (TextCell.TextProperty, "IP");

			listView.ItemTapped += async (sender, e) => {
				var serverItem = (StyrServer)e.Item;

				IPAddress desiredHost = IPAddress.Parse (serverItem.IP);
				IPEndPoint desiredEndPoint = new IPEndPoint (desiredHost, 1337);
				byte[] packet = { (byte)PacketType.ConnectionReq };
				UdpClient udpClient = new UdpClient ();
				udpClient.Send (packet, packet.Length, desiredEndPoint);
				Debug.WriteLine ("Sending ConnectionReq to {0}", desiredHost);

				byte[] receivedPacket = udpClient.Receive (ref desiredEndPoint);
				if (receivedPacket.Length != 0) {
					if (receivedPacket [0] == (byte)PacketType.ConnectionAck) {
						Debug.WriteLine ("Connection request accepted by {0}", desiredEndPoint.Address);
						var mainRemotePage = new MainRemotePage (desiredEndPoint, udpClient);
						await Navigation.PushAsync (mainRemotePage);
					}else if (receivedPacket [0] == (byte)PacketType.AccessDenied){
						await DisplayAlert("Access Denied", "Connection to " + desiredHost + " failed", "OK");
					}else{
						await DisplayAlert("ERROR", "Connection to " + desiredHost + " failed", "OK");
					}
				}
			};

			Content = new ScrollView {

				Content = listView
			};
		}

		public void DiscoverHosts ()
		{
			IPAddress send_to_address = IPAddress.Parse ("81.224.126.151");
			IPEndPoint sending_end_point = new IPEndPoint (send_to_address, 1337);
			Debug.WriteLine ("sending Discovery Message");

			byte[] send_buffer = {(byte)PacketType.Discovery};

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
								Debug.WriteLine ("Adding Address {0} to list", ep.Address);
								StyrServer Balle = new StyrServer{ IP = ep.Address.ToString () };
								Debug.WriteLine (Balle.IP);
								threadList.Add (Balle);

							}
						}
					} else if (sw.Elapsed.TotalSeconds > 10) {
						Debug.WriteLine ("Exiting thread 'offerListener'");
						break;
					}
				}
			});

			offerListener.Start ();
		}

	}
}

