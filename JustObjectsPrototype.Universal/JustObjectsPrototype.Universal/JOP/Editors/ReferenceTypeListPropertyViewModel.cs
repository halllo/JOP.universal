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
		public IEnumerable<object> Objects { private get; set; }
		public Action<ObjectChangedEventArgs> ObjectChanged { private get; set; }

		public bool CanWrite
		{
			get { return Property.CanWrite && Property.SetMethod.IsPublic; }
		}

		public string Label { get { return ObjectDisplay.Nicely(Property); } }

		public IEnumerable<object> References
		{
			get
			{
				var refs = Enumerable.Concat
				(
					Enumerable.Concat
					(
						new[] { NullEntry },
						((IEnumerable)Property.GetValue(Instance.ProxiedObject) ?? Enumerable.Empty<object>()).OfType<object>()
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

		public Type ValueType { get { return Property.PropertyType; } }

		public object Value
		{
			get
			{
				if (collection == null)
				{
					collectionItemType = Property.PropertyType.GetTypeInfo().IsGenericType ? Property.PropertyType.GenericTypeArguments[0] : null;
					var values = (IEnumerable)Property.GetValue(Instance.ProxiedObject);
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

				Property.SetValue(Instance.ProxiedObject, list);
				if (ObjectChanged != null) ObjectChanged(new ObjectChangedEventArgs { Object = Instance.ProxiedObject, PropertyName = Property.Name });
				Instance.RaisePropertyChanged(string.Empty);
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
