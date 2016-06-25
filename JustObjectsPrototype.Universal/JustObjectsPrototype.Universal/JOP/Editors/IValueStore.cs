using System;
using System.ComponentModel;

namespace JustObjectsPrototype.Universal.JOP.Editors
{
	public interface IValueStore : INotifyPropertyChanged
	{
		string Identifier { get; }
		Type ValueType { get; }
		bool CanRead { get; }
		bool CanWrite { get; }
		object Value { get; }

		void SetValue(object value);
	}

	public class ObjectChangedEventArgs
	{
		public object Object { get; set; }
		public string PropertyName { get; set; }
	}
}
