using System;

namespace StyrServer.Input
{
	public struct Point
	{
		public float X;
		public float Y;

		public Point (float x, float y)
		{
			X = x;
			Y = y;
		}
	}

	public interface IMouseDriver
	{
		void MoveTo(float x, float y);
		void RelativeMove(float x, float y);
		Point GetPosition();

		void LeftButtonDown();
		void LeftButtonUp();
		void LeftButtonClick();

		void RightButtonDown();
		void RightButtonUp();
		void RightButtonClick();

		void Scroll(float x, float y);
	}
}

