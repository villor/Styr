using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Collections.ObjectModel;


namespace StyrClient
{

	enum PacketType : byte
	{
		Discovery,
		Offer,
		ConnectionReq,
		Ack,
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
		ObservableCollection<StyrServer> _availHosts = new ObservableCollection<StyrServer> ();

		public DiscoveryPage ()
		{
			BuildPageGUI ();
			DiscoverHosts ();
		}

		public void BuildPageGUI ()
		{
			Title = "Available Hosts";

			var listView = new ListView {

				ItemsSource = _availHosts,
				ItemTemplate = new DataTemplate (typeof(TextCell)),
			};
			listView.ItemTemplate.SetBinding (TextCell.TextProperty, "IP");

			listView.ItemTapped += async (sender, e) => {
				var serverItem = (StyrServer)e.Item;

				try {
					RemoteSession remoteSession = new RemoteSession (IPAddress.Parse(serverItem.IP)); // <---- this is where it goes to shit
					var mainRemotePage = new MainRemotePage(ref remoteSession);
					await Navigation.PushAsync (mainRemotePage);
				} catch (UnauthorizedAccessException uex) {
					Debug.WriteLine(uex.StackTrace);
					await DisplayAlert ("Unauthorized access", "Access has been denied", "OK");
				} catch (SocketException sex) {
					Debug.WriteLine(sex.StackTrace);
					await DisplayAlert ("Socket exception", "Something went wrong when connecting", "OK");
				} catch (ArgumentNullException aex) {
					Debug.WriteLine(aex.StackTrace);
					await DisplayAlert ("Argument null exception", "Something went wrong when connecting", "OK");
				} catch (Exception ex) {
					Debug.WriteLine(ex.StackTrace);
					await DisplayAlert ("Unknown exception", "Something went wrong when connecting", "OK");
				}

			};

			Content = new ScrollView {

				Content = listView
			};
		}

		public void DiscoverHosts ()
		{
			IPAddress send_to_address = IPAddress.Parse ("10.7.41.60"); // 81.224.126.151
			IPEndPoint sending_end_point = new IPEndPoint (send_to_address, 1337);
			Debug.WriteLine ("sending Discovery Message");

			byte[] send_buffer = {(byte)PacketType.Discovery};

			UdpClient udpClient = new UdpClient ();
			udpClient.Send (send_buffer, send_buffer.Length, sending_end_point); // Loopa

			Debug.WriteLine ("sending to address: {0} port {1}", 
				sending_end_point.Address, sending_end_point.Port);
				
			UdpClient threadClient = udpClient;
			ObservableCollection<StyrServer> threadList = _availHosts;

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

