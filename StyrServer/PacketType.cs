using System;

namespace StyrServer
{
	enum PacketType : byte {
		Discovery,
		Offer,
		Connection,
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
		MouseRightDown
	};
}

