using System;
using System.ComponentModel;

namespace JustObjectsPrototype.Universal.JOP.Editors
{
	public class SelfcontainedValueStore : IValueStore
	{
		public bool CanWrite
		{
			get { return true; }
		}

		public string Identifier
		{
			get; set;
		}

		public object Value
		{
			get; set;
		}

		public Type ValueType
		{
			get; set;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void SetValue(object value)
		{
			Value = value;
		}
	}
}
