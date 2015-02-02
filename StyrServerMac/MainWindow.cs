using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using MonoMac.Foundation;
using MonoMac.AppKit;
using StyrServer;

namespace StyrServerMac
{
	public partial class MainWindow : MonoMac.AppKit.NSWindow
	{
		private Server server;

		#region Constructors

		// Called when created from unmanaged code
		public MainWindow (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindow (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
			Process.Start (new ProcessStartInfo("defaults", "write com.styrserver.mac NSAppSleepDisabled -bool YES"));
			Process.Start (new ProcessStartInfo ("defaults", "write com.styrserver.mac LSAppNapIsDisabled -bool YES"));

			NSProcessInfo.ProcessInfo.DisableAutomaticTermination ("Server");
			NSProcessInfo.ProcessInfo.DisableSuddenTermination ();

			server = new Server (1337);
		}

		#endregion
	}
}

