using JustObjectsPrototype.Universal.Shell;
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
		IValueStore _ValueStore;
		public IValueStore ValueStore
		{
			get { return _ValueStore; }
			set
			{
				_ValueStore = value;
				_ValueStore.PropertyChanged += (s, e) => Changed(() => Value);
			}
		}

		public bool CanWrite { get { return ValueStore.CanWrite; } }

		public string Label { get { return ValueStore.Identifier; } }

		public string Error { get; set; }

		Command addReference;
		public Command AddEntry
		{
			get
			{
				if (addReference == null)
				{
					addReference = new Command(() => collection.Add(new SimpleTypeWrapper
					{
						Value = null,
						ValueChanged = Assign,
						RemoveRequest = RemoveEntry
					}));
				}
				return addReference;
			}
		}

		private void RemoveEntry(SimpleTypeWrapper item)
		{
			collection.Remove(item);
		}

		public class SimpleTypeWrapper
		{
			public Action<SimpleTypeWrapper> RemoveRequest { get; set; }
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

			Command removeEntry;
			public Command RemoveEntry
			{
				get
				{
					if (removeEntry == null)
					{
						removeEntry = new Command(() => RemoveRequest(this));
					}
					return removeEntry;
				}
			}
		}

		ObservableCollection<SimpleTypeWrapper> collection;
		Type collectionItemType;

		public Type ValueType { get { return ValueStore.ValueType; } }

		public object Value
		{
			get
			{
				if (collection == null)
				{
					collectionItemType = ValueStore.ValueType.GetTypeInfo().IsGenericType ? ValueStore.ValueType.GenericTypeArguments[0] : null;
					var values = (IEnumerable)ValueStore.Value;
					var wrapper = Enumerable.Empty<SimpleTypeWrapper>();
					if (values != null)
					{
						wrapper = values.OfType<object>().Select(s => new SimpleTypeWrapper
						{
							Value = s,
							ValueChanged = Assign,
							RemoveRequest = RemoveEntry
						});
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

				ValueStore.SetValue(list);
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
