using System;
using System.Text;
using Gtk;

using StyrServer;

class Logger : System.IO.TextWriter {
	private TextView txt;
	public Logger(TextView txt) { this.txt = txt; }
	public override Encoding Encoding { get { return null; } }
	public override void Write(char value) {
		if (value != '\r') txt.Buffer.Text += new string(value, 1);
	}
}

public partial class MainWindow: Gtk.Window
{
	private Server server;

	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
		//Console.SetOut (new Logger (textview1));
		server = new Server (1337);
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		server.Running = false;
		Application.Quit ();
		a.RetVal = true;
	}
}