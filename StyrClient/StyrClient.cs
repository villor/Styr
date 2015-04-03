using System;

using Xamarin.Forms;

namespace StyrClient
{
	public class App : Application
	{
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
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

