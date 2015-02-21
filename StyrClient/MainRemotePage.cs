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

		private void BuildPageGUI()
		{
			Title = "Remote Control";
			BackgroundImage = "TPImage.png";

			var touchPad = createTouchPad ();
			var keyboardEditor = createKeyboardEditor ();

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

		private KeyboardEditor createKeyboardEditor()
		{
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

			return keyboardEditor;
		}

		private TouchPad createTouchPad()
		{
			var touchPad = new TouchPad ();

			touchPad.Moved += (float x, float y) => {
				remoteSession.SendMouseMovement(x, y);
			};

			touchPad.Scrolled += (float x, float y) => {
				remoteSession.SendMouseScroll(x, y);
			};

			touchPad.LeftClick += () => {
				remoteSession.SendLeftClick();
			};

			touchPad.LeftUp += () => {
				remoteSession.SendLeftUp();
			};

			touchPad.LeftDown += () => {
				remoteSession.SendLeftDown();
			};

			touchPad.RightClick += () => {
				remoteSession.SendRightClick();
			};

			return touchPad;
		}
	}
}


