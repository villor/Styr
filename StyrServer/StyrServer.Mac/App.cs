using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace StyrServer.Mac
{
	[MonoMac.Foundation.Register("AppDelegate")]
	public class App : NSApplicationDelegate
	{
		Server server;

		public App()
		{
		}

		public override void FinishedLaunching(NSObject notification)
		{
			// Create a Status Bar Menu
			NSStatusBar statusBar = NSStatusBar.SystemStatusBar;

			var item = statusBar.CreateStatusItem(30);
			item.Title = "Styr";
			item.HighlightMode = true;
			item.Menu = new NSMenu("Styr");

			var quit = new NSMenuItem("Quit");
			quit.Activated += (sender, e) =>
			{
				NSApplication.SharedApplication.Terminate(this);
			};
			item.Menu.AddItem(quit);

			server = new Server(1337);
		}
	}
}

