using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
		public object Tag { get; set; }

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
		public object Tag { get; set; }
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











































	public abstract class MainViewModel : ViewModel
	{
		public ObservableCollection<MenuItemViewModel> MenuItems { get; set; }
		public MenuItemViewModel SelectedMenuItem
		{
			get { return _SelectedMenuItem; }
			set
			{
				_SelectedMenuItem = value; PropertyChange();
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
				_SelectedMasterItem = value; PropertyChange();
				OnSelectedMasterItem(_SelectedMasterItem);
			}
		}
		ItemViewModel _SelectedMasterItem;
		protected abstract void OnSelectedMasterItem(ItemViewModel o);


		public ObservableCollection<ActionViewModel> MasterCommands { get; set; }
		public ObservableCollection<ActionViewModel> DetailCommands { get; set; }
	}
}
