using System;

using Xamarin.Forms;

namespace StyrClient
{
	public delegate void OnLifeCycleEventHandler ();

	public class App : Application
	{
		public event OnLifeCycleEventHandler Sleep;
		public event OnLifeCycleEventHandler Resume;

		public App ()
		{
			// The root page of your application
			MainPage = new NavigationPage (new DiscoveryPage ());

			/*MainPage = new ContentPage {Content = new StackLayout {
					Spacing = 0,
					VerticalOptions = LayoutOptions.FillAndExpand,			// Für testing pürposes
					Children = {
						new TouchPad {
							HorizontalOptions = LayoutOptions.FillAndExpand,
							VerticalOptions = LayoutOptions.FillAndExpand,
							Color = Color.Maroon

						}
					}
				}
			};*/
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			if (Sleep != null) {
				Sleep ();
			}
		}

		protected override void OnResume ()
		{
			if (Resume != null) {
				Resume ();
			}
		}
	}
}

