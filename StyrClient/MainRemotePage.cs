﻿using System;
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

		private Image keyboardButton;
		private KeyboardEditor keyboardEditor;

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
				keyboardButton.IsVisible = true;
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
			keyboardEditor = createKeyboardEditor ();
			keyboardEditor.VerticalOptions = LayoutOptions.Fill;

			var keyboardButtonTapRecognizer = new TapGestureRecognizer ();
			keyboardButtonTapRecognizer.Tapped += (object sender, EventArgs e) => {
				keyboardButton.IsVisible = false;
				keyboardEditor.IsVisible = true;
				keyboardEditor.Focus();
			};
			keyboardButton = new Image {
				VerticalOptions = LayoutOptions.End,
				HorizontalOptions = LayoutOptions.Center,
				Source = "ic_keyboard_white_48dp.png",
			};
			keyboardButton.GestureRecognizers.Add (keyboardButtonTapRecognizer);

			Content = new Grid {
				VerticalOptions = LayoutOptions.Fill,
				HorizontalOptions = LayoutOptions.Fill,
				ColumnDefinitions = { new ColumnDefinition() },
				RowDefinitions = { new RowDefinition() },
				Children = {
					keyboardEditor,
					touchPad,
					keyboardButton
				}
			};
		}

		private KeyboardEditor createKeyboardEditor()
		{
			var keyboardEditor = new KeyboardEditor ();
			keyboardEditor.VerticalOptions = LayoutOptions.End;

			keyboardEditor.InputChar += (c) => {
				remoteSession.sendCharacter(c);
			};

			keyboardEditor.KeyPress += (key) => {
				remoteSession.sendKeyPress(key);            
			};

			keyboardEditor.Completed += (object sender, EventArgs e) => {
				keyboardWasHidden = true;
				keyboardEditor.IsVisible = false;
				keyboardButton.IsVisible = true;
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

