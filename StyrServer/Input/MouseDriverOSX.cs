using System;
using System.Runtime.InteropServices;

namespace StyrServer.Input
{
	public class MouseDriverOSX : IMouseDriver
	{
		enum CGEventType : uint {
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

		enum CGMouseButton : uint {
			kCGMouseButtonLeft = 0,
			kCGMouseButtonRight = 1,
			kCGMouseButtonCenter = 2
		}; 

		enum CGEventTapLocation : uint {
			kCGHIDEventTap = 0,
			kCGSessionEventTap,
			kCGAnnotatedSessionEventTap 
		};

		[StructLayout(LayoutKind.Sequential)]
		struct CGPoint {
			public float X;
			public float Y;
		}

		[DllImport("ApplicationServices.framework/ApplicationServices")]
		private static extern IntPtr CGEventCreateMouseEvent (IntPtr source, CGEventType mouseType, CGPoint mouseCursorPosition, CGMouseButton mouseButton);

		[DllImport("ApplicationServices.framework/ApplicationServices")]
		private static extern void CGEventPost (CGEventTapLocation tap, IntPtr e);

		[DllImport("ApplicationServices.framework/ApplicationServices")]
		private static extern void CFRelease (IntPtr cf);

		[DllImport("ApplicationServices.framework/ApplicationServices")]
		private static extern IntPtr CGEventCreate (IntPtr source);

		[DllImport("ApplicationServices.framework/ApplicationServices")]
		private static extern CGPoint CGEventGetLocation (IntPtr e);

		public MouseDriverOSX ()
		{
			// TODO: Throw exception when not on OSX
			// throw new PlatformNotSupportedException("<message>"); 
		}

		public void MoveTo (float x, float y)
		{
			CGPoint p = new CGPoint { X = x, Y = y };
			IntPtr move = CGEventCreateMouseEvent(
				IntPtr.Zero, CGEventType.MouseMoved,
				p, CGMouseButton.kCGMouseButtonLeft
			);
			CGEventPost(CGEventTapLocation.kCGHIDEventTap, move);
			CFRelease(move);
		}

		public void RelativeMove (float x, float y)
		{
			Point mousePos = GetPosition ();
			MoveTo (mousePos.X + x, mousePos.Y + y);
		}

		public Point GetPosition ()
		{
			IntPtr mouse = CGEventCreate(IntPtr.Zero);
			CGPoint mousePos = CGEventGetLocation(mouse);
			CFRelease(mouse);
			return new Point { X = mousePos.X, Y = mousePos.Y };
		}

		public Point GetResolution ()
		{
			throw new NotImplementedException ();
		}
	}
}

