using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JustObjectsPrototype.Universal.Views
{
	public static class CommandBarBridge
	{
		public static readonly DependencyProperty MasterCommandsProperty = DependencyProperty.RegisterAttached(
			"MasterCommands",
			typeof(object),
			typeof(CommandBar),
			new PropertyMetadata(null, new PropertyChangedCallback(MasterCommandsPropertyChanged))
		);

		private static void MasterCommandsPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var commandBar = o as CommandBar;
			var mainViewModel = e.NewValue as MainViewModel;

			mainViewModel.MasterCommands.CollectionChanged += (s, e2) =>
			{
				MasterCommandsPropertyUpdateCommands(commandBar, mainViewModel);
			};
			MasterCommandsPropertyUpdateCommands(commandBar, mainViewModel);
		}

		private static void MasterCommandsPropertyUpdateCommands(CommandBar commandBar, MainViewModel mainViewModel)
		{
			commandBar.PrimaryCommands.Clear();

			foreach (var command in mainViewModel.MasterCommands)
			{
				commandBar.PrimaryCommands.Add(new AppBarButton
				{
					Command = command.Action,
					Label = command.Label,
					Icon = new SymbolIcon(command.Icon)
				});
			};
		}

		public static void SetMasterCommandsProperty(UIElement element, object value)
		{
			element.SetValue(MasterCommandsProperty, value);
		}

		public static object GetMasterCommandsProperty(UIElement element)
		{
			return (object)element.GetValue(MasterCommandsProperty);
		}








		public static readonly DependencyProperty DetailCommandsProperty = DependencyProperty.RegisterAttached(
			"DetailCommands",
			typeof(object),
			typeof(CommandBar),
			new PropertyMetadata(null, new PropertyChangedCallback(DetailCommandsPropertyChanged))
		);

		private static void DetailCommandsPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var commandBar = o as CommandBar;
			var mainViewModel = MainViewModel.Instance.Value;

			mainViewModel.DetailCommands.CollectionChanged += (s, e2) =>
			{
				DetailCommandsPropertyUpdateCommands(commandBar, mainViewModel);
			};
			DetailCommandsPropertyUpdateCommands(commandBar, mainViewModel);
		}

		private static void DetailCommandsPropertyUpdateCommands(CommandBar commandBar, MainViewModel mainViewModel)
		{
			commandBar.PrimaryCommands.Clear();

			foreach (var command in mainViewModel.DetailCommands)
			{
				commandBar.PrimaryCommands.Add(new AppBarButton
				{
					Command = command.Action,
					Label = command.Label,
					Icon = new SymbolIcon(command.Icon)
				});
			};
		}

		public static void SetDetailCommandsProperty(UIElement element, object value)
		{
			element.SetValue(DetailCommandsProperty, value);
		}

		public static object GetDetailCommandsProperty(UIElement element)
		{
			return (object)element.GetValue(DetailCommandsProperty);
		}
	}
}
