using System;
using System.IO;
using System.Runtime.InteropServices;

namespace StyrServer
{
	public static class PlatformDetector
	{
		public static PlatformID CurrentPlatform { get; private set; }

		static PlatformDetector ()
		{
			CurrentPlatform = Environment.OSVersion.Platform;
			if (CurrentPlatform == PlatformID.Unix && IsRunningOnMac()) {
				CurrentPlatform = PlatformID.MacOSX;
			}
		}

		[DllImport ("libc")]
		private static extern int uname (IntPtr buf);

		private static bool IsRunningOnMac ()
		{
			IntPtr buf = IntPtr.Zero;
			try {
				buf = Marshal.AllocHGlobal (8192);
				if (uname (buf) == 0) {
					string os = Marshal.PtrToStringAnsi (buf);
					if (os == "Darwin")
						return true;
				}
			} catch {
			} finally {
				if (buf != IntPtr.Zero)
					Marshal.FreeHGlobal (buf);
			}
			return false;
		}
	}
}

