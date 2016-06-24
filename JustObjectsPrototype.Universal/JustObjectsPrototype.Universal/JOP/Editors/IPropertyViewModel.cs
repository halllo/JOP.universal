using System;

namespace JustObjectsPrototype.Universal.JOP.Editors
{
	public interface IPropertyViewModel
	{
		string Label { get; }
		object Value { get; }
		Type ValueType { get; }
		void Refresh();
	}
}
