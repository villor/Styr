using System;
using System.Collections.Generic;
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

		public MainRemotePage(){ // <---- For testing purposes
			BuildPageGUI ();
		}

		~MainRemotePage(){
			Debug.WriteLine ("Instance of MainRemotePage destroyed"); // <------- Its just not happening
		}

		public void BuildPageGUI(){

			Title = "Remote Control";
			Icon = "Icon.png";

			var touchPad = new TouchPad {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Color = Color.Maroon

			};
			touchPad.Moved += (float x, float y) => {
				byte[] packet = new byte[9];
				byte[] xArray = BitConverter.GetBytes(x);
				byte[] yArray = BitConverter.GetBytes(y);
				packet[0] = (byte) PacketType.MouseMovement;
				if (BitConverter.IsLittleEndian){
					Array.Reverse(xArray);
					Array.Reverse(yArray);
				}

				Array.Copy(xArray, 0, packet, 1, xArray.Length);
				Array.Copy(yArray, 0, packet, 5, yArray.Length);

				connectedUdpClient.Send(packet, packet.Length, connectedEndPoint);
				Debug.WriteLine ("Sending MouseMovement to {0}", connectedEndPoint.Address);
			};

			Content = new StackLayout {
				Spacing = 0,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = {
					touchPad
				}
			};
		}
	}
}


