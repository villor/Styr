using System;
using System.IO;
using Gtk;

namespace StyrServer
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                FixGTKWindows();
            }

			Application.Init ();
			MainWindow win = new MainWindow ();
			win.Show ();
			Application.Run ();
		}

        // GTK-Sharp fix for Windows
        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
		[return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
		static extern bool SetDllDirectory (string lpPathName);

		static void FixGTKWindows ()
		{
			string location = null;
			using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Xamarin\GtkSharp\InstallFolder")) {
				if (key != null) {
					location = key.GetValue (null) as string;
				}
			}
			if (location == null || !File.Exists (Path.Combine (location, "bin", "libgtk-win32-2.0-0.dll"))) {
				return;
			}
			var path = Path.Combine (location, @"bin");
			try {
                Console.WriteLine("PATH = " + path);
				if (SetDllDirectory (path)) {
					return;
				}
			} catch (EntryPointNotFoundException) {
			}
		}
	}
}
