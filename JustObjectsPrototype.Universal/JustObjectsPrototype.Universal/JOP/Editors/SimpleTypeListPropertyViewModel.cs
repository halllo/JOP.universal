using JustObjectsPrototype.Universal.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace JustObjectsPrototype.Universal.JOP.Editors
{
	public class SimpleTypeListPropertyViewModel : ViewModel, IPropertyViewModel
	{
		ObjectProxy _Instance;
		public ObjectProxy Instance
		{
			private get { return _Instance; }
			set
			{
				_Instance = value;
				_Instance.PropertyChanged += (s, e) =>
				{
					if (e.PropertyName == Property.Name || e.PropertyName == string.Empty)
					{
						Changed(() => Value);
					}
				};
			}
		}
		public PropertyInfo Property { private get; set; }
		public Action<ObjectChangedEventArgs> ObjectChanged { private get; set; }

		public bool CanWrite
		{
			get { return Property.CanWrite && Property.SetMethod.IsPublic; }
		}

		public string Label { get { return ObjectDisplay.Nicely(Property); } }

		public string Error { get; set; }

		public class SimpleTypeWrapper
		{
			public Action ValueChanged { get; set; }
			object _value;
			public object Value
			{
				get { return _value; }
				set
				{
					_value = value;
					if (ValueChanged != null) ValueChanged();
				}
			}
		}

		ObservableCollection<SimpleTypeWrapper> collection;
		Type collectionItemType;

		public Type ValueType { get { return Property.PropertyType; } }

		public object Value
		{
			get
			{
				if (collection == null)
				{
					collectionItemType = Property.PropertyType.GetTypeInfo().IsGenericType ? Property.PropertyType.GenericTypeArguments[0] : null;
					var values = (IEnumerable)Property.GetValue(Instance.ProxiedObject);
					var wrapper = Enumerable.Empty<SimpleTypeWrapper>();
					if (values != null)
					{
						wrapper = values.OfType<object>().Select(s => new SimpleTypeWrapper { Value = s, ValueChanged = Assign });
					}
					collection = new ObservableCollection<SimpleTypeWrapper>(wrapper);
					collection.CollectionChanged += collection_CollectionChanged;
				}
				return collection;
			}
		}

		void collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			Assign();
		}

		private void Assign()
		{
			Error = string.Empty;
			try
			{
				var canBeNull = !collectionItemType.GetTypeInfo().IsValueType || (Nullable.GetUnderlyingType(collectionItemType) != null);
				var listType = typeof(List<>);
				var constructedListType = listType.MakeGenericType(collectionItemType);
				var list = (IList)Activator.CreateInstance(constructedListType);
				foreach (var item in collection)
				{
					if (item.ValueChanged == null) item.ValueChanged = Assign;
					if (!canBeNull && item.Value == null) continue;
					list.Add(Convert.ChangeType(item.Value, collectionItemType ?? typeof(object)));
				}

				Property.SetValue(Instance.ProxiedObject, list);
				if (ObjectChanged != null) ObjectChanged(new ObjectChangedEventArgs { Object = Instance.ProxiedObject, PropertyName = Property.Name });
				Instance.RaisePropertyChanged(string.Empty);
			}
			catch (Exception ex)
			{
				Error = ex.Message;
			}
			Changed(() => Error);
		}

		public void Refresh()
		{
			collection = null;
			Changed(() => Value);
		}
	}
}
