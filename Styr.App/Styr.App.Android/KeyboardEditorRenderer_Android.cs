﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Views;
using Android.Views.InputMethods;
using Android.Text;
using Styr.App;
using Styr.App.Droid;
using Styr.Core.Input;

[assembly: ExportRenderer (typeof(KeyboardEditor), typeof(KeyboardEditorRenderer_Android))]

namespace Styr.App.Droid
{
	public class KeyboardEditorRenderer_Android : EditorRenderer
	{
		private KeyboardEditor editor;
		private bool wasEmptied;

		protected override void OnElementChanged (ElementChangedEventArgs<Editor> e)
		{
			base.OnElementChanged (e);
			editor = (KeyboardEditor)e.NewElement;
			if (Control != null) {
				Control.SetTextIsSelectable (false);
				Control.SetCursorVisible (false);
				Control.Text = " ";
				Control.SetTextColor (DesignConstants.KeyboardInputTextColor.ToAndroid());
				Control.SetTextSize (Android.Util.ComplexUnitType.Pt, DesignConstants.KeyboardInputTextSize);
				Control.Gravity = GravityFlags.CenterHorizontal | GravityFlags.Bottom;
				Control.SetSelection (Control.Text.Length);
				Control.SetBackgroundColor (Android.Graphics.Color.Transparent);
				Control.InputType = Control.InputType | InputTypes.TextFlagNoSuggestions;
				Control.PrivateImeOptions = "nm";
				Control.ImeOptions = Control.ImeOptions | (ImeAction)ImeFlags.NoFullscreen;

				Control.TextChanged += OnTextChanged;
				Control.Touch += (object sender, TouchEventArgs ev) => {
					Control.SetSelection (Control.Text.Length);
				};
				Control.FocusChange += (object sender, FocusChangeEventArgs ev) => {
					var inputMethodManager = (InputMethodManager)Android.App.Application.Context.GetSystemService(Android.Content.Context.InputMethodService);
					if (ev.HasFocus) {
						inputMethodManager.ShowSoftInput(Control, ShowFlags.Forced);
						inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
					} else {
						inputMethodManager.HideSoftInputFromWindow(Control.WindowToken, HideSoftInputFlags.None);
						ResetInput();
					}
				};

				wasEmptied = false;
			}
		}

		private void OnTextChanged(object sender, Android.Text.TextChangedEventArgs e)
		{
			if (Control != null && editor != null) {
				if (!wasEmptied) {
					if (e.AfterCount < e.BeforeCount) {
						editor.OnKeyPress (KeyboardKey.Backspace);
					} else if (Control.Text [Control.Text.Length - 1] == '\n') {
						ResetInput ();
						editor.OnKeyPress (KeyboardKey.Enter);
					}else if ((e.AfterCount - e.BeforeCount) > 2){  // This else if-clause adds support for gesture typing
						for (int i = 0; i < Control.Text.Length; i++){
							editor.OnInputChar (Control.Text [i]);
						}
					}else if (e.AfterCount > e.BeforeCount) {
						editor.OnInputChar (Control.Text [Control.Text.Length - 1]);
						if (Control.Text [Control.Text.Length - 1] == ' ') {
							ResetInput ();
						}
					}
				} else {
					wasEmptied = false;
				}
				if (Control.Text == "") {
					ResetInput ();
				}
			}
		}

		private void ResetInput()
		{
			wasEmptied = true;
			Control.Text = " ";
			Control.SetSelection (Control.Text.Length);
		}
	}
}

