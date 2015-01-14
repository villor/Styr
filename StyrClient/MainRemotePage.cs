using System;
using Xamarin.Forms;

namespace StyrClient
{
	public class MainRemotePage : ContentPage
	{
		public MainRemotePage()
		{
			BuildPageGUI ();
		}

		public void BuildPageGUI(){

			Title = "Remote Control";
			Icon = "Icon.png";

			Content = new StackLayout {
				Spacing = 0,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = {
					new TouchPad {
						HorizontalOptions = LayoutOptions.FillAndExpand,
						VerticalOptions = LayoutOptions.FillAndExpand,
						Color = Color.Maroon,

					}
				}
			};
		}
	}
}


