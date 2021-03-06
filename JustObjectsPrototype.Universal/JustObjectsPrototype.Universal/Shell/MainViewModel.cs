﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
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


		public IEnumerable<object> MasterItems { get; set; }
		public object SelectedMasterItem
		{
			get { return _SelectedMasterItem; }
			set
			{
				_SelectedMasterItem = value; Changed();
				OnSelectedMasterItem(_SelectedMasterItem);
			}
		}
		object _SelectedMasterItem;
		protected abstract void OnSelectedMasterItem(object o);


		public ObservableCollection<ActionViewModel> MasterCommands { get; set; }
		public ObservableCollection<ActionViewModel> DetailCommands { get; set; }
	}
}
