using System;

namespace StyrClient.Network
{
	public enum PacketType : byte
	{
		Discovery,
		Offer,
		ConnectionReq,
		Ack,
		Ping,
		AccessDenied,
		MouseMovement,
		MouseLeftClick
	}
}

