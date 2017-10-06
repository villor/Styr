using Android.Views;

namespace Styr.App.Droid
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
		public event OnScrollEventHandler TwoFingerScroll;
		public event OnTouchEventHandler ShowPress;
		public event OnTouchEventHandler SingleTapConfirmed;

		public override void OnLongPress (MotionEvent e)
		{
			LongPress?.Invoke(e);
			base.OnLongPress (e);
		}

		public override bool OnDoubleTap (MotionEvent e)
		{
			DoubleTap?.Invoke(e);
			return base.OnDoubleTap (e);
		}

		public override bool OnDoubleTapEvent (MotionEvent e)
		{
			DoubleTapEvent?.Invoke(e);
			return base.OnDoubleTapEvent (e);
		}

		public override bool OnSingleTapUp (MotionEvent e)
		{
			SingleTapUp?.Invoke(e);
			return base.OnSingleTapUp (e);
		}

		public override bool OnDown (MotionEvent e)
		{
			Down?.Invoke(e);
			return base.OnDown (e);
		}

		public override bool OnFling (MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
		{
			Fling?.Invoke(e1, e2, velocityX, velocityY);
			return base.OnFling (e1, e2, velocityX, velocityY);
		}

		public override bool OnScroll (MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
		{
			if (e2.PointerCount < 2) {
				Scroll?.Invoke(e1, e2, distanceX, distanceY);
			} else if (e2.PointerCount == 2) {
				TwoFingerScroll?.Invoke(e1, e2, distanceX, distanceY);
			}
			return base.OnScroll (e1, e2, distanceX, distanceY);
		}

		public override void OnShowPress (MotionEvent e)
		{
			ShowPress?.Invoke(e);
			base.OnShowPress (e);
		}

		public override bool OnSingleTapConfirmed (MotionEvent e)
		{
			SingleTapConfirmed?.Invoke(e);
			return base.OnSingleTapConfirmed (e);
		}
			
	}
}