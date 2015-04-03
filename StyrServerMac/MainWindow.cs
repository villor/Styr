using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

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
		
			// Ugly hack. Send dummy packets to 192.0.2.1:1 every 200 ms to keep OS X sockets from sleeping or whatever
			Task.Run (() => {
				var udp = new UdpClient();
				while (true) {
					Thread.Sleep(100);
					byte[] p = {1};
					udp.Send(p, p.Length, new IPEndPoint(new IPAddress(3221225985), 1)); 
				}
			});

			server = new Server (1337);
		}

		#endregion
	}
}

