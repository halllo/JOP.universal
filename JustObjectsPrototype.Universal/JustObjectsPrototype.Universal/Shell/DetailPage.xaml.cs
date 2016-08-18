using System.Collections.Specialized;
using System.Linq;
using JustObjectsPrototype.Universal.JOP;
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
			Loaded += DetailPage_Loaded;
		}

		private void DetailPage_Loaded(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("DetailPage.Loaded");

			Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => detailView.Prepare());
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
			JopViewModel.Instance.Value.DirectItemsChanged += DirectItemsChanged;
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			JopViewModel.Instance.Value.DirectItemsChanged -= DirectItemsChanged;
			base.OnNavigatedFrom(e);

			SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
			systemNavigationManager.BackRequested -= DetailPage_BackRequested;
			systemNavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

			detailView.Unprepare();
		}

		void DirectItemsChanged(NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems.Contains(Item.Tag))
			{
				OnBackRequested();
			}
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				OnBackRequested();
			}
		}

		void OnBackRequested()
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

		bool ShouldGoToWideState()
		{
			return Window.Current.Bounds.Width >= 720;
		}

		void PageRoot_Loaded(object sender, RoutedEventArgs e)
		{
			if (ShouldGoToWideState())
			{
				// We shouldn't see this page since we are in "wide master-detail" mode.
				// Play a transition as we are navigating from a separate page.
				NavigateBackForWideState(useTransition: true);
			}

			Window.Current.SizeChanged += Window_SizeChanged;
		}

		void PageRoot_Unloaded(object sender, RoutedEventArgs e)
		{
			Window.Current.SizeChanged -= Window_SizeChanged;
		}

		void Window_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
		{
			if (ShouldGoToWideState())
			{
				// Make sure we are no longer listening to window change events.
				Window.Current.SizeChanged -= Window_SizeChanged;

				// We shouldn't see this page since we are in "wide master-detail" mode.
				NavigateBackForWideState(useTransition: false);
			}
		}

		void DetailPage_BackRequested(object sender, BackRequestedEventArgs e)
		{
			e.Handled = true;

			JopViewModel.Instance.Value.SelectedMasterItem = null;
			OnBackRequested();
		}
	}
}
