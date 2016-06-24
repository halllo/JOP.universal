using JustObjectsPrototype.Universal.Shell;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JustObjectsPrototype.Universal.JOP.Editors
{
	public class ReferenceTypePropertyViewModel : ViewModel, IPropertyViewModel
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
				return Enumerable.Concat(new[] { NullEntry, Value ?? NullEntry }, Objects).Distinct();
			}
		}

		public Type ValueType { get { return ValueStore.ValueType; } }

		public object Value
		{
			get
			{
				return ValueStore.Value ?? NullEntry;
			}
			set
			{
				if (value == NullEntry) value = null;

				ValueStore.SetValue(value);
			}
		}

		public void Refresh()
		{
			Changed(() => Value);
		}
	}
}
