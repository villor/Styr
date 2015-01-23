using System;

namespace StyrServer
{
	enum PacketType : byte {
		Discovery,
		Offer,
		Connection,
		Ack,
		AccessDenied,
		MouseMovement,
		MouseLeftClick
	};
}

