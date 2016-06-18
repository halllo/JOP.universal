using JustObjectsPrototype.Universal.JOP;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
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

	public class Command : ICommand
	{
		Action _Execute;
		Func<bool> _CanExecute;

		public Command(Action execute, Func<bool> canExecute = null)
		{
			_Execute = execute;
			_CanExecute = canExecute ?? (() => true);
		}

		public bool CanExecute(object parameter)
		{
			return _CanExecute();
		}

		public void Execute(object parameter)
		{
			_Execute();
		}

		public event EventHandler CanExecuteChanged;
		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, new EventArgs());
		}
	}




	public class MenuItemViewModel
	{
		public string Label { get; set; }
		public Symbol Symbol { get; set; }
		public char SymbolAsChar { get { return (char)this.Symbol; } }

		public MenuItemViewModel()
		{
		}
	}

	public class ItemViewModel
	{
		public static int IdZaehler = 0;

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

	public class ActionViewModel
	{
		public Command Action { get; set; }
		public string Label { get; set; }
		public Symbol Icon { get; set; }
	}
























































	public class MainViewModel : ViewModel
	{
		public readonly static Lazy<MainViewModel> Instance = new Lazy<MainViewModel>(() => new MainViewModel());

		private MainViewModel()
		{
			MenuItems = new ObservableCollection<MenuItemViewModel>
			{
				new MenuItemViewModel()
				{
					Label = "Items",
					Symbol = Symbol.Contact,
				},
				new MenuItemViewModel()
				{
					Label = "Other Items",
					Symbol = Symbol.Edit,
				},
				new MenuItemViewModel()
				{
					Label = "More Items",
					Symbol = Symbol.Favorite,
				},
				new MenuItemViewModel()
				{
					Label = "Stuff",
					Symbol = Symbol.Mail,
				},
			};
			MasterItems = new ObservableCollection<ItemViewModel>
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

			MasterCommands = new ObservableCollection<ActionViewModel>
			{
				new ActionViewModel
				{
					Action = new Command(()=>
					{
						MasterItems.Add(new ItemViewModel
						{
							Id = MasterItems.Max(i => i.Id) + 1,
							DateCreated = DateTime.Now,
							Text = "neu neu",
							Title = "item" + DateTime.Now.Ticks
						});
					}),
					Label = "new item",
					Icon = Symbol.NewWindow
				},
			};

			DetailCommands = new ObservableCollection<ActionViewModel>
			{
				new ActionViewModel
				{
					Action = new Command(()=>
					{
						System.Diagnostics.Debug.WriteLine("d1");
						DetailCommands.Add(new ActionViewModel
						{
							Label = "function" + DateTime.Now.Ticks,
							Icon = Symbol.Placeholder,
							Action = new Command(() => { })
						});
					}),
					Label = "new function",
					Icon = Symbol.NewFolder
				},
			};
		}




		Objects _Objects;

		public void Init(ObservableCollection<object> objects)
		{
			_Objects = new Objects(objects);


			UpdateMenu(_Objects.Types);
			_Objects.Types.CollectionChanged += (s, e) => UpdateMenu(_Objects.Types);
		}


		private void UpdateMenu(ObservableCollection<Type> types)
		{
			MenuItems.Clear();
			foreach (var type in types)
			{
				var menuItemVM = new MenuItemViewModel
				{
					Label = type.Name,
					Symbol = Symbol.AllApps
				};
				MenuItems.Add(menuItemVM);
			}
		}

		private void UpdateItems(ObservableCollection<ObjectProxy> items)
		{
			MasterItems.Clear();
			foreach (var item in items)
			{
				var itemVM = new ItemViewModel
				{
					Id = ++ItemViewModel.IdZaehler,
					Title = item.ToString(),
					Text = "...",
					DateCreated = DateTime.Now
				};
				MasterItems.Add(itemVM);
			}
		}







		public ObservableCollection<MenuItemViewModel> MenuItems { get; set; }

		public MenuItemViewModel SelectedMenuItem
		{
			get { return _SelectedMenuItem; }
			set
			{
				_SelectedMenuItem = value; PropertyChange();

				if (_SelectedMenuItem != null)
				{
					var selectedType = _Objects.Types.First(t => t.Name == SelectedMenuItem.Label);
					UpdateItems(_Objects.OfType(selectedType));
					_Objects.OfType(selectedType).CollectionChanged += (s, e) => UpdateItems(_Objects.OfType(selectedType));
				}
			}
		}
		MenuItemViewModel _SelectedMenuItem;


		public ObservableCollection<ItemViewModel> MasterItems { get; set; }

		public ItemViewModel SelectedMasterItem
		{
			get { return _SelectedMasterItem; }
			set { _SelectedMasterItem = value; PropertyChange(); }
		}
		ItemViewModel _SelectedMasterItem;


		public ObservableCollection<ActionViewModel> MasterCommands { get; set; }
		public ObservableCollection<ActionViewModel> DetailCommands { get; set; }
	}
}
