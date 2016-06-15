using JustObjectsPrototype.Universal.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;

namespace JustObjectsPrototype.Universal.Views
{
	public abstract class ViewModel : INotifyPropertyChanged
	{
		protected void PropertyChange([CallerMemberName]string member = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(member));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}

	public class MainViewModel : ViewModel
	{
		public MainViewModel()
		{
			MenuItems = new List<NavMenuItem>(
			new[]
			{
				new NavMenuItem()
				{
					Symbol = Symbol.Contact,
					Label = "Page1",
				},
				new NavMenuItem()
				{
					Symbol = Symbol.Edit,
					Label = "Page2",
				},
				new NavMenuItem()
				{
					Symbol = Symbol.Favorite,
					Label = "Page3",
				},
				new NavMenuItem()
				{
					Symbol = Symbol.Mail,
					Label = "Master Detail",
				},
				new NavMenuItem()
				{
					Symbol = Symbol.Link,
					Label = "Download Source Code",
					Arguments = "http://scottge.net/product/uwp-windows-10-sample-navigation-panes",
				}
			});
		}

		public List<NavMenuItem> MenuItems { get; set; }

		public ItemViewModel SelectedObject
		{
			get { return _SelectedObject; }
			set { _SelectedObject = value; PropertyChange(); }
		}
		ItemViewModel _SelectedObject;
	}
}
