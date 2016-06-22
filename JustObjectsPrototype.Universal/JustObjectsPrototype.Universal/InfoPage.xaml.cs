using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace JustObjectsPrototype.Universal
{
	public sealed partial class InfoPage : Page
	{
		public InfoPage()
		{
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
			systemNavigationManager.BackRequested += DetailPage_BackRequested;
			systemNavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			base.OnNavigatedFrom(e);

			SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
			systemNavigationManager.BackRequested -= DetailPage_BackRequested;
			systemNavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
		}

		private void DetailPage_BackRequested(object sender, BackRequestedEventArgs e)
		{
			e.Handled = true;
			Frame.GoBack(new DrillInNavigationTransitionInfo());
		}
	}
}
