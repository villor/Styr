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
			base.OnLongPress (e);
		}

		public override bool OnDoubleTap (MotionEvent e)
		{
			if (DoubleTap != null) {
				DoubleTap (e);
			}
			return base.OnDoubleTap (e);
		}

		public override bool OnDoubleTapEvent (MotionEvent e)
		{
			if (DoubleTapEvent != null) {
				DoubleTapEvent (e);
			}
			return base.OnDoubleTapEvent (e);
		}

		public override bool OnSingleTapUp (MotionEvent e)
		{
			if (SingleTapUp != null) {
				SingleTapUp (e);
			}
			return base.OnSingleTapUp (e);
		}

		public override bool OnDown (MotionEvent e)
		{
			if (Down != null) {
				Down (e);
			}
			return base.OnDown (e);
		}

		public override bool OnFling (MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
		{
			if (Fling != null) {
				Fling (e1, e2, velocityX, velocityY);
			}
			return base.OnFling (e1, e2, velocityX, velocityY);
		}

		public override bool OnScroll (MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
		{
			if (Scroll != null) {
				Scroll (e1, e2, distanceX, distanceY);
			}
			return base.OnScroll (e1, e2, distanceX, distanceY);
		}

		public override void OnShowPress (MotionEvent e)
		{
			if (ShowPress != null) {
				ShowPress (e);
			}
			base.OnShowPress (e);
		}

		public override bool OnSingleTapConfirmed (MotionEvent e)
		{
			if (SingleTapConfirmed != null) {
				SingleTapConfirmed (e);
			}
			return base.OnSingleTapConfirmed (e);
		}
	}
}