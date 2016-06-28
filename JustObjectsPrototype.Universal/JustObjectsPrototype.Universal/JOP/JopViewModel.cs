using JustObjectsPrototype.Universal.JOP.Editors;
using JustObjectsPrototype.Universal.Shell;
using System;
using System.Collections;
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
			MasterCommands = new ObservableCollection<ActionViewModel> { };
			DetailCommands = new ObservableCollection<ActionViewModel> { };
		}



		public List<IPropertyViewModel> Properties { get; set; }



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
					Symbol = type.GetTypeInfo().GetCustomAttribute<IconAttribute>()?.Icon ?? Symbol.Placeholder,
					Tag = type
				};
				MenuItems.Add(menuItemVM);
			}
		}

		private Type SelectedType { get; set; }
		protected override void OnSelectedMenuItem(MenuItemViewModel o)
		{
			if (o != null)
			{
				SelectedType = (Type)o.Tag;

				UpdateItems(_Objects.OfType(SelectedType));
				_Objects.OfType(SelectedType).CollectionChanged += (s, e) => UpdateItems(_Objects.OfType((Type)o.Tag));


				var staticMethods = SelectedType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
				var staticFunctions = GetFunctions(null, staticMethods);

				MasterCommands.Clear();
				foreach (var function in staticFunctions)
				{
					MasterCommands.Add(new ActionViewModel
					{
						Label = function.Item1,
						Icon = function.Item2,
						Action = function.Item3,
					});
				}
			}
		}

		private void UpdateItems(ObservableCollection<ObjectProxy> items)
		{
			foreach (var item in MasterItems)
			{
				(item.Tag as ObjectProxy).RemovePropertyChangedCallbacks();
			}
			MasterItems.Clear();

			foreach (var item in items)
			{
				var itemVM = new ItemViewModel
				{
					Id = ++ItemViewModel.IdZaehler,
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
			var properties = SelectedType
				.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
				.Where(p => p.GetIndexParameters().Length == 0)
				.ToList();

			var firstString = properties.FirstOrDefault(p => p.PropertyType == typeof(string));
			var secondString = properties.Except(new[] { firstString }).FirstOrDefault(p => p.PropertyType == typeof(string));
			var firstDateTime = properties.FirstOrDefault(p => p.PropertyType == typeof(DateTime));

			itemVM.Title = firstString != null ? item.GetMember(firstString.Name) as string : item.ProxiedObject.ToString();
			itemVM.Text = secondString != null ? item.GetMember(secondString.Name) as string : null;
			itemVM.Date = firstDateTime != null ? item.GetMember(firstDateTime.Name) as DateTime? : null;
		}

		protected override void OnSelectedMasterItem(ItemViewModel o)
		{
			var selectedObject = o?.Tag as ObjectProxy;
			if (selectedObject != null)
			{
				var type = selectedObject.ProxiedObject.GetType();
				Properties = PropertiesViewModels.Of(PropertyValueStore.ForPropertiesOf(type, selectedObject, null), _Objects);

				var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
				var functions = GetFunctions(selectedObject, methods);

				DetailCommands.Clear();
				foreach (var function in functions)
				{
					DetailCommands.Add(new ActionViewModel
					{
						Label = function.Item1,
						Icon = function.Item2,
						Action = function.Item3
					});
				}
			}
			else
			{
				Properties = new List<IPropertyViewModel>();
				DetailCommands.Clear();
			}

			System.Diagnostics.Debug.WriteLine("Properties.Count: " + Properties.Count);
			Changed(() => Properties);
		}




























		public Action<List<IValueStore>> ShowMethodInvocationDialog { get; set; }
		public string MethodInvocationTitle { get; set; }
		public List<IPropertyViewModel> MethodInvocationParameters { get; set; }
		public Command MethodInvocationContinuation { get; set; }


		IEnumerable<Tuple<string, Symbol, Command>> GetFunctions(ObjectProxy instance, MethodInfo[] methods)
		{
			return from m in methods
				   where m.DeclaringType != typeof(object)
				   where m.IsSpecialName == false
				   where m.Name != "ToString"
				   select Tuple.Create(ObjectDisplay.Nicely(m), m.GetCustomAttribute<IconAttribute>()?.Icon ?? Symbol.Placeholder, new Command(() =>
				   {
					   var parameters = m.GetParameters();
					   var parameterValueStores = parameters
						.Select(p => new SelfcontainedValueStore
						{
							Identifier = ObjectDisplay.Nicely(p),
							Value = null,
							ValueType = p.ParameterType,
						}).ToList<IValueStore>();

					   var parameterValueStoresWithoutObservableCollections = parameterValueStores
						.Where(vs => IsObservableCollection(vs.ValueType) == false)
						.ToList();

					   var propertiesViewModels = PropertiesViewModels.Of(parameterValueStoresWithoutObservableCollections, _Objects);

					   if (parameterValueStoresWithoutObservableCollections.Any())
					   {
						   MethodInvocationTitle = ObjectDisplay.Nicely(m);
						   MethodInvocationParameters = propertiesViewModels;
						   MethodInvocationContinuation = new Command(() => InvokeMethod(instance, m, parameterValueStores));
						   ShowMethodInvocationDialog(parameterValueStoresWithoutObservableCollections);
					   }
					   else
					   {
						   InvokeMethod(instance, m, parameterValueStores);
					   }
				   }));
		}

		private void InvokeMethod(ObjectProxy instance, MethodInfo method, List<IValueStore> parameterValueStores)
		{
			var parameterInstances = parameterValueStores
									.Select(vs => IsObservableCollection(vs.ValueType)
									   ? _Objects.OfType_OneWayToSourceChangePropagation(vs.ValueType.GetGenericArguments().First())
									   : vs.Value)
									.ToList();

			object result = null;
			try
			{
				result = method.Invoke(instance != null ? instance.ProxiedObject : null, parameterInstances.ToArray());
			}
			catch (TargetInvocationException tie)
			{
				System.Diagnostics.Debug.WriteLine("An Exception occured in " + method.Name + ".\n\n" + tie.InnerException.ToString());
				//MessageBox.Show("An Exception occured in " + m.Name + ".\n\n" + tie.InnerException.ToString(), "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}


			var objectsToRefreshCandidates = parameterInstances.SelectMany(i =>
			{
				if (i is IEnumerable)
				{
					return ((IEnumerable)i).OfType<object>();
				}
				else
				{
					return new[] { i }.OfType<object>();
				}
			});
			var ofTypeMethod = typeof(Enumerable).GetMethod("OfType").MakeGenericMethod(SelectedType);
			var objectsToRefresh = (IEnumerable<object>)ofTypeMethod.Invoke(null, new[] { objectsToRefreshCandidates });
			foreach (var objectToRefresh in objectsToRefresh)
			{
				var proxy = _Objects.GetProxy(objectToRefresh);
				if (proxy != null) proxy.RaisePropertyChanged(string.Empty);
			}


			if (result != null)
			{
				var resultType = result.GetType();

				if (resultType.GetTypeInfo().IsGenericType
					&& (resultType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
						||
						resultType.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IEnumerable)))
					&& resultType.GetGenericArguments().Any()
					&& resultType.GetGenericArguments().First().GetTypeInfo().IsValueType == false
					&& IsMicrosoftType(resultType.GetGenericArguments().First()) == false)
				{
					var resultItemType = resultType.GetGenericArguments().First();
					var objectsOfType = _Objects.OfType(resultItemType);
					foreach (var resultItem in (IEnumerable)result)
					{
						if (resultItem != null && objectsOfType.All(o => !o.ProxiedObject.Equals(resultItem)))
						{
							objectsOfType.Add(new ObjectProxy(resultItem));
						}
					}
				}
				if (resultType.GetTypeInfo().IsValueType == false && IsMicrosoftType(resultType) == false)
				{
					var objectsOfType = _Objects.OfType(resultType);
					if (objectsOfType.All(o => !o.ProxiedObject.Equals(result)))
					{
						objectsOfType.Add(new ObjectProxy(result));
					}
				}
			}
			if (instance != null) instance.RaisePropertyChanged(string.Empty);
			if (Properties != null) Properties.ForEach(p => p.Refresh());
		}

		static bool IsObservableCollection(Type type)
		{
			return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(ObservableCollection<>);
		}

		static bool IsMicrosoftType(Type type)
		{
			var attrs = type.GetTypeInfo().Assembly.GetCustomAttributes<AssemblyCompanyAttribute>();
			return attrs.OfType<AssemblyCompanyAttribute>().Any(attr => attr.Company == "Microsoft Corporation");
		}
	}
}
