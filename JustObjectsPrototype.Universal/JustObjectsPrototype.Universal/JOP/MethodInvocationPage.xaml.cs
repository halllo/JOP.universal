using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace JustObjectsPrototype.Universal.JOP
{
	public sealed partial class MethodInvocationPage : Page
	{
		public MethodInvocationPage()
		{
			this.InitializeComponent();
		}

		public JopViewModel ViewModel { get { return JopViewModel.Instance.Value; } }

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

		private async void AppBarButton_OkClick(object sender, RoutedEventArgs e)
		{
			AppBar.Focus(FocusState.Keyboard);
			await Dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
			{
				await Task.Delay(100);
				await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
				{
					ViewModel.MethodInvocationContinuation.Execute(null);
					(Window.Current.Content as Frame).GoBack(new DrillInNavigationTransitionInfo());
				});
			});
		}
	}
}
