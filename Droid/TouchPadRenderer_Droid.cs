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

					float lastY = 0;
					float lastX = 0;
					listener.DoubleTapEvent += (e1) => {
						switch (e1.Action)
						{
						case MotionEventActions.Down:
							lastX = e1.GetX();
							lastY = e1.GetY();
							scrollSinceLastDown = false;
							break;

						case MotionEventActions.Move:
							scrollSinceLastDown = true;
							tp.OnMove (e1.GetX() - lastX, e1.GetY() - lastY);
							lastX = e1.GetX();
							lastY = e1.GetY();
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

