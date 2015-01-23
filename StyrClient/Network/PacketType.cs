using System;

namespace StyrClient.Network
{
	public enum PacketType : byte
	{
		Discovery,
		Offer,
		ConnectionReq,
		Ack,
		AccessDenied,
		MouseMovement,
		MouseLeftClick
	}
}

