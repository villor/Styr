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


			longPressGestureRecognizer = new UILongPressGestureRecognizer (() => Console.WriteLine ("Long Press"));
			pinchGestureRecognizer = new UIPinchGestureRecognizer (() => Console.WriteLine ("Pinch"));
			panGestureRecognizer = new UIPanGestureRecognizer (() => Console.WriteLine ("Pan" + panGestureRecognizer.TranslationInView(this)));
			swipeGestureRecognizer = new UISwipeGestureRecognizer (() => Console.WriteLine ("Swipe"));
			rotationGestureRecognizer = new UIRotationGestureRecognizer (() => Console.WriteLine ("Rotation"));

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
				this.AddGestureRecognizer (longPressGestureRecognizer);
				this.AddGestureRecognizer (pinchGestureRecognizer);
				this.AddGestureRecognizer (panGestureRecognizer);
				this.AddGestureRecognizer (swipeGestureRecognizer);
				this.AddGestureRecognizer (rotationGestureRecognizer);
			}
		}
	}
}

