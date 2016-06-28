using System;
using System.Collections.ObjectModel;
using Windows.Globalization.DateTimeFormatting;
using Windows.UI.Xaml.Controls;

namespace JustObjectsPrototype.Universal.Shell
{






















	public class MenuItemViewModel
	{
		public string Label { get; set; }
		public Symbol Symbol { get; set; }
		public char SymbolAsChar { get { return (char)this.Symbol; } }
		public object Tag { get; set; }

		public MenuItemViewModel()
		{
		}
	}

	public class ItemViewModel : ViewModel
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
				if (Date == null) return string.Empty;
				else
				{
					try
					{
						var formatter = new DateTimeFormatter("shortdate");
						return formatter.Format(Date.Value);
					}
					catch (Exception)
					{
						return "-";
					}
				}
			}
		}

		public string Title { get; set; }
		public string Text { get; set; }
		public object Tag { get; set; }
		public DateTime? Date { get; set; }

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











































	public abstract class MainViewModel : ViewModel
	{
		public bool PostFirstClick
		{
			get { return _PostFirstClick; }
			set { _PostFirstClick = value; Changed(); }
		}
		bool _PostFirstClick = false;

		public ObservableCollection<MenuItemViewModel> MenuItems { get; set; }
		public MenuItemViewModel SelectedMenuItem
		{
			get { return _SelectedMenuItem; }
			set
			{
				PostFirstClick = true;
				_SelectedMenuItem = value; Changed();
				OnSelectedMenuItem(_SelectedMenuItem);
			}
		}
		MenuItemViewModel _SelectedMenuItem;
		protected abstract void OnSelectedMenuItem(MenuItemViewModel o);


		public ObservableCollection<ItemViewModel> MasterItems { get; set; }
		public ItemViewModel SelectedMasterItem
		{
			get { return _SelectedMasterItem; }
			set
			{
				_SelectedMasterItem = value; Changed();
				OnSelectedMasterItem(_SelectedMasterItem);
			}
		}
		ItemViewModel _SelectedMasterItem;
		protected abstract void OnSelectedMasterItem(ItemViewModel o);


		public ObservableCollection<ActionViewModel> MasterCommands { get; set; }
		public ObservableCollection<ActionViewModel> DetailCommands { get; set; }
	}
}
