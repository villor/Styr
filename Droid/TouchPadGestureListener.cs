using Android.Views;
using System;

namespace StyrClient.Droid
{
	public delegate void OnTouchEventHandler(MotionEvent e);
	public delegate void OnFlingEventHandler(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY);
	public delegate void OnScrollEventHandler(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY);

	public class TouchPadGestureListener : GestureDetector.SimpleOnGestureListener
	{
		public event OnTouchEventHandler LongPress;
		public event OnTouchEventHandler DoubleTap;
		public event OnTouchEventHandler DoubleTapEvent;
		public event OnTouchEventHandler SingleTapUp;
		public event OnTouchEventHandler Down;
		public event OnFlingEventHandler Fling;
		public event OnScrollEventHandler Scroll;
		public event OnTouchEventHandler ShowPress;
		public event OnTouchEventHandler SingleTapConfirmed;

		public override void OnLongPress (MotionEvent e)
		{
			if (LongPress != null) {
				LongPress (e);
			}
			//Console.WriteLine ("OnLongPress");
			base.OnLongPress (e);
		}

		public override bool OnDoubleTap (MotionEvent e)
		{
			if (DoubleTap != null) {
				DoubleTap (e);
			}
			//Console.WriteLine ("OnDoubleTap");
			return base.OnDoubleTap (e);
		}

		public override bool OnDoubleTapEvent (MotionEvent e)
		{
			if (DoubleTapEvent != null) {
				DoubleTapEvent (e);
			}
			//Console.WriteLine ("OnDoubleTapEvent {0}", e.Action.ToString());
			return base.OnDoubleTapEvent (e);
		}

		public override bool OnSingleTapUp (MotionEvent e)
		{
			if (SingleTapUp != null) {
				SingleTapUp (e);
			}
			//Console.WriteLine ("OnSingleTap");
			return base.OnSingleTapUp (e);
		}

		public override bool OnDown (MotionEvent e)
		{
			if (Down != null) {
				Down (e);
			}
			//Console.WriteLine ("OnDown");
			return base.OnDown (e);
		}

		public override bool OnFling (MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
		{
			if (Fling != null) {
				Fling (e1, e2, velocityX, velocityY);
			}
			//Console.WriteLine ("OnFling");
			return base.OnFling (e1, e2, velocityX, velocityY);
		}

		public override bool OnScroll (MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
		{
			if (Scroll != null) {
				Scroll (e1, e2, distanceX, distanceY);
			}
			//Console.WriteLine ("OnScroll");
			return base.OnScroll (e1, e2, distanceX, distanceY);
		}

		public override void OnShowPress (MotionEvent e)
		{
			if (ShowPress != null) {
				ShowPress (e);
			}
			//Console.WriteLine ("OnShowPress");

			base.OnShowPress (e);
		}

		public override bool OnSingleTapConfirmed (MotionEvent e)
		{
			if (SingleTapConfirmed != null) {
				SingleTapConfirmed (e);
			}
			//Console.WriteLine ("OnSingleTapConfirmed");
			return base.OnSingleTapConfirmed (e);
		}
	}
}