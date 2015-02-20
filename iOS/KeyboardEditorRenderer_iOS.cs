using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using StyrClient;
using StyrClient.iOS;
using UIKit;
using CoreText;
using ObjCRuntime;
using System.Linq;
using System.Text;
using Foundation;

[assembly: ExportRenderer (typeof(KeyboardEditor), typeof(KeyboardEditorRenderer_iOS))]

namespace StyrClient.iOS
{
	public class KeyboardEditorRenderer_iOS : EditorRenderer
	{
		private KeyboardEditor editor;
		private int oldTextLength;

		//private string newText;

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
				Control.AutocorrectionType = UITextAutocorrectionType.No;
				Control.Changed += onTextChanged;

				//Control.SelectedRange = new NSRange {
				//	Location = Control.Text.Length,
				//	Length = 0
				//};

			}
		}

		private void onTextChanged(object sender, EventArgs e)
		{
			if (Control != null && editor != null) {
					if (Control.Text.Length <= oldTextLength) {
						//Console.WriteLine ("Backspace");
						editor.OnKeyPress (KeyboardKey.Backspace);
					} else if (Control.Text [Control.Text.Length - 1] == '\n') {
						//Console.WriteLine ("Enter");
						resetInput ();
						editor.OnKeyPress (KeyboardKey.Enter);
					} else if (Control.Text.Length > oldTextLength) {
						//Console.WriteLine ("Input");
						editor.OnInputChar (Control.Text [Control.Text.Length - 1]);
						if (Control.Text [Control.Text.Length - 1] == ' ') {
							//Console.WriteLine ("Space");
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
			//Console.WriteLine ("Nu körs skiteventet");
			Control.Text = " ";
		}
	}
}

