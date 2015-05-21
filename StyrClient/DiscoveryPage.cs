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
		private CancellationTokenSource cts;

		public DiscoveryPage ()
		{
			availableHosts = new ObservableCollection<StyrServer> ();
			cts = new CancellationTokenSource ();
			BuildPageGUI ();
		}

		protected override async void OnAppearing ()
		{
			base.OnAppearing ();
			Debug.WriteLine ("DiscoveryPage active");
			availableHosts.Clear ();
			if (cts != null) {
				cts.Cancel ();
				cts.Dispose ();
				cts = new CancellationTokenSource ();
			}
			await discoverHosts ();
		}

		private async Task discoverHosts()
		{
			Discovery discovery = new Discovery (availableHosts);
			await RepeatActionEvery (() => discovery.DiscoverHostsAsync (), TimeSpan.FromSeconds (2), cts.Token);
			Debug.WriteLine("Discovery is finished");

		}

		public void BuildPageGUI ()
		{
			Title = "Available Hosts";

			var ipConnectField = new Entry { Placeholder = "IP Address" };
			ipConnectField.IsVisible = false;
			ipConnectField.Completed += (sender, e) => {
				try {
					openMainRemotePage (new IPEndPoint (IPAddress.Parse(ipConnectField.Text), 1337));
				} catch (FormatException) {
					DisplayAlert("Incorrect Address", "Please check format of IP Address", "OK");
				} catch (Exception) {
					DisplayAlert("Error", "Could not connect to remote host", "OK");
				}
			};

			ToolbarItems.Add(new ToolbarItem("+", null,  async () => {
				if (ipConnectField.IsVisible == false){
					ipConnectField.IsVisible = true;

				} else {
					ipConnectField.IsVisible = false;

				}
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
			};

			var scrollView = new ScrollView {
				Content = listView
			};

			Content = new StackLayout {
				Children = {
					ipConnectField,
					scrollView
				}
			};
		}

		private void openMainRemotePage(IPEndPoint ep)
		{
			cts.Cancel ();
			var mainRemotePage = new MainRemotePage (ep);
			Navigation.PushAsync (mainRemotePage);
		}

		private static async Task RepeatActionEvery(Action action, TimeSpan interval, CancellationToken cancellationToken)
		{
			while (true) {
				action ();
				Task task = Task.Delay (interval, cancellationToken);

				try {
					await task;
				} catch (TaskCanceledException) {
					return;
				}
			}
		}
	}
}

