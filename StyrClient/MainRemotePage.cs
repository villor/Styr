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
		private bool keyboardWasHidden;

		public MainRemotePage(ref RemoteSession session)
		{
			keyboardWasHidden = false;
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

		protected override bool OnBackButtonPressed ()
		{
			if (keyboardWasHidden) {
				keyboardWasHidden = false;
				return true;
			} else {
				return base.OnBackButtonPressed ();
			}
		}

		public void BuildPageGUI(){

			Title = "Remote Control";
			Icon = "Icon.png";

			var keyboardEditor = new KeyboardEditor ();
			keyboardEditor.InputChar += (c) => {
				remoteSession.sendCharacter(c);
			};

			keyboardEditor.KeyPress += (key) => {
				remoteSession.sendKeyPress(key);            
			};

			keyboardEditor.Completed += (object sender, EventArgs e) => {
				keyboardWasHidden = true;
				keyboardEditor.IsVisible = false;
			};

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
				//Debug.WriteLine("Sending MouseRightClick to remote connected server");
			};

			ToolbarItems.Add(new ToolbarItem("keys", null, () =>{
				keyboardEditor.IsVisible = true;
				keyboardEditor.Focus();
			}));

			Content = new Grid {
				VerticalOptions = LayoutOptions.FillAndExpand,
				ColumnDefinitions = { new ColumnDefinition() },
				RowDefinitions = {new RowDefinition() },
				Children = {
					touchPad,
					new StackLayout() {
						VerticalOptions = LayoutOptions.EndAndExpand,
						Children = { keyboardEditor }
					}
				}
			};
		}
	}
}


