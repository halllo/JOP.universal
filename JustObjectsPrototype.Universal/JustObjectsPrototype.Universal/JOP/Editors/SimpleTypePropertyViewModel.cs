using JustObjectsPrototype.Universal.Shell;
using System;

namespace JustObjectsPrototype.Universal.JOP.Editors
{
	public class DateTimePropertyViewModel : SimpleTypePropertyViewModel { }

	public class BooleanPropertyViewModel : SimpleTypePropertyViewModel { }

	public class SimpleTypePropertyViewModel : ViewModel, IPropertyViewModel
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

		public Type ValueType { get { return ValueStore.ValueType; } }

		public object Value
		{
			get
			{
				return ValueStore.Value;
			}
			set
			{
				try
				{
					var convertedValue = Convert.ChangeType(value, ValueType);
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
