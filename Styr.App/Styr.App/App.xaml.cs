using Xamarin.Forms;

namespace Styr.App
{
	public delegate void OnLifeCycleEventHandler();

	public partial class App : Application
	{
		public event OnLifeCycleEventHandler Sleep;
		public event OnLifeCycleEventHandler Resume;

		public App()
		{
			InitializeComponent();
			MainPage = new NavigationPage(new DiscoveryPage());
		}

		protected override void OnStart() { }
		protected override void OnSleep() => Sleep?.Invoke();
		protected override void OnResume() => Resume?.Invoke();
	}
}
