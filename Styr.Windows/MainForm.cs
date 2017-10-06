using System;
using System.Windows.Forms;
using Styr.Core.Server;

namespace Styr.Windows
{
	public partial class MainForm : Form
	{
		Server server;
		public MainForm()
		{
			InitializeComponent();
			server = new Server(new MouseDriver(), new KeyboardDriver(), 1337, Core.Platform.Windows);
		}
	}
}
