using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Xamarin.Forms;
using System.Diagnostics;

using StyrClient.Network;

namespace StyrClient
{

	public class MainRemotePage : ContentPage
	{

		private RemoteSession remoteSession;

		public MainRemotePage(ref RemoteSession session)
		{
			remoteSession = session;

			remoteSession.Timeout += () => {
				Console.WriteLine("Server has stopped responding");
				SendBackButtonPressed();				
				//await Navigation.PopAsync();   // <--------- Balle
			};
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
				remoteSession.SendMouseMovement(x, y);
				//Debug.WriteLine ("Sending MouseMovement to remote connected server");
			};

			touchPad.Scrolled += (float x, float y) => {
				remoteSession.SendMouseScroll(x, y);
			};

			touchPad.LeftClick += () => {
				remoteSession.SendLeftClick();
				//Debug.WriteLine("Sending MouseLeftClick to remote connected server");
			};

			touchPad.LeftUp += () => {
				remoteSession.SendLeftUp();
				//Debug.WriteLine("Sending MouseLeftClick to remote connected server");
			};

			touchPad.LeftDown += () => {
				remoteSession.SendLeftDown();
				//Debug.WriteLine("Sending MouseLeftClick to remote connected server");
			};

			touchPad.RightClick += () => {
				remoteSession.SendRightClick();
				Debug.WriteLine("Sending MouseRightClick to remote connected server");
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


