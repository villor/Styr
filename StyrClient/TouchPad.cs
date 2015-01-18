using System;
using Xamarin.Forms;

namespace StyrClient
{

	public delegate void OnMoveEventHandler (float x, float y);
	public delegate void OnSingleTapEventHandler ();
	public delegate void OnDoubleTapEventHandler ();

	public class TouchPad : BoxView {

		public event OnMoveEventHandler Moved;
		public event OnSingleTapEventHandler SingleTapped;
		public event OnDoubleTapEventHandler DoubleTapped;

		public void OnMove(float x, float y) {
			Console.WriteLine ("OnScroll x:{0} y:{1}", x, y);
			if (Moved != null) {
				Moved (x, y);
			}
		}

		public void OnSingleTap(){
			Console.WriteLine ("OnSingleTap");
			if (SingleTapped != null) {
				SingleTapped ();
			}
		}

		public void OnDoubleTap(){
			Console.WriteLine ("OnDoubleTap");
			if (DoubleTapped != null) {
				DoubleTapped ();
			}
		}
	}
}

