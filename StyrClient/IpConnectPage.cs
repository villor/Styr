using System;
using System.Net.Sockets;
using System.Net;
using Xamarin.Forms;
using StyrClient.Network;

namespace StyrClient
{
	public delegate void OnCompletedEventHandler();

	public class IpConnectPage : ContentPage
	{
		public IPAddress IP { get; set; }

		public event OnCompletedEventHandler Complete;

		public IpConnectPage ()
		{

			Title = "Connect to IP";

			var label = new Label{
				Text = "Input: "
			};

			var entry = new Entry { 
				Placeholder = "IP Address",
				
			};

			entry.Completed += (object sender, EventArgs e) => {
				IP = IPAddress.Parse(entry.Text);
				Complete();
			};

			Content = new StackLayout {
				Children = 
				{
					label,
					entry
				}
			};

		}
	}
}

