using System;

namespace StyrClient.Network
{
	public enum PacketType : byte
	{
		Discovery,
		Offer,
		Connection,
		Disconnect,
		Ack,
		Ping,
		AccessDenied,
		MouseMovement,
		MouseScroll,
		MouseLeftClick,
		MouseLeftUp,
		MouseLeftDown,
		MouseRightClick,
		MouseRightUp,
		MouseRightDown,
		InputCharacter,
		KeyPress
	}
}

