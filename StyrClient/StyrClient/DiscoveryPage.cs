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
		private ObservableCollection<StyrServer> AvailableHosts;
		private CancellationTokenSource cts;
		private StackLayout PlaceholderLayout;
		private Label Placeholder;
		private ListView ServerListView;
		private ScrollView ServerScrollView;
		private Entry IpConnectField;

		public DiscoveryPage ()
		{
			AvailableHosts = new ObservableCollection<StyrServer> ();
			cts = new CancellationTokenSource ();
			BuildPageGUI ();
		}

		protected override async void OnAppearing ()
		{
			base.OnAppearing ();
			Debug.WriteLine ("DiscoveryPage active");
			AvailableHosts.Clear ();
			if (cts != null) {
				cts.Cancel ();
				cts.Dispose ();
				cts = new CancellationTokenSource ();
			}
			await discoverHosts ();
		}

		private async Task discoverHosts()
		{
			Discovery discovery = new Discovery (AvailableHosts);
			await RepeatActionEvery (() => discovery.DiscoverHostsAsync (), TimeSpan.FromSeconds (2), cts.Token);
			Debug.WriteLine("Discovery is finished");

		}

		public void BuildPageGUI ()
		{
			Title = "Available Hosts";

			IpConnectField = new Entry { Placeholder = "IP Address" };
			IpConnectField.IsVisible = false;
			IpConnectField.Completed += (sender, e) => {
				try {
					openMainRemotePage (new IPEndPoint (IPAddress.Parse(IpConnectField.Text), 1337));
				} catch (FormatException) {
					DisplayAlert("Incorrect Address", "Check format of IP Address", "OK");
				} catch (Exception) {
					DisplayAlert("Error", "Could not connect to remote host", "OK");
				}
			};

			ToolbarItems.Add(new ToolbarItem("+", null, () => {
				if (!IpConnectField.IsVisible){
					IpConnectField.IsVisible = true;
				} else {
					IpConnectField.IsVisible = false;

				}
			}));

			Placeholder = new Label
			{
				Text = "There seem to be no available servers in your network.",
				HorizontalTextAlignment = TextAlignment.Center,
				FontSize = 18

			};

			PlaceholderLayout = new StackLayout
			{
				Padding = 10,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Children = {
					Placeholder
				}
			};

			ServerListView = new ListView
			{
				HasUnevenRows = true,
				RowHeight = 120,
				ItemsSource = AvailableHosts,
				ItemTemplate = new DataTemplate (() => {
					// Create views with bindings for displaying each property.
					Label nameLabel = new Label ();
					nameLabel.SetBinding (Label.TextProperty, "ServerName");
					nameLabel.FontSize = 20;
					nameLabel.TextColor = Color.FromHex("#212121");

					Label ipLabel = new Label ();
					ipLabel.SetBinding (Label.TextProperty, "IP");

					Image image = new Image ();
					image.SetBinding (Image.SourceProperty, "PlatformImageSource");
							
					// Return an assembled ViewCell.
					return new ViewCell {
						View = new StackLayout {
							Padding = new Thickness (20, 20),
							VerticalOptions = LayoutOptions.CenterAndExpand,
							HorizontalOptions = LayoutOptions.Center,
							Orientation = StackOrientation.Horizontal,
							Children = {
								image,
								new StackLayout {
									VerticalOptions = LayoutOptions.CenterAndExpand,
									Spacing = 0,
									Padding = 10,
									Children = {
										nameLabel,
										ipLabel
									}
								}
							}
						}
					};
				})
			};

			ServerListView.ChildAdded += (sender, e) => ServerListViewChanged();
			ServerListView.ChildRemoved += (sender, e) => ServerListViewChanged();

			ServerListView.ItemTapped += (sender, e) => {
				var serverItem = (StyrServer)e.Item;
				ServerListView.SelectedItem = null;
				openMainRemotePage(serverItem.EndPoint);
			};

			ServerScrollView = new ScrollView
			{
				Content = ServerListView,
				IsVisible = false
			};

			Content = new StackLayout {
				Children = {
					IpConnectField,
					PlaceholderLayout,
					ServerScrollView
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

		private void ServerListViewChanged() 
		{
			PlaceholderLayout.IsVisible = AvailableHosts.Count == 0;
			ServerScrollView.IsVisible = !(AvailableHosts.Count == 0);
		}
	}
}

