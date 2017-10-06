namespace Styr.Core.Input
{
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
