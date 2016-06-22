using JustObjectsPrototype.Universal.JOP.Editors;
using JustObjectsPrototype.Universal.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml.Controls;

namespace JustObjectsPrototype.Universal.JOP
{
	public class JopViewModel : MainViewModel
	{
		public readonly static Lazy<JopViewModel> Instance = new Lazy<JopViewModel>(() => new JopViewModel());

		private JopViewModel()
		{
			MenuItems = new ObservableCollection<MenuItemViewModel> { };
			MasterItems = new ObservableCollection<ItemViewModel> { };

			MasterCommands = new ObservableCollection<ActionViewModel>
			{
				new ActionViewModel
				{
					Action = new Command(()=>
					{
						//MasterItems.Add(new ItemViewModel
						//{
						//	Id = MasterItems.Max(i => i.Id) + 1,
						//	DateCreated = DateTime.Now,
						//	Text = "neu neu",
						//	Title = "item" + DateTime.Now.Ticks
						//});

						MasterItems.First().Title += "!";
						MasterItems.First().RaiseChanged("");
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
					Symbol = Symbol.AllApps,
					Tag = type
				};
				MenuItems.Add(menuItemVM);
			}
		}

		protected override void OnSelectedMenuItem(MenuItemViewModel o)
		{
			if (o != null)
			{
				UpdateItems(_Objects.OfType((Type)o.Tag));
				_Objects.OfType((Type)o.Tag).CollectionChanged += (s, e) => UpdateItems(_Objects.OfType((Type)o.Tag));
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
					DateCreated = DateTime.Now,
					Tag = item
				};
				UpdateViewModel(itemVM, item);
				item.PropertyChanged += (s, e) =>
				{
					UpdateViewModel(itemVM, item);
					itemVM.RaiseChanged(e.PropertyName);
				};
				MasterItems.Add(itemVM);
			}
		}

		private void UpdateViewModel(ItemViewModel itemVM, ObjectProxy item)
		{
			var selectedType = (Type)SelectedMenuItem.Tag;
			var properties = selectedType
				.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
				.Where(p => p.GetIndexParameters().Length == 0)
				.ToList();

			var firstString = properties.FirstOrDefault(p => p.PropertyType == typeof(string));

			itemVM.Title = firstString != null ? item.GetMember(firstString.Name)?.ToString() ?? string.Empty : item.ToString();
			itemVM.Text = string.Join("\n", properties.Except(new[] { firstString }).Select(p => ObjectDisplay.Nicely(p) + ": " + item.GetMember(p.Name)));
		}

		protected override void OnSelectedMasterItem(ItemViewModel o)
		{
			var selectedObject = o?.Tag as ObjectProxy;
			if (selectedObject != null)
			{
				var type = selectedObject.ProxiedObject.GetType();
				Properties = PropertiesViewModels.Of(type, _Objects, selectedObject, null);
			}
			else
			{
				Properties = new List<IPropertyViewModel>();
			}

			System.Diagnostics.Debug.WriteLine("Properties.Count: " + Properties.Count);
			Changed(() => Properties);
		}



		public List<IPropertyViewModel> Properties { get; set; }
	}
}
