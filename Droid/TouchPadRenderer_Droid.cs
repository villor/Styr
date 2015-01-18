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
		private readonly TouchPadGestureListener _listener;
		private readonly GestureDetector _detector;

		public TouchPadRenderer_Droid ()
		{
			_listener = new TouchPadGestureListener ();
			_detector = new GestureDetector (_listener);
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

					_listener.SingleTapUp += (e2) => {
						tp.OnSingleTap();
					};

					_listener.Scroll += (e1, e2, distanceX, distanceY) => {
						tp.OnMove (-distanceX, -distanceY);
					};
				}
			}
		}

		void HandleTouch (object sender, TouchEventArgs e)
		{
			_detector.OnTouchEvent (e.Event);
		}

		void HandleGenericMotion (object sender, GenericMotionEventArgs e)
		{
			_detector.OnTouchEvent (e.Event);
		}
	}
}

