﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Collections.ObjectModel;

using StyrClient.Network;

namespace StyrClient
{

	public class DiscoveryPage : ContentPage
	{

		//Class members
		ObservableCollection<StyrServer> AvailHosts;

		public DiscoveryPage ()
		{
			AvailHosts = new ObservableCollection<StyrServer> ();

			BuildPageGUI ();
			FindServers ();
		}

		public void BuildPageGUI ()
		{
			Title = "Available Hosts";

			IpConnectPage ipPage;
			ToolbarItems.Add(new ToolbarItem("+", null,  async () =>
				{
					ipPage = new IpConnectPage();
					await Navigation.PushAsync (ipPage);
					ipPage.Complete += async () => {
						RemoteSession remoteSession = new RemoteSession (ipPage.IP); // <---- this is where it goes to shit
						var mainRemotePage = new MainRemotePage(ref remoteSession);
						await Navigation.PushAsync (mainRemotePage);
					};
				}));



			var listView = new ListView {

				ItemsSource = AvailHosts,
				ItemTemplate = new DataTemplate (typeof(TextCell)),
			};
			listView.ItemTemplate.SetBinding (TextCell.TextProperty, "IP");

			listView.ItemTapped += async (sender, e) => {
				var serverItem = (StyrServer)e.Item;

				try {
					RemoteSession remoteSession = new RemoteSession (serverItem.EndPoint); // <---- this is where it goes to shit
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

		public void FindServers ()
		{
			Discovery discovery = new Discovery (ref AvailHosts);
			discovery.DiscoverHosts ();
		}

	}
}

