using System;
using Xamarin.Forms;

namespace StyrClient
{

	public delegate void OnMoveEventHandler (float x, float y);

	public class TouchPad : BoxView {

		public event OnMoveEventHandler Moved;

		public void OnMove(float x, float y) {
			Console.WriteLine ("OnScroll x:{0} y:{1}", x, y);
			if (Moved != null) {
				Moved (x, y);
			}
		}
	}
}

