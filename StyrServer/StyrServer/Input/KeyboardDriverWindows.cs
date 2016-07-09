using System;
using System.Runtime.InteropServices;

namespace StyrServer.Input
{
	public class KeyboardDriverWindows : IKeyboardDriver
	{
		[DllImport("user32.dll", SetLastError = true)]
		static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

		[StructLayout(LayoutKind.Sequential)]
		struct INPUT
		{
			public SendInputEventType type;
			public MouseKeybdhardwareInputUnion mkhi;
		}

		[StructLayout(LayoutKind.Explicit)]
		struct MouseKeybdhardwareInputUnion
		{
			[FieldOffset(0)]
			public MouseInputData mi;

			[FieldOffset(0)]
			public KEYBDINPUT ki;

			[FieldOffset(0)]
			public HARDWAREINPUT hi;
		}

		[StructLayout(LayoutKind.Sequential)]
		struct KEYBDINPUT
		{
			public ushort wVk;
			public ushort wScan;
			public KeyboardEventFlags dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		[Flags]
		enum KeyboardEventFlags : uint
		{
			KEYEVENTF_EXTENDEDKEY = 0x0001,
			KEYEVENTF_KEYUP = 0x0002,
			KEYEVENTF_SCANCODE = 0x0008,
			KEYEVENTF_UNICODE = 0x0004
		}

		[StructLayout(LayoutKind.Sequential)]
		struct HARDWAREINPUT
		{
			public int uMsg;
			public short wParamL;
			public short wParamH;
		}

		struct MouseInputData
		{
			public int dx;
			public int dy;
			public int mouseData;
			public MouseEventFlags dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		[Flags]
		enum MouseEventFlags : uint
		{
			MOUSEEVENTF_MOVE = 0x0001,
			MOUSEEVENTF_LEFTDOWN = 0x0002,
			MOUSEEVENTF_LEFTUP = 0x0004,
			MOUSEEVENTF_RIGHTDOWN = 0x0008,
			MOUSEEVENTF_RIGHTUP = 0x0010,
			MOUSEEVENTF_MIDDLEDOWN = 0x0020,
			MOUSEEVENTF_MIDDLEUP = 0x0040,
			MOUSEEVENTF_XDOWN = 0x0080,
			MOUSEEVENTF_XUP = 0x0100,
			MOUSEEVENTF_WHEEL = 0x0800,
			MOUSEEVENTF_VIRTUALDESK = 0x4000,
			MOUSEEVENTF_ABSOLUTE = 0x8000,
			MOUSEEVENTF_HWHEEL = 0x01000
		}

		enum SendInputEventType : int
		{
			InputMouse,
			InputKeyboard,
			InputHardware
		}

		public KeyboardDriverWindows()
		{
			if (PlatformDetector.CurrentPlatform != PlatformID.Win32NT)
			{
				throw new PlatformNotSupportedException("Can't run Windows-specific code on this platform!");
			}
		}

		public void InputChar(char c)
		{
			INPUT input = new INPUT();
			input.type = SendInputEventType.InputKeyboard;
			input.mkhi.ki.wVk = 0;
			input.mkhi.ki.wScan = c;
			input.mkhi.ki.time = 0;
			input.mkhi.ki.dwFlags = KeyboardEventFlags.KEYEVENTF_UNICODE;
			input.mkhi.ki.dwExtraInfo = IntPtr.Zero;
			SendInput(1, ref input, Marshal.SizeOf(typeof(INPUT)));
		}

		public void KeyPress(KeyboardKey key)
		{
			INPUT[] input = new INPUT[2];

			input[0].type = SendInputEventType.InputKeyboard;
			input[0].mkhi.ki.wVk = virtualKeyCode(key);
			input[0].mkhi.ki.wScan = 0;
			input[0].mkhi.ki.time = 0;
			input[0].mkhi.ki.dwFlags = 0;
			input[0].mkhi.ki.dwExtraInfo = IntPtr.Zero;

			input[1].type = SendInputEventType.InputKeyboard;
			input[1].mkhi.ki.wVk = virtualKeyCode(key);
			input[1].mkhi.ki.wScan = 0;
			input[1].mkhi.ki.time = 0;
			input[1].mkhi.ki.dwFlags = KeyboardEventFlags.KEYEVENTF_KEYUP;
			input[1].mkhi.ki.dwExtraInfo = IntPtr.Zero;

			SendInput((uint)input.Length, ref input[0], Marshal.SizeOf(typeof(INPUT)));
		}

		private ushort virtualKeyCode(KeyboardKey key)
		{
			switch (key)
			{
				case KeyboardKey.Enter:
					return 13;
				case KeyboardKey.Backspace:
					return 8;
			}
			return 0;
		}
	}
}

