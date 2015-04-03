﻿using System;
using System.Net;

namespace StyrClient
{
	public class StyrServer
	{
		public IPEndPoint EndPoint { get; set; }
		public string IP {
			get {
				return EndPoint.Address.ToString ();
			}
		}
		public string Detail {
			get {
				return "This is the detail text";
			}
		}

		public StyrServer (IPEndPoint endPoint)
		{
			EndPoint = endPoint;
		}
	}
}
