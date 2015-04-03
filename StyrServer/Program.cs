namespace StyrServer
{
	class Program
	{
		public static void Main ()
		{
			var server = new Server (1337);
			while (server.Running) { System.Threading.Thread.Sleep (100); }
		}
	}
}
