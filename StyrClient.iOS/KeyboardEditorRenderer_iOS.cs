using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using StyrClient;
using StyrClient.iOS;
using UIKit;

[assembly: ExportRenderer (typeof(KeyboardEditor), typeof(KeyboardEditorRenderer_iOS))]

namespace StyrClient.iOS
{
	public class KeyboardEditorRenderer_iOS : EditorRenderer
	{
		private KeyboardEditor editor;
		private int oldTextLength;

		public KeyboardEditorRenderer_iOS ()
		{
			oldTextLength = 0;
		}

		protected override void OnElementChanged (ElementChangedEventArgs<Editor> e)
		{
			base.OnElementChanged (e);
			editor = (KeyboardEditor)e.NewElement; 

			if (Control != null) {
				resetInput();
				Control.BackgroundColor = UIColor.Clear;
				Control.TintColor = Control.BackgroundColor;
				Control.AutocorrectionType = UITextAutocorrectionType.No;
				Control.Changed += onTextChanged;
				Control.TextColor = DesignConstants.KeyboardInputTextColor.ToUIColor ();
				Control.TextAlignment = UITextAlignment.Center;
				Control.Font = Control.Font.WithSize (DesignConstants.KeyboardInputTextSize * 1.6f);
			}
		}

		private void onTextChanged(object sender, EventArgs e)
		{
			if (Control != null && editor != null) {
				if (Control.Text.Length <= oldTextLength) {
					editor.OnKeyPress (KeyboardKey.Backspace);
				} else if (Control.Text [Control.Text.Length - 1] == '\n') {
					resetInput ();
					editor.OnKeyPress (KeyboardKey.Enter);
				}else if((Control.Text.Length - oldTextLength) > 2){ // This should add gesture typing support, can't test in sim
					for (int i = 0; i < Control.Text.Length; i++) {
						editor.OnInputChar (Control.Text[i]);
					}
				} else if (Control.Text.Length > oldTextLength) {
					editor.OnInputChar (Control.Text [Control.Text.Length - 1]);
					if (Control.Text [Control.Text.Length - 1] == ' ') {
						resetInput ();
					}
				}
				if (Control.Text == "") {
					resetInput ();
				}
			}
			oldTextLength = Control.Text.Length;
		}

		private void resetInput()
		{
			Control.Text = " ";
		}
	}
}

