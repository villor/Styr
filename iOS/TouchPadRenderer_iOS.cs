using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using StyrClient;
using StyrClient.iOS;
using UIKit;

[assembly: ExportRenderer (typeof(TouchPad), typeof(TouchPadRenderer_iOS))]

namespace StyrClient.iOS
{
	public class TouchPadRenderer_iOS : BoxRenderer
	{
		UITapGestureRecognizer singleTapGestureRecognizer;
		UITapGestureRecognizer doubleTapGestureRecognizer;
		UILongPressGestureRecognizer longPressGestureRecognizer;
		UIPinchGestureRecognizer pinchGestureRecognizer;
		UIPanGestureRecognizer panGestureRecognizer;
		UISwipeGestureRecognizer swipeGestureRecognizer;
		UIRotationGestureRecognizer rotationGestureRecognizer;

		public TouchPadRenderer_iOS ()
		{
		}

		protected override void OnElementChanged (ElementChangedEventArgs<BoxView> e){
			base.OnElementChanged (e);

			if (e.NewElement == null) {
				if (longPressGestureRecognizer != null) {
					this.RemoveGestureRecognizer (longPressGestureRecognizer);
				}
				if (pinchGestureRecognizer != null) {
					this.RemoveGestureRecognizer (pinchGestureRecognizer);
				}
				if (panGestureRecognizer != null) {
					this.RemoveGestureRecognizer (panGestureRecognizer);
				}
				if (swipeGestureRecognizer != null) {
					this.RemoveGestureRecognizer (swipeGestureRecognizer);
				}
				if (rotationGestureRecognizer != null) {
					this.RemoveGestureRecognizer (rotationGestureRecognizer);
				}
			}

			if (e.OldElement == null) {
			
				if (e.NewElement != null) {
					TouchPad tp = (TouchPad)e.NewElement;
					CoreGraphics.CGPoint lastPos = new CoreGraphics.CGPoint(-1, -1);

					singleTapGestureRecognizer = new UITapGestureRecognizer (() => {
						tp.OnSingleTap();
					});

					doubleTapGestureRecognizer = new UITapGestureRecognizer (() => {
						tp.OnDoubleTap();
					});

					singleTapGestureRecognizer.NumberOfTapsRequired = 1;
					singleTapGestureRecognizer.RequireGestureRecognizerToFail (doubleTapGestureRecognizer);
					doubleTapGestureRecognizer.NumberOfTapsRequired = 2;

					longPressGestureRecognizer = new UILongPressGestureRecognizer (() => {
						if (lastPos.X > 0 && lastPos.Y > 0){
							tp.OnMove((float)longPressGestureRecognizer.LocationInView(this).X - (float)lastPos.X, (float)longPressGestureRecognizer.LocationInView(this).Y - (float)lastPos.Y);
						}
						lastPos = longPressGestureRecognizer.LocationInView(this);

					}); 

					pinchGestureRecognizer = new UIPinchGestureRecognizer (() => Console.WriteLine ("Pinch"));

					panGestureRecognizer = new UIPanGestureRecognizer (() => {
						if (panGestureRecognizer.State == UIGestureRecognizerState.Began){
							lastPos.X = -1;
							lastPos.Y = -1;
						}
						if (lastPos.X > 0 && lastPos.Y > 0){
							tp.OnMove((float)panGestureRecognizer.LocationInView(this).X - (float)lastPos.X, (float)panGestureRecognizer.LocationInView(this).Y - (float)lastPos.Y);
						}
						lastPos = panGestureRecognizer.LocationInView(this);
					});

					swipeGestureRecognizer = new UISwipeGestureRecognizer (() => Console.WriteLine ("Swipe"));

					rotationGestureRecognizer = new UIRotationGestureRecognizer (() => Console.WriteLine ("Rotation"));

					this.AddGestureRecognizer (singleTapGestureRecognizer);
					this.AddGestureRecognizer (doubleTapGestureRecognizer);
					this.AddGestureRecognizer (longPressGestureRecognizer);
					this.AddGestureRecognizer (pinchGestureRecognizer);
					this.AddGestureRecognizer (panGestureRecognizer);
					this.AddGestureRecognizer (swipeGestureRecognizer);
					this.AddGestureRecognizer (rotationGestureRecognizer);
				}
			}
		}
	}
}

