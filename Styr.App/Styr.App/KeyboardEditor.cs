using Xamarin.Forms;
using Styr.Core.Input;

namespace Styr.App
{
	public delegate void OnInputCharEventHandler(char c);
	public delegate void OnKeyPressEventHandler(KeyboardKey key);

	public class KeyboardEditor : Editor
	{
		public event OnInputCharEventHandler InputChar;
		public event OnKeyPressEventHandler KeyPress;

		public void OnInputChar(char c) => InputChar?.Invoke(c);

		public void OnKeyPress(KeyboardKey key) => KeyPress?.Invoke(key);
	}
}
