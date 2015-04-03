using System;
using Xamarin.Forms;

namespace StyrClient
{

	public delegate void OnMoveEventHandler (float x, float y);
	public delegate void OnSingleTapEventHandler ();
	public delegate void OnDoubleTapEventHandler ();

	public delegate void OnLeftClickEventHandler ();
	public delegate void OnLeftDownEventHandler ();
	public delegate void OnLeftUpEventHandler ();
	public delegate void OnRightClickEventHandler ();
	public delegate void OnRightDownEventHandler ();
	public delegate void OnRightUpEventHandler ();

	public class TouchPad : BoxView {
	
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

		public void OnMove(float x, float y) {
			Console.WriteLine ("OnMove x:{0} y:{1}", x, y);
			if (Moved != null) {
				Moved (x, y);
			}
		}

		public void OnTwoFingerScroll(float x, float y) {
			Console.WriteLine ("OnTwoFingerScroll x:{0} y:{1}", x, y);
			if (Scrolled != null) {
				Scrolled (x, y);
			}
		}

		public void OnLeftDown(){
			Console.WriteLine ("OnLeftDown");
			if (LeftDown != null) {
				LeftDown ();
			}
		}

		public void OnLeftUp(){
			if (LeftUp != null) {
				LeftUp ();
			}
		}

		public void OnLeftClick(){
			Console.WriteLine ("OnLeftClick");
			if (LeftClick != null) {
				LeftClick ();
			}
		}

		public void OnRightDown(){
			Console.WriteLine ("OnRightDown");
			if (RightDown != null) {
				RightDown ();
			}
		}

		public void OnRightUp(){
			if (RightUp != null) {
				RightUp ();
			}
		}

		public void OnRightClick(){
			Console.WriteLine("OnRightClick");
			if (RightClick != null) {
				RightClick ();
			}
		}
	}
}
