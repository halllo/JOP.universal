using JustObjectsPrototype.Universal.JOP;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace JustObjectsPrototype.Universal.Shell
{
	public sealed partial class DetailPage : Page
	{
		private static DependencyProperty s_itemProperty = DependencyProperty.Register("Item", typeof(ItemViewModel), typeof(DetailPage), new PropertyMetadata(null));

		public static DependencyProperty ItemProperty
		{
			get { return s_itemProperty; }
		}

		public ItemViewModel Item
		{
			get { return (ItemViewModel)GetValue(s_itemProperty); }
			set { SetValue(s_itemProperty, value); }
		}

		public DetailPage()
		{
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("DetailPage.OnNavigatedTo");
			base.OnNavigatedTo(e);

			var item = JopViewModel.Instance.Value.MasterItems.FirstOrDefault(mi => mi.Id == (int)e.Parameter);
			Item = item;
			JopViewModel.Instance.Value.SelectedMasterItem = item;

			var backStack = Frame.BackStack;
			var backStackCount = backStack.Count;

			if (backStackCount > 0)
			{
				var masterPageEntry = backStack[backStackCount - 1];
				backStack.RemoveAt(backStackCount - 1);

				// Doctor the navigation parameter for the master page so it
				// will show the correct item in the side-by-side view.
				var modifiedEntry = new PageStackEntry(
					masterPageEntry.SourcePageType,
					Item.Id,
					masterPageEntry.NavigationTransitionInfo
					);
				backStack.Add(modifiedEntry);
			}

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

		private void OnBackRequested()
		{
			Frame.GoBack(new DrillInNavigationTransitionInfo());
		}

		void NavigateBackForWideState(bool useTransition)
		{
			// Evict this page from the cache as we may not need it again.
			NavigationCacheMode = NavigationCacheMode.Disabled;

			if (useTransition)
			{
				Frame.GoBack(new EntranceNavigationTransitionInfo());
			}
			else
			{
				Frame.GoBack(new SuppressNavigationTransitionInfo());
			}
		}

		private bool ShouldGoToWideState()
		{
			return Window.Current.Bounds.Width >= 720;
		}

		private void PageRoot_Loaded(object sender, RoutedEventArgs e)
		{
			if (ShouldGoToWideState())
			{
				// We shouldn't see this page since we are in "wide master-detail" mode.
				// Play a transition as we are navigating from a separate page.
				NavigateBackForWideState(useTransition: true);
			}
			else
			{
				// Realize the main page content.
				FindName("RootPanel");
			}

			Window.Current.SizeChanged += Window_SizeChanged;
		}

		private void PageRoot_Unloaded(object sender, RoutedEventArgs e)
		{
			Window.Current.SizeChanged -= Window_SizeChanged;
		}

		private void Window_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
		{
			if (ShouldGoToWideState())
			{
				// Make sure we are no longer listening to window change events.
				Window.Current.SizeChanged -= Window_SizeChanged;

				// We shouldn't see this page since we are in "wide master-detail" mode.
				NavigateBackForWideState(useTransition: false);
			}
		}

		private void DetailPage_BackRequested(object sender, BackRequestedEventArgs e)
		{
			e.Handled = true;

			JopViewModel.Instance.Value.SelectedMasterItem = null;
			OnBackRequested();
		}
	}
}
