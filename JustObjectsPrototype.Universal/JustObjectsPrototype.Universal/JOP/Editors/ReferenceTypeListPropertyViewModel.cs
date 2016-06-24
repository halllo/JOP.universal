using JustObjectsPrototype.Universal.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace JustObjectsPrototype.Universal.JOP.Editors
{
	public class ReferenceTypeListPropertyViewModel : ViewModel, IPropertyViewModel
	{
		public static object NullEntry = " ";

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

		public IEnumerable<object> Objects { private get; set; }

		public bool CanWrite { get { return ValueStore.CanWrite; } }

		public string Label { get { return ValueStore.Identifier; } }

		public IEnumerable<object> References
		{
			get
			{
				var refs = Enumerable.Concat
				(
					Enumerable.Concat
					(
						new[] { NullEntry },
						((IEnumerable)ValueStore.Value ?? Enumerable.Empty<object>()).OfType<object>()
					),
					Objects
				).Distinct();
				return refs;
			}
		}

		Command addReference;
		public Command AddReference
		{
			get
			{
				if (addReference == null)
				{
					addReference = new Command(() => collection.Add(new ReferenceTypeWrapper
					{
						Value = NullEntry,
						References = References,
						ValueChanged = Assign,
						RemoveRequest = RemoveReference
					}));
				}
				return addReference;
			}
		}

		private void RemoveReference(ReferenceTypeWrapper item)
		{
			collection.Remove(item);
		}

		public class ReferenceTypeWrapper
		{
			public Action<ReferenceTypeWrapper> RemoveRequest { get; set; }
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

			public object References { get; set; }

			Command removeReference;
			public Command RemoveReference
			{
				get
				{
					if (removeReference == null)
					{
						removeReference = new Command(() => RemoveRequest(this));
					}
					return removeReference;
				}
			}
		}

		ObservableCollection<ReferenceTypeWrapper> collection;
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
					var wrapper = Enumerable.Empty<ReferenceTypeWrapper>();
					if (values != null)
					{
						wrapper = values.Cast<object>().Select(s => new ReferenceTypeWrapper
						{
							Value = s ?? NullEntry,
							References = References,
							ValueChanged = Assign,
							RemoveRequest = RemoveReference
						});
					}
					collection = new ObservableCollection<ReferenceTypeWrapper>(wrapper);
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
			try
			{
				var canBeNull = !collectionItemType.GetTypeInfo().IsValueType || (Nullable.GetUnderlyingType(collectionItemType) != null);
				var listType = typeof(List<>);
				var constructedListType = listType.MakeGenericType(collectionItemType);
				var list = (IList)Activator.CreateInstance(constructedListType);
				foreach (var item in collection)
				{
					if (!canBeNull && item.Value == null) continue;
					list.Add(item.Value == NullEntry ? null : Convert.ChangeType(item.Value, collectionItemType ?? typeof(object)));
				}

				ValueStore.SetValue(list);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Assignment error: " + ex.Message);
				//System.Windows.MessageBox.Show("Assignment error: " + ex.Message);
			}
		}

		public void Refresh()
		{
			collection = null;
			Changed(() => Value);
		}
	}
}
