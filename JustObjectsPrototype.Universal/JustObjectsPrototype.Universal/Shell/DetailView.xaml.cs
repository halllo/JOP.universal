using JustObjectsPrototype.Universal.JOP;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JustObjectsPrototype.Universal.Shell
{
	public sealed partial class DetailView
	{
		public DetailView()
		{
			this.InitializeComponent();
		}

		public void Prepare()
		{
			itemsControl.Visibility = Visibility.Visible;
		}

		public void Unprepare()
		{
			itemsControl.Visibility = Visibility.Collapsed;
		}

		public bool CommandBarVisible
		{
			get { return commandBar.Visibility == Visibility.Visible; }
			set { commandBar.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
		}

		public JopViewModel ViewModel { get { return JopViewModel.Instance.Value; } }

		private void OpenAgain(object sender, object e)
		{
			(sender as CommandBar).IsOpen = true;
		}
	}
}
