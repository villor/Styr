using System;
using System.Net;

namespace StyrServer
{
	public class Client
	{
		private IPEndPoint _ep;
		public IPEndPoint EndPoint {
			get {
				return _ep;
			}
			set {
				_ep = value;
			}
		}

		public IPAddress IP {
			get {
				return _ep.Address;
			}
			set {
				_ep.Address = value;
			}
		}

		public int Port {
			get {
				return _ep.Port;
			}
			set {
				_ep.Port = value;
			}
		}

		public Client (IPEndPoint ep)
		{
			_ep = ep;
		}
	}
}

