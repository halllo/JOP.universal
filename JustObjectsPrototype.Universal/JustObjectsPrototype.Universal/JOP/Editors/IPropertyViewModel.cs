using System;

namespace JustObjectsPrototype.Universal.JOP.Editors
{
	public interface IPropertyViewModel
	{
		string Label { get; }
		object Value { get; }
		Type ValueType { get; }
		void Refresh();
		Action<ObjectChangedEventArgs> ObjectChanged { set; }
	}

	public class ObjectChangedEventArgs
	{
		public object Object { get; set; }
		public string PropertyName { get; set; }
	}
}
