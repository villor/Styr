using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using StyrClient;
using StyrClient.Droid;
using Android.Views;
using System;

[assembly: ExportRenderer (typeof(TouchPad), typeof(TouchPadRenderer_Droid))]

namespace StyrClient.Droid
{
	public class TouchPadRenderer_Droid : BoxRenderer
	{
		private readonly TouchPadGestureListener listener;
		private readonly GestureDetector detector;

		public TouchPadRenderer_Droid ()
		{
			listener = new TouchPadGestureListener ();
			detector = new GestureDetector (listener);
			detector.IsLongpressEnabled = false;
		}

		protected override void OnElementChanged (ElementChangedEventArgs<BoxView> e)
		{
			base.OnElementChanged (e);
			if (e.NewElement == null) {
				this.GenericMotion -= HandleGenericMotion;
				this.Touch -= HandleTouch;
			}

			if (e.OldElement == null) {
				this.GenericMotion += HandleGenericMotion;
				this.Touch += HandleTouch;

				if (e.NewElement != null) {
					TouchPad tp = (TouchPad)e.NewElement;
					bool scrollSinceLastDown = false;

					listener.SingleTapUp += (e1) => {
						tp.OnLeftDown();
					};

					listener.SingleTapConfirmed += (e1) => {
						tp.OnLeftUp();
					};


					listener.DoubleTapEvent += (e1) => {
						switch (e1.Action)
						{
						case MotionEventActions.Down:
							scrollSinceLastDown = false;
							break;

						case MotionEventActions.Move:
							scrollSinceLastDown = true;
							if (e1.HistorySize > 2){
								tp.OnMove (e1.GetX() - e1.GetHistoricalX(e1.HistorySize - 2), e1.GetY() - e1.GetHistoricalY(e1.HistorySize - 2));
							}
							break;

						case MotionEventActions.Up:
							tp.OnLeftUp();
							if (scrollSinceLastDown == false){
								tp.OnLeftClick();
							}
							Console.WriteLine(scrollSinceLastDown.ToString());
							break;
						}
					};

					listener.Scroll += (e1, e2, distanceX, distanceY) => {
						tp.OnMove (-distanceX, -distanceY);
					};
				}
			}
		}

		void HandleTouch (object sender, TouchEventArgs e)
		{
			detector.OnTouchEvent (e.Event);
		}

		void HandleGenericMotion (object sender, GenericMotionEventArgs e)
		{
			detector.OnTouchEvent (e.Event);
		}
	}
}

