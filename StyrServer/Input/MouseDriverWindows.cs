﻿using System;
using System.Runtime.InteropServices;

namespace StyrServer.Input
{
	public class MouseDriverWindows : IMouseDriver
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
			public uint dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
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
			public uint mouseData;
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
			MOUSEEVENTF_ABSOLUTE = 0x8000
		}
		enum SendInputEventType : int
		{
			InputMouse,
			InputKeyboard,
			InputHardware
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int X;
			public int Y;

			public static implicit operator Point(POINT point)
			{
				return new Point(point.X, point.Y);
			}
		}

		[DllImport("user32.dll")]
		public static extern bool GetCursorPos(out POINT lpPoint);

		public MouseDriverWindows()
		{
			if (PlatformDetector.CurrentPlatform != PlatformID.Win32NT)
			{
				throw new PlatformNotSupportedException("Can't run Windows-specific code on this platform!");
			}
		}

		public void MoveTo(float x, float y)
		{
			throw new NotImplementedException();
		}

		public void RelativeMove(float x, float y)
		{
			INPUT mouseInput = new INPUT();
			mouseInput.type = SendInputEventType.InputMouse;
			mouseInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_MOVE;
			mouseInput.mkhi.mi.dx = (int)x;
			mouseInput.mkhi.mi.dy = (int)y;

			SendInput(1, ref mouseInput, Marshal.SizeOf(new INPUT()));
		}

		public Point GetPosition()
		{
			POINT lpPoint;
			GetCursorPos(out lpPoint);
			return lpPoint;
		}

		public void LeftButtonDown()
		{
			INPUT mouseDownInput = new INPUT();
			mouseDownInput.type = SendInputEventType.InputMouse;
			mouseDownInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTDOWN;
			SendInput(1, ref mouseDownInput, Marshal.SizeOf(new INPUT()));
		}

		public void LeftButtonUp()
		{
			INPUT mouseUpInput = new INPUT();
			mouseUpInput.type = SendInputEventType.InputMouse;
			mouseUpInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTUP;
			SendInput(1, ref mouseUpInput, Marshal.SizeOf(new INPUT()));
		}

		public void LeftButtonClick()
		{
			Console.WriteLine("WIN: Left button click!");
			LeftButtonDown();
			LeftButtonUp();
		}

		public void RightButtonDown()
		{
			INPUT mouseDownInput = new INPUT();
			mouseDownInput.type = SendInputEventType.InputMouse;
			mouseDownInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_RIGHTDOWN;
			SendInput(1, ref mouseDownInput, Marshal.SizeOf(new INPUT()));
		}

		public void RightButtonUp()
		{
			INPUT mouseUpInput = new INPUT();
			mouseUpInput.type = SendInputEventType.InputMouse;
			mouseUpInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_RIGHTUP;
			SendInput(1, ref mouseUpInput, Marshal.SizeOf(new INPUT()));
		}

		public void RightButtonClick()
		{
			Console.WriteLine("WIN: Right button click!");
			RightButtonDown();
			RightButtonUp();
		}
	}
}

