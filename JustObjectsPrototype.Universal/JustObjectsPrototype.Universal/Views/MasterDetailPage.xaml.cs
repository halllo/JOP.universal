using JustObjectsPrototype.Universal.Controls;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace JustObjectsPrototype.Universal.Views
{
	public partial class MasterDetailPage : Page
	{
		private ItemViewModel _lastSelectedItem;

		public MasterDetailPage()
		{
			this.InitializeComponent();

			DataContext = new MainViewModel();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			var items = MasterListView.ItemsSource as List<ItemViewModel>;

			if (items == null)
			{
				items = new List<ItemViewModel>();

				foreach (var item in ItemsDataSource.GetAllItems())
				{
					items.Add(ItemViewModel.FromItem(item));
				}

				MasterListView.ItemsSource = items;
			}

			if (e.Parameter != null && e.Parameter.ToString() != string.Empty)
			{
				// Parameter is item ID
				var id = (int)e.Parameter;
				_lastSelectedItem = items.Where((item) => item.ItemId == id).FirstOrDefault();
			}

			UpdateForVisualState(AdaptiveStates.CurrentState);
		}

		private void AdaptiveStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
		{
			UpdateForVisualState(e.NewState, e.OldState);
		}

		private void UpdateForVisualState(VisualState newState, VisualState oldState = null)
		{
			if (newState == NarrowState && oldState == DefaultState && _lastSelectedItem != null)
			{
				// Resize down to the detail item. Don't play a transition.
				Frame.Navigate(typeof(DetailPage), _lastSelectedItem.ItemId, new SuppressNavigationTransitionInfo());
			}
			else if (newState == DefaultState && oldState == NarrowState)
			{
				_lastSelectedItem = null;
			}

			var isNarrow = newState == NarrowState;
			EntranceNavigationTransitionInfo.SetIsTargetElement(MasterListView, isNarrow);

			Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, UpdateTitleBar);
		}

		private void UpdateTitleBar()
		{
			if (AdaptiveStates.CurrentState != NarrowState)
			{
				this.titleBar.Margin = new Thickness(12, 6, 0, 0);
				System.Diagnostics.Debug.WriteLine("breit");
			}
			else
			{
				this.titleBar.Margin = new Thickness(50, 6, 0, 0);
				System.Diagnostics.Debug.WriteLine("schmal");
			}
		}

		private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
		{
			// Assure we are displaying the correct item. This is necessary in certain adaptive cases.
			MasterListView.SelectedItem = _lastSelectedItem;
		}

		private void NavMenuList_ItemInvoked(object sender, ListViewItem listViewItem)
		{
			var item = (NavMenuItem)((NavMenuListView)sender).ItemFromContainer(listViewItem);

			if (item != null)
			{
				titleBar.Text = item.Label;
			}
		}

		private void MasterListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var clickedItem = (ItemViewModel)e.ClickedItem;
			_lastSelectedItem = clickedItem;

			if (AdaptiveStates.CurrentState == NarrowState)
			{
				// Use "drill in" transition for navigating from master list to detail view
				Frame.Navigate(typeof(DetailPage), clickedItem.ItemId, new DrillInNavigationTransitionInfo());
			}
		}
	}
}
