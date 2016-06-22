using JustObjectsPrototype.Universal.JOP;

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
	}
}
