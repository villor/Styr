using System;
using System.Net;
using System.Net.Sockets;
using Xamarin.Forms;
using System.Diagnostics;

namespace StyrClient
{
	public class MainRemotePage : ContentPage
	{
		//Class members
		IPEndPoint connectedEndPoint;
		UdpClient connectedUdpClient;

		public MainRemotePage(IPEndPoint endPoint, UdpClient udpClient)
		{
			connectedEndPoint = endPoint;
			connectedUdpClient = udpClient;
			BuildPageGUI ();
		}

		~MainRemotePage(){
			Debug.WriteLine ("Instance of MainRemotePage destroyed"); //<---------- Its just not happening
		}

		public void BuildPageGUI(){

			Title = "Remote Control";
			Icon = "Icon.png";

			Content = new StackLayout {
				Spacing = 0,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = {
					new TouchPad {
						HorizontalOptions = LayoutOptions.FillAndExpand,
						VerticalOptions = LayoutOptions.FillAndExpand,
						Color = Color.Maroon,

					}
				}
			};
		}
	}
}


