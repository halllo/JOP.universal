﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JustObjectsPrototype.Universal.JOP.Editors;
using JustObjectsPrototype.Universal.Shell;
using Windows.UI.Core;
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
		Dictionary<Type, Action<ObjectChangedEventArgs>> _ChangedEvents { get; set; }

		public void Init(ICollection<object> objects, List<Type> types = null, Dictionary<Type, Action<ObjectChangedEventArgs>> changedEvents = null)
		{
			_Objects = new Objects(objects);
			_ChangedEvents = changedEvents;

			if (types != null && types.Count > 0)
			{
				UpdateMenu(new ObservableCollection<Type>(types));
			}
			else
			{
				UpdateMenu(_Objects.Types);
				_Objects.Types.CollectionChanged += (s, e) => UpdateMenu(_Objects.Types);
			}
		}



		private void UpdateMenu(ObservableCollection<Type> types)
		{
			MenuItems.Clear();
			foreach (var type in types)
			{
				var menuItemVM = new MenuItemViewModel
				{
					Label = type.GetTypeInfo().GetCustomAttribute<TitleAttribute>()?.Title ?? ObjectDisplay.Nicely(type),
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
				var previousType = SelectedType;
				var menuItemType = SelectedType = (Type)o.Tag;

				UpdateItems(menuItemType, previousType, _Objects.OfType(menuItemType), null);
				_Objects.OfType(menuItemType).CollectionChanged += (s, e) => UpdateItems(menuItemType, null, _Objects.OfType(menuItemType), e);


				var staticMethods = menuItemType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
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

		bool _UpdatingItemsEnabled = true;
		public event Action<NotifyCollectionChangedEventArgs> DirectItemsChanged;
		private void UpdateItems(Type type, Type previousType, ObservableCollection<ObjectProxy> items, NotifyCollectionChangedEventArgs collectionChangedEventArgs)
		{
			if (!SelectedType.Equals(type)) return;
			if (collectionChangedEventArgs != null) DirectItemsChanged?.Invoke(collectionChangedEventArgs);
			if (!_UpdatingItemsEnabled) return;

			foreach (var item in MasterItems)
			{
				(item.Tag as ObjectProxy).RemovePropertyChangedCallbacks();
			}
			var selectedId = previousType != null ? null : SelectedMasterItem?.Id;
			MasterItems.Clear();
			int idZaehler = -1;
			foreach (var item in items)
			{
				var itemVM = new ItemViewModel
				{
					Id = ++idZaehler,
					Tag = item
				};
				UpdateViewModel(type, itemVM, item);
				item.PropertyChanged += (s, e) =>
				{
					UpdateViewModel(type, itemVM, item);
					itemVM.RaiseChanged(e.PropertyName);
				};
				MasterItems.Add(itemVM);
			}

			if (selectedId.HasValue && selectedId.Value < MasterItems.Count)
			{
				SelectedMasterItem = MasterItems[selectedId.Value];
			}
			else
			{
				SelectedMasterItem = null;
			}
		}

		private void UpdateViewModel(Type type, ItemViewModel itemVM, ObjectProxy item)
		{
			var properties = type
				.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
				.Where(p => p.GetIndexParameters().Length == 0)
				.ToList();

			var toStrings = type.GetMethods().Where(m => m.Name == "ToString" && m.DeclaringType == type && m.GetParameters().Length == 0);
			if (toStrings.Any())
			{
				var toStringResult = toStrings.First().Invoke(item.ProxiedObject, new object[0]);
				itemVM.Title = toStringResult != null ? toStringResult.ToString() : string.Empty;
			}
			else
			{
				var firstString = properties.FirstOrDefault(p => p.PropertyType == typeof(string));
				var secondString = properties.Except(new[] { firstString }).FirstOrDefault(p => p.PropertyType == typeof(string));
				itemVM.Title = firstString != null ? FirstCharacters(100, item.GetMember(firstString.Name) as string) : item.ProxiedObject.ToString();
				itemVM.Text = secondString != null ? FirstCharacters(100, item.GetMember(secondString.Name) as string) : null;
			}

			var firstDateTime = properties.FirstOrDefault(p => p.PropertyType == typeof(DateTime));
			itemVM.Date = firstDateTime != null ? item.GetMember(firstDateTime.Name) as DateTime? : null;
		}

		private string FirstCharacters(int count, string text)
		{
			return string.IsNullOrEmpty(text) ? text : new string(text.Take(count).ToArray());
		}

		protected override void OnSelectedMasterItem(ItemViewModel o)
		{
			var selectedObject = o?.Tag as ObjectProxy;
			if (selectedObject != null)
			{
				var type = selectedObject.ProxiedObject.GetType();
				Properties = PropertiesViewModels.Of(PropertyValueStore.ForPropertiesOf(type, selectedObject, InvokeChangedEvents), _Objects);

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
				   select Tuple.Create(
					   m.GetCustomAttribute<TitleAttribute>()?.Title ?? ObjectDisplay.Nicely(m),
					   m.GetCustomAttribute<IconAttribute>()?.Icon ?? Symbol.Placeholder,
					   new Command(async () =>
				   {
					   var parameters = m.GetParameters();
					   var parameterValueStores = parameters
						.Select(p => new SelfcontainedValueStore
						{
							Identifier = ObjectDisplay.Nicely(p),
							Value = p.DefaultValue,
							ValueType = p.ParameterType,
							CustomView = p.GetCustomAttribute<CustomViewAttribute>()?.ResourceKey,
						}).ToList<IValueStore>();

					   var parameterValueStoresWithoutObservableCollections = parameterValueStores
						.Where(vs => IsObservableCollection(vs.ValueType) == false)
						.ToList();

					   var propertiesViewModels = PropertiesViewModels.Of(parameterValueStoresWithoutObservableCollections, _Objects);

					   if (parameterValueStoresWithoutObservableCollections.Any())
					   {
						   MethodInvocationTitle = ObjectDisplay.Nicely(m);
						   MethodInvocationParameters = propertiesViewModels;
						   MethodInvocationContinuation = new Command(async () => await InvokeMethod(instance, m, parameterValueStores));
						   ShowMethodInvocationDialog(parameterValueStoresWithoutObservableCollections);
					   }
					   else
					   {
						   await InvokeMethod(instance, m, parameterValueStores);
					   }
				   }));
		}

		private async Task InvokeMethod(ObjectProxy instance, MethodInfo method, List<IValueStore> parameterValueStores)
		{
			var parameterInstances = parameterValueStores
									.Select(vs => IsObservableCollection(vs.ValueType)
									   ? _Objects.OfType_OneWayToSourceChangePropagation(vs.ValueType.GetGenericArguments().First())
									   : Convert.ChangeType(vs.Value, vs.ValueType))
									.ToList();

			object result = null;

			try
			{
				_UpdatingItemsEnabled = false;
				var methodResult = method.Invoke(instance != null ? instance.ProxiedObject : null, parameterInstances.ToArray());
				if (methodResult is Task)
				{
					var methodResultTask = (Task)methodResult;
					await methodResultTask;
					var methodResultTaskType = methodResultTask.GetType();
					if (methodResultTaskType.GetTypeInfo().GenericTypeArguments[0].FullName != "System.Threading.Tasks.VoidTaskResult")
					{
						var resultProperty = methodResultTaskType.GetProperty("Result");
						result = resultProperty.GetValue(methodResultTask);
					}
				}
				else
				{
					result = methodResult;
				}
			}
			catch (TargetInvocationException tie)
			{
				System.Diagnostics.Debug.WriteLine("An Exception occured in " + method.Name + ".\n\n" + tie.InnerException.ToString());
				//MessageBox.Show("An Exception occured in " + m.Name + ".\n\n" + tie.InnerException.ToString(), "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			finally
			{
				_UpdatingItemsEnabled = true;
			}


			var objectsToRefreshCandidates = parameterInstances.SelectMany(i =>
			{
				if (i != null && IsObservableCollection(i.GetType()))
				{
					return new object[0];
				}
				else if (i is IEnumerable)
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

			var typesToRefreshCandidates = from parameterInstance in parameterInstances
										   where parameterInstance != null
										   let parameterInstanceType = parameterInstance.GetType()
										   where IsObservableCollection(parameterInstanceType)
										   select parameterInstanceType.GetTypeInfo().GenericTypeArguments.First();
			var typeToRefresh = typesToRefreshCandidates.FirstOrDefault(t => t.Equals(SelectedType));
			if (typeToRefresh != null)
			{
				UpdateItems(typeToRefresh, null, _Objects.OfType(typeToRefresh), null);
			}


			if (result != null)
			{
				var jumpToResult = method.GetCustomAttribute<JumpToResultAttribute>() != null;
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
					if (jumpToResult) JumpToResult(resultItemType);
				}
				if (resultType.GetTypeInfo().IsValueType == false && IsMicrosoftType(resultType) == false)
				{
					var objectsOfType = _Objects.OfType(resultType);
					if (objectsOfType.All(o => !o.ProxiedObject.Equals(result)))
					{
						objectsOfType.Add(new ObjectProxy(result));
					}
					if (jumpToResult) JumpToResult(resultType, result);
				}
			}
			if (instance != null) instance.RaisePropertyChanged(string.Empty);
			if (Properties != null) Properties.ForEach(p => p.Refresh());
		}

		void JumpToResult(Type resultType, object result = null)
		{
			Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				SelectedMenuItem = MenuItems.First(mi => mi.Tag == resultType);
				if (result != null)
				{
					var resultProxy = _Objects.GetProxy(result);
					SelectedMasterItem = MasterItems.First(mi => mi.Tag == resultProxy);

					var detailPage = (((Frame)Windows.UI.Xaml.Window.Current.Content)).Content as DetailPage;
					if (detailPage != null)
					{
						detailPage.Item = SelectedMasterItem;
						//KNOWN BUG: resizing into narrow state doesn't go to DetailPage until it is selected again.
					}
					else
					{
						var masterDetailPage = (((Frame)Windows.UI.Xaml.Window.Current.Content)).Content as MasterDetailPage;
						if (masterDetailPage != null && masterDetailPage.CurrentState.Name == "NarrowState")
						{
							masterDetailPage.GotoDetail(SelectedMasterItem);
						}
					}
				}
				else
				{
					if ((((Frame)Windows.UI.Xaml.Window.Current.Content)).Content is DetailPage)
					{
						((Frame)Windows.UI.Xaml.Window.Current.Content).GoBack();
					}
				}
			});
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
























		void InvokeChangedEvents(ObjectChangedEventArgs obj)
		{
			var type = obj.Object.GetType();
			if (_ChangedEvents != null && _ChangedEvents.ContainsKey(type))
			{
				try
				{
					_ChangedEvents[type].Invoke(obj);
				}
				catch (Exception e)
				{
				}
			}
		}
	}
}
