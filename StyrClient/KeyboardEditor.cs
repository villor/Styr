using System;
using Xamarin.Forms;

namespace StyrClient
{
	public delegate void OnInputCharEventHandler(char c);
	public delegate void OnKeyPressEventHandler(KeyboardKey key);

	public class KeyboardEditor : Editor
	{
		public event OnInputCharEventHandler InputChar;
		public event OnKeyPressEventHandler KeyPress;

		public void OnInputChar(char c)
		{
			if (InputChar != null) {
				InputChar (c);
			}
		}

		public void OnKeyPress(KeyboardKey key)
		{
			if (KeyPress != null) {
				KeyPress (key);
			}
		}
	}
}

