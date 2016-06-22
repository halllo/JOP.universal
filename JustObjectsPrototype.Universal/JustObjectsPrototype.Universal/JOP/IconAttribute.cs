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
}
