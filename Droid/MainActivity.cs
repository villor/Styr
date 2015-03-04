using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace StyrClient.Droid
{
	[Activity (Label = "StyrClient.Droid", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			global::Xamarin.Forms.Forms.Init (this, bundle);

			if (Build.VERSION.SdkInt == BuildVersionCodes.Lollipop) {
				Window.AddFlags (WindowManagerFlags.DrawsSystemBarBackgrounds);
			}

			Window.SetSoftInputMode (SoftInput.AdjustResize);

			LoadApplication (new App ());
		}
	}
}

