using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using System.Diagnostics;
using StyrClient;
using StyrClient.Droid;
using Android.Views;
using System;

[assembly: ExportRenderer (typeof(TouchPad), typeof(TouchPadRenderer_Droid))]

public delegate void OnRightClickEventHandler(MotionEvent e);

namespace StyrClient.Droid
{
	public class TouchPadRenderer_Droid : BoxRenderer
	{
		public event OnRightClickEventHandler RightClick;

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

					RightClick += () => {
						tp.OnRightClick();
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
			/*
			* Det här är kod för att hantera multi touch Right click,
			* som inte hanteras i listener-klassen.
			*/
			Stopwatch rightClickTimer = new Stopwatch ();
			//Console.WriteLine("1 Nu är jag här, och här är bra");
			switch (e.Event.Action) 
			{
			case MotionEventActions.PointerDown:
				rightClickTimer.Restart ();
				break;
			case MotionEventActions.PointerUp:
				//Console.WriteLine("2 Nu är jag här, och här är bra");
				if (rightClickTimer.Elapsed.TotalMilliseconds < 100) {
					//Console.WriteLine("3 Nu är jag här, och här är bra");
					RightClick ();
					rightClickTimer.Reset ();
				}
				break;

			}

			detector.OnTouchEvent (e.Event);
		}

		void HandleGenericMotion (object sender, GenericMotionEventArgs e)
		{
			detector.OnTouchEvent (e.Event);
		}

	}
		
}

