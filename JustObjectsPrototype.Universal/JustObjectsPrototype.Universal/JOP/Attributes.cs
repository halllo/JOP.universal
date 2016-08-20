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

	public class TitleAttribute : Attribute
	{
		public TitleAttribute(string title)
		{
			Title = title;
		}

		public string Title { get; private set; }
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

	public class CustomViewAttribute : Attribute
	{
		public CustomViewAttribute(string dateTemplateResourceKey)
		{
			ResourceKey = dateTemplateResourceKey;
		}

		public string ResourceKey { get; private set; }
	}

	public class JumpsToResultAttribute : Attribute
	{
		public JumpsToResultAttribute()
		{
		}
	}

	public class RequiresConfirmationAttribute : Attribute
	{
		public RequiresConfirmationAttribute()
		{
		}
	}
}
