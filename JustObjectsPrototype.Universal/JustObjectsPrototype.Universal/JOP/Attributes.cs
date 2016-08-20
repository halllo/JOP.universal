using System;
using Windows.UI.Xaml.Controls;

namespace JustObjectsPrototype.Universal.JOP
{
	public abstract class JOPAttribute : Attribute
	{
	}

	public class IconAttribute : JOPAttribute
	{
		public IconAttribute(Symbol icon)
		{
			Icon = icon;
		}

		public Symbol Icon { get; private set; }
	}

	public class TitleAttribute : JOPAttribute
	{
		public TitleAttribute(string title)
		{
			Title = title;
		}

		public string Title { get; private set; }
	}

	public class EditorAttribute : JOPAttribute
	{
		public EditorAttribute(bool hide = false, bool @readonly = false)
		{
			Hide = hide;
			Readonly = @readonly;
		}

		public bool Hide { get; private set; }
		public bool Readonly { get; private set; }
	}

	public class CustomViewAttribute : JOPAttribute
	{
		public CustomViewAttribute(string dateTemplateResourceKey)
		{
			ResourceKey = dateTemplateResourceKey;
		}

		public string ResourceKey { get; private set; }
	}

	public class JumpsToResultAttribute : JOPAttribute
	{
		public JumpsToResultAttribute()
		{
		}
	}

	public class RequiresConfirmationAttribute : JOPAttribute
	{
		public RequiresConfirmationAttribute()
		{
		}
	}
}
