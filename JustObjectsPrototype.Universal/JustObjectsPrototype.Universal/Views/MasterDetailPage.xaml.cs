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

			DataContext = JopViewModel.Instance.Value;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (e.Parameter != null && e.Parameter.ToString() != string.Empty)
			{
				_lastSelectedItem = JopViewModel.Instance.Value.MasterItems.FirstOrDefault(mi => mi.Id == (int)e.Parameter);
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
				Frame.Navigate(typeof(DetailPage), _lastSelectedItem.Id, new SuppressNavigationTransitionInfo());
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
				this.titleBar.Margin = new Thickness(60, 6, 0, 0);
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
		}

		private void MasterListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var clickedItem = (ItemViewModel)e.ClickedItem;
			_lastSelectedItem = clickedItem;

			if (AdaptiveStates.CurrentState == NarrowState)
			{
				// Use "drill in" transition for navigating from master list to detail view
				Frame.Navigate(typeof(DetailPage), clickedItem.Id, new DrillInNavigationTransitionInfo());
			}
		}
	}
}
