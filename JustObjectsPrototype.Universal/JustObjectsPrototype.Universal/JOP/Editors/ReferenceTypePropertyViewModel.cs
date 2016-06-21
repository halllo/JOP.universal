using JustObjectsPrototype.Universal.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JustObjectsPrototype.Universal.JOP.Editors
{
	public class ReferenceTypePropertyViewModel : ViewModel, IPropertyViewModel
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
				return Enumerable.Concat(new[] { NullEntry, Value ?? NullEntry }, Objects).Distinct();
			}
		}

		public Type ValueType { get { return Property.PropertyType; } }

		public object Value
		{
			get
			{
				return Property.GetValue(Instance.ProxiedObject) ?? NullEntry;
			}
			set
			{
				if (value == NullEntry) value = null;

				Property.SetValue(Instance.ProxiedObject, value);
				if (ObjectChanged != null) ObjectChanged(new ObjectChangedEventArgs { Object = Instance.ProxiedObject, PropertyName = Property.Name });
				Instance.RaisePropertyChanged(string.Empty);
			}
		}

		public void Refresh()
		{
			Changed(() => Value);
		}
	}
}
