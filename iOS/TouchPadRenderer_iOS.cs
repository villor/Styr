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

					longPressGestureRecognizer = new UILongPressGestureRecognizer (() => {
						Console.WriteLine ("Long Press" + longPressGestureRecognizer.LocationInView(this));
						tp.OnMove(longPressGestureRecognizer.LocationInView(this).X, longPressGestureRecognizer.LocationInView(this).Y);
					}); 
					pinchGestureRecognizer = new UIPinchGestureRecognizer (() => Console.WriteLine ("Pinch"));
					panGestureRecognizer = new UIPanGestureRecognizer (() => {
						Console.WriteLine ("Pan" + panGestureRecognizer.LocationInView(this));
						tp.OnMove(panGestureRecognizer.LocationInView(this).X, panGestureRecognizer.LocationInView(this).Y);
					});
					swipeGestureRecognizer = new UISwipeGestureRecognizer (() => Console.WriteLine ("Swipe"));
					rotationGestureRecognizer = new UIRotationGestureRecognizer (() => Console.WriteLine ("Rotation"));

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

