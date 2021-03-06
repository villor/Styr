﻿using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using System.Diagnostics;
using Styr.App;
using Styr.App.Droid;
using Android.Views;
using System;

[assembly: ExportRenderer (typeof(TouchPad), typeof(TouchPadRenderer_Android))]

public delegate void OnRightClickEventHandler(MotionEvent e);

namespace Styr.App.Droid
{
	public class TouchPadRenderer_Android : BoxRenderer
	{
		public event OnRightClickEventHandler RightClick;
		Stopwatch rightClickTimer = new Stopwatch();

		private readonly TouchPadGestureListener listener;
		private readonly GestureDetector detector;

		public TouchPadRenderer_Android()
		{
			listener = new TouchPadGestureListener();
			detector = new GestureDetector(listener);
			detector.IsLongpressEnabled = false;
		}
	
		protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
		{
			base.OnElementChanged(e);
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

					listener.TwoFingerScroll += (e1, e2, distanceX, distanceY) => {
						tp.OnTwoFingerScroll (distanceX, distanceY);
					};
				}
			}
		}


		void HandleTouch(object sender, TouchEventArgs e)
		{
			/*
			* Det här är kod för att hantera multi touch Right click,
			* som inte hanteras i listener-klassen.
			*/
			switch (e.Event.Action) 
			{
			case MotionEventActions.Pointer2Down:
				Console.WriteLine(e.Event.PointerCount);
				Console.WriteLine("Pointer2Down");
				rightClickTimer.Restart ();
				
				break;
			case MotionEventActions.PointerUp:
				Console.WriteLine("PointerUp");
				if (rightClickTimer.Elapsed.TotalMilliseconds < 250) {
					RightClick ();
					rightClickTimer.Reset ();
				}
				break;

			}

			detector.OnTouchEvent(e.Event);
		}

		void HandleGenericMotion(object sender, GenericMotionEventArgs e)
		{
			detector.OnTouchEvent(e.Event);
		}
	}	
}

