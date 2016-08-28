using JustObjectsPrototype.Universal.JOP;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace JustObjectsPrototype.Universal.Shell
{
	public partial class MasterDetailPage : Page
	{
		private object _lastSelectedItem;

		public MasterDetailPage()
		{
			this.InitializeComponent();

			DataContext = JopViewModel.Instance.Value;

			detailView.Prepare();
		}

		internal VisualState CurrentState => AdaptiveStates.CurrentState;

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

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
				Frame.Navigate(typeof(DetailPage), 0, new SuppressNavigationTransitionInfo());
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

		private void NavMenuList_ItemInvoked(object sender, ListViewItem listViewItem)
		{
		}

		private void NavMenuListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_lastSelectedItem = null;
		}

		private void MasterListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			JopViewModel.Instance.Value.SelectedMasterItem = e.ClickedItem;
			GotoDetail(e.ClickedItem);
		}

		internal void GotoDetail(object clickedItem)
		{
			_lastSelectedItem = clickedItem;

			if (AdaptiveStates.CurrentState == NarrowState)
			{
				// Use "drill in" transition for navigating from master list to detail view
				Frame.Navigate(typeof(DetailPage), 0, new DrillInNavigationTransitionInfo());
			}
		}
	}
}
