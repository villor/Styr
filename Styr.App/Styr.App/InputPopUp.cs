using System;
using Xamarin.Forms;

namespace Styr.App
{
	public class InputPopUp : ContentPage
	{
		private Entry input;
		private Label titleLabel;
		private Label msgLabel;
		private StackLayout innerBox;
		private Button submitButton;
		private InputPopUpArgs args;

		public InputPopUp(string title, string message, string placeholder, string buttonText, Action<string> onSubmit)
		{
			args = new InputPopUpArgs
			{
				Title = title,
				Message = message,
				Placeholder = placeholder,
				ButtonText = buttonText,
				OnSubmit = onSubmit
			};

			BuildGUI();
		}

		public InputPopUp(InputPopUpArgs ipa)
		{
			args = ipa;
			BuildGUI();
		}

		private void BuildGUI()
		{
			BackgroundColor = new Color(0, 0, 0, 0.3);
			Padding = new Thickness(0, 0, 0, 0);


			input = new Entry
			{
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Placeholder = args.Placeholder
			};

			titleLabel = new Label
			{
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Text = args.Title
			};

			msgLabel = new Label
			{
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Text = args.Message
			};

			submitButton = new Button
			{
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Text = args.ButtonText
			};
			submitButton.Clicked += (sender, e) => Submit(args.OnSubmit);

			innerBox = new StackLayout
			{
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				BackgroundColor = Color.White,
				Padding = new Thickness(15, 15),
				Children = {
					titleLabel,
					msgLabel,
					input,
					submitButton
				}
			};

			Content = new StackLayout
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = {
					innerBox
				}
			};

		}

		private void Submit(Action<string> action)
		{
			action(input.Text);
		}
	}

	public class InputPopUpArgs
	{
		public string Title { get; set; }
		public string Message { get; set; }
		public string Placeholder { get; set; }
		public string ButtonText { get; set; }
		public Action<string> OnSubmit { get; set; }
	}
}
