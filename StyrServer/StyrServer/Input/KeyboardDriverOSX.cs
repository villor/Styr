using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace StyrServer.Input
{
	public class KeyboardDriverOSX : IKeyboardDriver
	{
		enum CGEventType : uint
		{
			Null = 0,
			KeyDown = 10,
			KeyUp = 11
		};

		enum CGEventTapLocation : uint
		{
			kCGHIDEventTap = 0,
			kCGSessionEventTap,
			kCGAnnotatedSessionEventTap
		};

		[DllImport("ApplicationServices.framework/ApplicationServices")]
		private static extern void CGEventPost(CGEventTapLocation tap, IntPtr e);

		[DllImport("ApplicationServices.framework/ApplicationServices")]
		private static extern void CFRelease(IntPtr cf);

		[DllImport("ApplicationServices.framework/ApplicationServices")]
		private static extern IntPtr CGEventCreateKeyboardEvent(IntPtr source, ushort virtualKey, bool keyDown);

		[DllImport("ApplicationServices.framework/ApplicationServices", CharSet = CharSet.Unicode)]
		private static extern void CGEventKeyboardSetUnicodeString(IntPtr e, int length, StringBuilder s);

		public KeyboardDriverOSX()
		{
			if (PlatformDetector.CurrentPlatform != PlatformID.MacOSX)
			{
				throw new PlatformNotSupportedException("Can't run OSX-specific code on this platform!");
			}
		}

		public void InputChar(char c)
		{
			IntPtr eDown = CGEventCreateKeyboardEvent(IntPtr.Zero, 0, true);
			IntPtr eUp = CGEventCreateKeyboardEvent(IntPtr.Zero, 0, false);

			StringBuilder s = new StringBuilder(c.ToString(), 1);

			CGEventKeyboardSetUnicodeString(eDown, s.Capacity, s);
			CGEventKeyboardSetUnicodeString(eUp, s.Capacity, s);

			CGEventPost(CGEventTapLocation.kCGHIDEventTap, eDown);
			CGEventPost(CGEventTapLocation.kCGHIDEventTap, eUp);

			CFRelease(eDown);
			CFRelease(eUp);
		}

		public void KeyPress(KeyboardKey key)
		{
			IntPtr eDown = CGEventCreateKeyboardEvent(IntPtr.Zero, virtualKeyCode(key), true);
			IntPtr eUp = CGEventCreateKeyboardEvent(IntPtr.Zero, virtualKeyCode(key), false);

			CGEventPost(CGEventTapLocation.kCGHIDEventTap, eDown);
			CGEventPost(CGEventTapLocation.kCGHIDEventTap, eUp);

			CFRelease(eDown);
			CFRelease(eUp);
		}

		private ushort virtualKeyCode(KeyboardKey key)
		{
			switch (key)
			{
				case KeyboardKey.Enter:
					return 0x24;
				case KeyboardKey.Backspace:
					return 0x33;
			}

			return 0;
		}
	}
}

