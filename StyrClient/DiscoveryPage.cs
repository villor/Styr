using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Collections.ObjectModel;

using StyrClient.Network;
using System.Threading.Tasks;

namespace StyrClient
{

	public class DiscoveryPage : ContentPage
	{
		private ObservableCollection<StyrServer> availableHosts;
		private bool itemSelected = false;

		public DiscoveryPage ()
		{
			availableHosts = new ObservableCollection<StyrServer> ();
			BuildPageGUI ();
		}

		protected override async void OnAppearing ()
		{
			base.OnAppearing ();
			await discoverHosts ();
		}

		private async Task discoverHosts()
		{
			Discovery discovery = new Discovery (availableHosts);
			Thread discoveryThread = new Thread (() => {
				Debug.WriteLine("discoveryThread is live!");
				while(!itemSelected){
					discovery.DiscoverHostsAsync ();
					Thread.Sleep(2000);
				}
				itemSelected = false;
				Debug.WriteLine("Discovery is finished");
			});
			discoveryThread.Start ();

		}

		public void BuildPageGUI ()
		{
			Title = "Available Hosts";

			ToolbarItems.Add(new ToolbarItem("+", null,  async () => {
				var ipPage = new IpConnectPage();
				await Navigation.PushAsync (ipPage);
				ipPage.Complete += async () => {
					var ep = new IPEndPoint (ipPage.IP, 1337);
					openMainRemotePage(ep);
				};
			}));
				
			var listView = new ListView {

				ItemsSource = availableHosts,
				ItemTemplate = new DataTemplate (typeof(ImageCell)),
			};
			listView.ItemTemplate.SetBinding (ImageCell.TextProperty, "ServerName");
			listView.ItemTemplate.SetBinding (ImageCell.DetailProperty, "IP");
			listView.ItemTemplate.SetValue (ImageCell.TextColorProperty, Color.FromHex("#212121"));
			listView.ItemTemplate.SetValue (ImageCell.DetailColorProperty, Color.FromHex ("#727272"));
			listView.ItemTemplate.SetValue (ImageCell.ImageSourceProperty, "ic_action_computer.png");

			listView.ItemTapped += (sender, e) => {
				var serverItem = (StyrServer)e.Item;
				listView.SelectedItem = null;
				openMainRemotePage(serverItem.EndPoint);
				itemSelected = true;
			};

			Content = new ScrollView {

				Content = listView
			};
		}

		private void openMainRemotePage(IPEndPoint ep)
		{
			var mainRemotePage = new MainRemotePage (ep);
			Navigation.PushAsync (mainRemotePage);
		}
	}
}

