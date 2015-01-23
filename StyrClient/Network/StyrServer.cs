using System;
using System.Net;

namespace StyrClient
{
	public struct StyrServer
	{

		public string IP { get; private set; }

		public IPEndPoint EndPoint {
			get
			{
				return pEndPoint;
			}
			set
			{
				pEndPoint = value;
				IP = value.Address.ToString ();
			}
		}

		private IPEndPoint pEndPoint;

	}
}

