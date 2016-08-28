using JustObjectsPrototype.Universal.JOP.Editors;
using JustObjectsPrototype.Universal.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
			MasterItems = new ObservableCollection<object> { };
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

		public ObservableCollection<ObjectProxy> ObjectsOfType<T>()
		{
			return _Objects.OfType(typeof(T));
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
		private NotifyCollectionChangedEventHandler MasterItemsChangedEventHandler { get; set; }
		protected override void OnSelectedMenuItem(MenuItemViewModel o)
		{
			if (o != null)
			{
				if (SelectedType != null)
				{
					var objectsOfPreviousType = _Objects.OfType(SelectedType);
					objectsOfPreviousType.CollectionChanged -= MasterItemsChangedEventHandler;
				}

				var menuItemType = SelectedType = (Type)o.Tag;
				var objectsOfType = _Objects.OfType(menuItemType);
				MasterItemsChangedEventHandler = new NotifyCollectionChangedEventHandler((s, e) => DirectItemsChanged?.Invoke(e));
				objectsOfType.CollectionChanged += MasterItemsChangedEventHandler;
				MasterItems = objectsOfType;
				Changed("MasterItems");

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

		public event Action<NotifyCollectionChangedEventArgs> DirectItemsChanged;



		protected override void OnSelectedMasterItem(object o)
		{
			var selectedObject = o as ObjectProxy;
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
					   var methodName = m.GetCustomAttribute<TitleAttribute>()?.Title ?? ObjectDisplay.Nicely(m);
					   var requireConfirmation = m.GetCustomAttribute<RequiresConfirmationAttribute>() != null;
					   if (requireConfirmation)
					   {
						   await Show.Message(methodName + "?", onYes: async () =>
						   {
							   await ExecuteFunction(instance, m, methodName);
						   });
					   }
					   else
					   {
						   await ExecuteFunction(instance, m, methodName);
					   }
				   }));
		}

		private async Task ExecuteFunction(ObjectProxy instance, MethodInfo method, string methodName)
		{
			var parameters = method.GetParameters();
			var parameterValueStores = parameters
			 .Select(p => new SelfcontainedValueStore
			 {
				 Identifier = p.GetCustomAttribute<TitleAttribute>()?.Title ?? ObjectDisplay.Nicely(p),
				 Value = ((p.Attributes & ParameterAttributes.HasDefault) == ParameterAttributes.HasDefault) ? p.DefaultValue : null,
				 ValueType = p.ParameterType,
				 CustomView = p.GetCustomAttribute<CustomViewAttribute>()?.ResourceKey,
			 }).ToList<IValueStore>();

			var parameterValueStoresWithoutObservableCollections = parameterValueStores
			 .Where(vs => IsObservableCollection(vs.ValueType) == false)
			 .ToList();

			var propertiesViewModels = PropertiesViewModels.Of(parameterValueStoresWithoutObservableCollections, _Objects);

			if (parameterValueStoresWithoutObservableCollections.Any())
			{
				MethodInvocationTitle = methodName;
				MethodInvocationParameters = propertiesViewModels;
				MethodInvocationContinuation = new Command(async () => await InvokeMethod(instance, method, parameterValueStores));
				ShowMethodInvocationDialog(parameterValueStoresWithoutObservableCollections);
			}
			else
			{
				await InvokeMethod(instance, method, parameterValueStores);
			}
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


			var objectsOfSelectedType = _Objects.OfType(SelectedType);
			foreach (var objectToRefresh in objectsOfSelectedType)
			{
				objectToRefresh.RaisePropertyChanged(string.Empty);
			}


			if (result != null)
			{
				var jumpToResult = method.GetCustomAttribute<JumpsToResultAttribute>() != null;
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
				else if (resultType.GetTypeInfo().IsValueType == false && IsMicrosoftType(resultType) == false)
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
				SelectedMenuItem = MenuItems.FirstOrDefault(mi => mi.Tag == resultType);
				if (SelectedMenuItem != null)
				{
					if (result != null)
					{
						var resultProxy = _Objects.GetProxy(result);
						SelectedMasterItem = MasterItems.First(mi => mi == resultProxy);

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
				catch (Exception)
				{
				}
			}
		}
	}
}
