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
			await discovery.DiscoverHostsAsync ();
		}

		public void BuildPageGUI ()
		{
			Title = "Available Hosts";

			ToolbarItems.Add(new ToolbarItem("+", null,  async () => {
				var ipPage = new IpConnectPage();
				await Navigation.PushAsync (ipPage);
				ipPage.Complete += async () => {
					var remoteSession = new RemoteSession (ipPage.IP);
					var mainRemotePage = new MainRemotePage(ref remoteSession);
					await Navigation.PushAsync (mainRemotePage);
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

			listView.ItemTapped += async (sender, e) => {
				var serverItem = (StyrServer)e.Item;
				listView.SelectedItem = null;

				RemoteSession remoteSession = null;
				string errorMessage = "";

				try {
					remoteSession = new RemoteSession (serverItem.EndPoint);
				} catch (UnauthorizedAccessException uex) {
					Debug.WriteLine(uex.StackTrace);
					errorMessage =  "Unauthorized access";
				} catch (SocketException sex) {
					Debug.WriteLine(sex.StackTrace);
					errorMessage = "Socket exception";
				} catch (ArgumentNullException aex) {
					Debug.WriteLine(aex.StackTrace);
					errorMessage = "Argument null exception";
				} catch (Exception ex) {
					Debug.WriteLine(ex.StackTrace);
					errorMessage = "Unknown exception";
				}

				if (remoteSession != null) {
					var mainRemotePage = new MainRemotePage(ref remoteSession);
					await Navigation.PushAsync (mainRemotePage);
				} else {
					await DisplayAlert("Connection failed", errorMessage, "OK");
				}

			};

			Content = new ScrollView {

				Content = listView
			};
		}
	}
}

