using System;
using Windows.UI.Xaml.Controls;

namespace JustObjectsPrototype.Universal.JOP
{
	public class IconAttribute : Attribute
	{
		public IconAttribute(Symbol icon)
		{
			Icon = icon;
		}

		public Symbol Icon { get; private set; }
	}

	public class EditorAttribute : Attribute
	{
		public EditorAttribute(bool hide = false, bool @readonly = false)
		{
			Hide = hide;
			Readonly = @readonly;
		}

		public bool Hide { get; private set; }
		public bool Readonly { get; private set; }
	}
}
