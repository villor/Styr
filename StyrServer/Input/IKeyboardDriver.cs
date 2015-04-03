using System;

namespace StyrServer.Input
{
	public interface IKeyboardDriver
	{
		void InputChar (char c);
		void KeyPress (KeyboardKey key);
	}
}

