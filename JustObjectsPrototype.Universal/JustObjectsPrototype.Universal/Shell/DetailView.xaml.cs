using JustObjectsPrototype.Universal.JOP;
using Windows.UI.Xaml.Controls;

namespace JustObjectsPrototype.Universal.Shell
{
	public sealed partial class DetailView
	{
		public DetailView()
		{
			this.InitializeComponent();
		}

		public bool CommandBarVisible
		{
			get { return commandBar.Visibility == Windows.UI.Xaml.Visibility.Visible; }
			set { commandBar.Visibility = value ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed; }
		}

		public JopViewModel JopViewModel { get { return JopViewModel.Instance.Value; } }

		private void OpenAgain(object sender, object e)
		{
			(sender as CommandBar).IsOpen = true;
		}
	}
}
