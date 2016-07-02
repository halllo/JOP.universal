using JustObjectsPrototype.Universal.Shell;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JustObjectsPrototype.Universal.JOP.Editors
{
	public class EnumTypePropertyViewModel : ViewModel, IPropertyViewModel
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

		public IEnumerable<object> References
		{
			get
			{
				return Enumerable.Concat(new[] { Value }.Where(v => v != null), Enum.GetNames(ValueType)).Distinct();
			}
		}

		public Type ValueType { get { return ValueStore.ValueType; } }

		public object Value
		{
			get
			{
				return Enum.GetName(ValueType, ValueStore.Value);
			}
			set
			{
				try
				{
					var convertedValue = Enum.Parse(ValueType, (string)value);
					ValueStore.SetValue(convertedValue);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine("Assignment error: " + ex.Message);
					//System.Windows.MessageBox.Show("Assignment error: " + ex.Message);
				}
			}
		}

		public void Refresh()
		{
			Changed(() => Value);
		}
	}
}
