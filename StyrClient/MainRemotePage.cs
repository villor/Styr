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

			var KeyboardEditor = new Editor {
				BackgroundColor = Color.Transparent,
				Text = " "
			};


			KeyboardEditor.Completed += (object sender, EventArgs e) => {
				Console.WriteLine("Editor Completed");
			};

			KeyboardEditor.TextChanged += (object sender, TextChangedEventArgs e) => { // Det här suger kuk
				if (e.OldTextValue != null && e.NewTextValue.Length >= 1){
					if (e.NewTextValue[e.NewTextValue.Length - 1].Equals('\n')){
						KeyboardEditor.Text = " ";
						remoteSession.sendKeyPress(KeyboardKey.Enter);
					} else if (e.NewTextValue.Length > e.OldTextValue.Length){
						Console.WriteLine(e.NewTextValue.Length);
						Console.WriteLine(e.NewTextValue[e.NewTextValue.Length - 1]);
						remoteSession.sendCharacter((char) e.NewTextValue[e.NewTextValue.Length - 1]);
					} else if (e.NewTextValue.Length <= e.OldTextValue.Length ){
						Console.WriteLine("Backspace");
						remoteSession.sendKeyPress(KeyboardKey.Backspace);
					}
				}
			};

			/*
				if (e.OldTextValue != null && e.NewTextValue.Length >= 1){
					if (e.NewTextValue[e.NewTextValue.Length - 1].Equals('\n')){
						KeyboardEditor.Text = " ";
						remoteSession.sendKeyPress(KeyboardKey.Enter);
					} else if (e.NewTextValue.Length > e.OldTextValue.Length){
						Console.WriteLine(e.NewTextValue.Length);
						Console.WriteLine(e.NewTextValue[e.NewTextValue.Length - 1]);
						remoteSession.sendCharacter((char) e.NewTextValue[e.NewTextValue.Length - 1]);
					} else if (e.NewTextValue.Length < e.OldTextValue.Length ){
						Console.WriteLine("Backspace");
						remoteSession.sendKeyPress(KeyboardKey.Backspace);
					}
				}

			bool cheat = false;
			KeyboardEditor.TextChanged += (object sender, TextChangedEventArgs e) => {
				if (!cheat){
					if (e.OldTextValue != null && e.NewTextValue.Length >= 1){
						if (e.NewTextValue[e.NewTextValue.Length - 1].Equals('\n')){
							cheat = true;
							KeyboardEditor.Text = " ";
							remoteSession.sendKeyPress(KeyboardKey.Enter);
						} else if (e.NewTextValue.Length > e.OldTextValue.Length){
							Console.WriteLine(e.NewTextValue.Length);
							Console.WriteLine(e.NewTextValue[e.NewTextValue.Length - 1]);
							remoteSession.sendCharacter((char) e.NewTextValue[e.NewTextValue.Length - 1]);
						} else if (e.NewTextValue.Length <= e.OldTextValue.Length ){
							Console.WriteLine("Backspace");
							remoteSession.sendKeyPress(KeyboardKey.Backspace);
						}
					}
					if (e.NewTextValue.Length == 0){
						cheat = true;
						KeyboardEditor.Text = " ";
					}
				} else {
					cheat = false;
				}
			};
			*/



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

			Content = new StackLayout {
				Spacing = 0,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = {
					touchPad,
					KeyboardEditor
				}
			};
		}
	}
}


