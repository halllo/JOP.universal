using JustObjectsPrototype.Universal.JOP;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

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
			var binding = new Binding();
			binding.Source = ViewModel;
			binding.Path = new PropertyPath("Properties");
			binding.Mode = BindingMode.OneWay;
			BindingOperations.SetBinding(itemsControl, ItemsControl.ItemsSourceProperty, binding);

			progressBar.Visibility = Visibility.Collapsed;
		}

		public void Unprepare()
		{
			progressBar.Visibility = Visibility.Visible;
			itemsControl.ClearValue(ItemsControl.ItemsSourceProperty);
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
