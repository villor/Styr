using System;
using System.Runtime.InteropServices;

namespace StyrServer.Input
{
	public class MouseDriverOSX : IMouseDriver
	{
		enum CGEventType : uint
		{
			Null = 0,
			LeftMouseDown = 1,
			LeftMouseUp = 2,
			RightMouseDown = 3,
			RightMouseUp = 4,
			MouseMoved = 5,
			LeftMouseDragged = 6,
			RightMouseDragged = 7,
			KeyDown = 10,
			KeyUp = 11,
			FlagsChanged = 12,
			ScrollWheel = 22,
			TabletPointer = 23,
			TabletProximity = 24,
			OtherMouseDown = 25,
			OtherMouseUp = 26,
			OtherMouseDragged = 27,
			TapDisabledByTimeout = 4294967294,
			TapDisabledByUserInput = 4294967295
		};

		enum CGMouseButton : uint
		{
			kCGMouseButtonLeft = 0,
			kCGMouseButtonRight = 1,
			kCGMouseButtonCenter = 2
		};

		enum CGEventTapLocation : uint
		{
			kCGHIDEventTap = 0,
			kCGSessionEventTap,
			kCGAnnotatedSessionEventTap
		};

		enum CGScrollEventUnit : uint
		{
			kCGScrollEventUnitPixel = 0,
			kCGScrollEventUnitLine = 1
		};

		[StructLayout(LayoutKind.Sequential)]
		struct CGPoint
		{
			public float X;
			public float Y;
		}

		[DllImport("ApplicationServices.framework/ApplicationServices")]
		private static extern IntPtr CGEventCreateMouseEvent(IntPtr source, CGEventType mouseType, CGPoint mouseCursorPosition, CGMouseButton mouseButton);

		[DllImport("ApplicationServices.framework/ApplicationServices")]
		private static extern void CGEventPost(CGEventTapLocation tap, IntPtr e);

		[DllImport("ApplicationServices.framework/ApplicationServices")]
		private static extern void CFRelease(IntPtr cf);

		[DllImport("ApplicationServices.framework/ApplicationServices")]
		private static extern IntPtr CGEventCreate(IntPtr source);

		[DllImport("ApplicationServices.framework/ApplicationServices")]
		private static extern CGPoint CGEventGetLocation(IntPtr e);

		[DllImport("ApplicationServices.framework/ApplicationServices")]
		private static extern uint CGDisplayPixelsHigh(IntPtr screenId);

		[DllImport("ApplicationServices.framework/ApplicationServices")]
		private static extern uint CGDisplayPixelsWide(IntPtr screenId);

		[DllImport("ApplicationServices.framework/ApplicationServices")]
		private static extern IntPtr CGMainDisplayID();

		[DllImport("ApplicationServices.framework/ApplicationServices")]
		private static extern IntPtr CGEventCreateScrollWheelEvent(IntPtr source, CGScrollEventUnit units, uint wheelCount, int wheel1, int wheel2);

		private bool rightButtonDown = false;
		private bool leftButtonDown = false;

		private uint screenWidth;
		private uint screenHeight;

		public MouseDriverOSX()
		{
			if (PlatformDetector.CurrentPlatform != PlatformID.MacOSX)
			{
				throw new PlatformNotSupportedException("Can't run OSX-specific code on this platform!");
			}
			screenWidth = CGDisplayPixelsWide(CGMainDisplayID());
			screenHeight = CGDisplayPixelsHigh(CGMainDisplayID());
		}

		public void MoveTo(float x, float y)
		{
			CGEventType eventType;

			if (leftButtonDown)
			{
				eventType = CGEventType.LeftMouseDragged;
			}
			else if (rightButtonDown)
			{
				eventType = CGEventType.RightMouseDragged;
			}
			else {
				eventType = CGEventType.MouseMoved;
			}

			CGPoint p = new CGPoint { X = x, Y = y };

			if (p.X > screenWidth - 1)
				p.X = screenWidth - 1;
			if (p.X < 0)
				p.X = 0;
			if (p.Y > screenHeight - 1)
				p.Y = screenHeight - 1;
			if (p.Y < 0)
				p.Y = 0;

			IntPtr move = CGEventCreateMouseEvent(
				IntPtr.Zero, eventType,
				p, CGMouseButton.kCGMouseButtonLeft
			);

			CGEventPost(CGEventTapLocation.kCGHIDEventTap, move);
			CFRelease(move);
		}

		public void RelativeMove(float x, float y)
		{
			Point mousePos = GetPosition();
			MoveTo(mousePos.X + x, mousePos.Y + y);
		}

		public Point GetPosition()
		{
			IntPtr mouse = CGEventCreate(IntPtr.Zero);
			CGPoint mousePos = CGEventGetLocation(mouse);
			CFRelease(mouse);
			return new Point { X = mousePos.X, Y = mousePos.Y };
		}

		public void LeftButtonDown()
		{
			Point mousePos = GetPosition();
			CGPoint p = new CGPoint { X = mousePos.X, Y = mousePos.Y };
			IntPtr click = CGEventCreateMouseEvent(
				IntPtr.Zero, CGEventType.LeftMouseDown,
				p, CGMouseButton.kCGMouseButtonLeft
			);
			CGEventPost(CGEventTapLocation.kCGHIDEventTap, click);
			CFRelease(click);

			leftButtonDown = true;
		}

		public void LeftButtonUp()
		{
			Point mousePos = GetPosition();
			CGPoint p = new CGPoint { X = mousePos.X, Y = mousePos.Y };
			IntPtr click = CGEventCreateMouseEvent(
				IntPtr.Zero, CGEventType.LeftMouseUp,
				p, CGMouseButton.kCGMouseButtonLeft
			);
			CGEventPost(CGEventTapLocation.kCGHIDEventTap, click);
			CFRelease(click);

			leftButtonDown = false;
		}

		public void LeftButtonClick()
		{
			LeftButtonDown();
			LeftButtonUp();
		}

		public void RightButtonDown()
		{
			Point mousePos = GetPosition();
			CGPoint p = new CGPoint { X = mousePos.X, Y = mousePos.Y };
			IntPtr click = CGEventCreateMouseEvent(
				IntPtr.Zero, CGEventType.RightMouseDown,
				p, CGMouseButton.kCGMouseButtonRight
			);
			CGEventPost(CGEventTapLocation.kCGHIDEventTap, click);
			CFRelease(click);

			rightButtonDown = true;
		}

		public void RightButtonUp()
		{
			Point mousePos = GetPosition();
			CGPoint p = new CGPoint { X = mousePos.X, Y = mousePos.Y };
			IntPtr click = CGEventCreateMouseEvent(
				IntPtr.Zero, CGEventType.RightMouseUp,
				p, CGMouseButton.kCGMouseButtonRight
			);
			CGEventPost(CGEventTapLocation.kCGHIDEventTap, click);
			CFRelease(click);

			rightButtonDown = false;
		}

		public void RightButtonClick()
		{
			RightButtonDown();
			RightButtonUp();
		}

		public void Scroll(float x, float y)
		{
			var scroll = CGEventCreateScrollWheelEvent(
				IntPtr.Zero,
				CGScrollEventUnit.kCGScrollEventUnitPixel,
				2,
				(int)-y,
				(int)-x
			);
			CGEventPost(CGEventTapLocation.kCGHIDEventTap, scroll);
			CFRelease(scroll);
		}
	}
}

