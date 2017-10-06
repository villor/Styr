using Xamarin.Forms;

namespace Styr.App
{
	public delegate void OnMoveEventHandler(float x, float y);
	public delegate void OnSingleTapEventHandler();
	public delegate void OnDoubleTapEventHandler();

	public delegate void OnLeftClickEventHandler();
	public delegate void OnLeftDownEventHandler();
	public delegate void OnLeftUpEventHandler();
	public delegate void OnRightClickEventHandler();
	public delegate void OnRightDownEventHandler();
	public delegate void OnRightUpEventHandler();

	public class TouchPad : BoxView
	{

		public event OnMoveEventHandler Moved;
		public event OnMoveEventHandler Scrolled;
		public event OnLeftClickEventHandler LeftClick;
		public event OnLeftDownEventHandler LeftDown;
		public event OnLeftUpEventHandler LeftUp;
		public event OnRightClickEventHandler RightClick;
		public event OnRightDownEventHandler RightDown;
		public event OnRightUpEventHandler RightUp;

		public TouchPad()
		{
			HorizontalOptions = LayoutOptions.FillAndExpand;
			VerticalOptions = LayoutOptions.FillAndExpand;
			Color = Color.Transparent;
		}

		public void OnMove(float x, float y) => Moved?.Invoke(x, y);

		public void OnTwoFingerScroll(float x, float y) => Scrolled?.Invoke(x, y);

		public void OnLeftDown() => LeftDown?.Invoke();

		public void OnLeftUp() => LeftUp?.Invoke();

		public void OnLeftClick() => LeftClick?.Invoke();

		public void OnRightDown() => RightDown?.Invoke();

		public void OnRightUp() => RightUp?.Invoke();

		public void OnRightClick() => RightClick?.Invoke();
	}
}
