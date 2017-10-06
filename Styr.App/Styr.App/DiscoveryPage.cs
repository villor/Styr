using System;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Styr.Core.Client;

namespace Styr.App
{
	public class DiscoveryPage : ContentPage
	{
		private ObservableServerCollection<Server> AvailableHosts;
		private CancellationTokenSource cts;
		private StackLayout PlaceholderLayout;
		private Label Placeholder;
		private ListView ServerListView;
		private ScrollView ServerScrollView;
		private Discovery discovery;

		public DiscoveryPage()
		{
			AvailableHosts = new ObservableServerCollection<Server>();
			discovery = new Discovery(AvailableHosts);
			cts = new CancellationTokenSource();
			BuildPageGUI();
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			Debug.WriteLine("DiscoveryPage active");
			AvailableHosts.InnerCollection.Clear();
			if (cts != null)
			{
				cts.Cancel();
				cts.Dispose();
				cts = new CancellationTokenSource();
			}
			await DiscoverHosts();
		}

		private async Task DiscoverHosts()
		{
			await RepeatActionEvery(() => discovery.DiscoverHostsAsync(), TimeSpan.FromSeconds(2), cts.Token);
			Debug.WriteLine("Discovery is finished");

		}

		public void BuildPageGUI()
		{
			Title = "Available Hosts";

			var puArgs = new InputPopUpArgs
			{
				Title = "Connect Manually",
				Placeholder = "Enter IP or hostname",
				ButtonText = "Connect"
			};
			puArgs.OnSubmit = async (s) =>
			{
				try
				{
					await Navigation.PopModalAsync(true);
					OpenMainRemotePage(new IPEndPoint(IPAddress.Parse(s), 1337));
				}
				catch (FormatException)
				{
					await DisplayAlert("Incorrect Address", "Check format of IP Address", "OK");
				}
				catch (Exception)
				{
					await DisplayAlert("Error", "Could not connect to remote host", "OK");
				}
			};

			var popup = new InputPopUp(puArgs);

			var IpConnectItem = new ToolbarItem
			{
				Text = "Connect by IP/Host",
				Order = ToolbarItemOrder.Secondary
			};
			IpConnectItem.Clicked += async (sender, e) =>
			{
				await Navigation.PushModalAsync(new NavigationPage(popup));
				Debug.WriteLine("Pushed");
			};

			ToolbarItems.Add(IpConnectItem);

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
				ItemsSource = AvailableHosts.InnerCollection,
				ItemTemplate = new DataTemplate(() =>
				{
					// Create views with bindings for displaying each property.
					Label nameLabel = new Label();
					nameLabel.SetBinding(Label.TextProperty, "ServerName");
					nameLabel.FontSize = 20;
					nameLabel.TextColor = Color.FromHex("#212121");

					Label ipLabel = new Label();
					ipLabel.SetBinding(Label.TextProperty, "IP");

					Image image = new Image();
					image.SetBinding(Image.SourceProperty, "PlatformImageSource");

					// Return an assembled ViewCell.
					return new ViewCell
					{
						View = new StackLayout
						{
							Padding = new Thickness(20, 20),
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

			AvailableHosts.CollectionChanged += (sender, e) => ServerListViewChanged();

			ServerListView.ItemTapped += (sender, e) =>
			{
				var serverItem = (Server)e.Item;
				ServerListView.SelectedItem = null;
				OpenMainRemotePage(serverItem.EndPoint);
			};

			ServerScrollView = new ScrollView
			{
				Content = ServerListView,
				IsVisible = false
			};

			Content = new StackLayout
			{
				Children = {
					PlaceholderLayout,
					ServerScrollView
				}
			};
		}

		private void OpenMainRemotePage(IPEndPoint ep)
		{
			cts.Cancel();
			var mainRemotePage = new MainRemotePage(ep);
			Navigation.PushAsync(mainRemotePage);
		}

		private static async Task RepeatActionEvery(Action action, TimeSpan interval, CancellationToken cancellationToken)
		{
			while (true)
			{
				action();
				Task task = Task.Delay(interval, cancellationToken);

				try
				{
					await task;
				}
				catch (TaskCanceledException)
				{
					return;
				}
			}
		}

		private void ServerListViewChanged()
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				PlaceholderLayout.IsVisible = AvailableHosts.InnerCollection.Count == 0;
				ServerScrollView.IsVisible = !(AvailableHosts.InnerCollection.Count == 0);
			});
		}
	}
}
