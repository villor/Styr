using System;
using Xamarin.Forms;

namespace StyrClient
{
	public class TouchPad : BoxView {
		public void OnMove(float x, float y) {
			Console.WriteLine ("OnScroll x:{0} y:{1}", x, y);
		}
	}
}

