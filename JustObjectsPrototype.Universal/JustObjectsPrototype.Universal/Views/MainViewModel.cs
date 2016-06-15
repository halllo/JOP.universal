using JustObjectsPrototype.Universal.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Globalization.DateTimeFormatting;
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




	public class ItemViewModel
	{
		public int Id
		{
			get; set;
		}

		public string DateCreatedHourMinute
		{
			get
			{
				var formatter = new DateTimeFormatter("hour minute");
				return formatter.Format(DateCreated);
			}
		}

		public string Title { get; set; }
		public string Text { get; set; }
		public DateTime DateCreated { get; set; }

		public ItemViewModel()
		{
		}
	}





	public class MainViewModel : ViewModel
	{
		public readonly static Lazy<MainViewModel> Instance = new Lazy<MainViewModel>(() => new MainViewModel());

		private MainViewModel()
		{
			MenuItems = new List<NavMenuItem>
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
			};

			MasterItems = new List<ItemViewModel>
			{
				new ItemViewModel()
				{
					Id = 0,
					DateCreated = DateTime.Now,
					Title = "Item 1",
					Text = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer id facilisis lectus. Cras nec convallis ante, quis pulvinar tellus. Integer dictum accumsan pulvinar. Pellentesque eget enim sodales sapien vestibulum consequat.

Maecenas eu sapien ac urna aliquam dictum.

Nullam eget mattis metus. Donec pharetra, tellus in mattis tincidunt, magna ipsum gravida nibh, vitae lobortis ante odio vel quam."
				},
				new ItemViewModel()
				{
					Id = 1,
					DateCreated = DateTime.Now,
					Title = "Item 2",
					Text = @"Quisque accumsan pretium ligula in faucibus. Mauris sollicitudin augue vitae lorem cursus condimentum quis ac mauris. Pellentesque quis turpis non nunc pretium sagittis. Nulla facilisi. Maecenas eu lectus ante. Proin eleifend vel lectus non tincidunt. Fusce condimentum luctus nisi, in elementum ante tincidunt nec.

Aenean in nisl at elit venenatis blandit ut vitae lectus. Praesent in sollicitudin nunc. Pellentesque justo augue, pretium at sem lacinia, scelerisque semper erat. Ut cursus tortor at metus lacinia dapibus."
				},
				new ItemViewModel()
				{
					Id = 2,
					DateCreated = DateTime.Now,
					Title = "Item 3",
					Text = @"Ut consequat magna luctus justo egestas vehicula. Integer pharetra risus libero, et posuere justo mattis et.

Proin malesuada, libero vitae aliquam venenatis, diam est faucibus felis, vitae efficitur erat nunc non mauris. Suspendisse at sodales erat.
Aenean vulputate, turpis non tincidunt ornare, metus est sagittis erat, id lobortis orci odio eget quam. Suspendisse ex purus, lobortis quis suscipit a, volutpat vitae turpis."
				},
				new ItemViewModel()
				{
					Id = 3,
					DateCreated = DateTime.Now,
					Title = "Item 4",
					Text = @"Duis facilisis, quam ut laoreet commodo, elit ex aliquet massa, non varius tellus lectus et nunc. Donec vitae risus ut ante pretium semper. Phasellus consectetur volutpat orci, eu dapibus turpis. Fusce varius sapien eu mattis pharetra.

Nam vulputate eu erat ornare blandit. Proin eget lacinia erat. Praesent nisl lectus, pretium eget leo et, dapibus dapibus velit. Integer at bibendum mi, et fringilla sem."
				}
			};
		}

		public List<NavMenuItem> MenuItems { get; set; }


		public List<ItemViewModel> MasterItems { get; set; }


		public ItemViewModel SelectedMasterItem
		{
			get { return _SelectedMasterItem; }
			set { _SelectedMasterItem = value; PropertyChange(); }
		}
		ItemViewModel _SelectedMasterItem;
	}
}
