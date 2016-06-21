using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JustObjectsPrototype.Universal.JOP.Editors
{
	public class ObjectDisplay
	{
		public static readonly DependencyProperty DisplayProperty = DependencyProperty.RegisterAttached(
			"Display",
			typeof(object),
			typeof(TextBlock),
			new PropertyMetadata(null, new PropertyChangedCallback(DisplayPropertyChanged))
		);
		public static void SetDisplayProperty(UIElement element, object value)
		{
			element.SetValue(DisplayProperty, value);
		}
		public static object GetDisplayProperty(UIElement element)
		{
			return (object)element.GetValue(DisplayProperty);
		}
		private static void DisplayPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var textBlock = o as TextBlock;
			if (e.NewValue == null
				||
				(
					e.NewValue != null
					&&
					(
						e.NewValue == ReferenceTypePropertyViewModel.NullEntry
						||
						e.NewValue == ReferenceTypeListPropertyViewModel.NullEntry
						||
						(e.NewValue is string && e.NewValue as string == string.Empty)
					)
				))
			{
				textBlock.Text = "";
			}
			else
			{
				textBlock.Text = ToStringOrJson(e.NewValue);
			}
		}


		public static string ToStringOrJson(object value)
		{
			var type = value.GetType();
			var toStrings = type.GetMethods().Where(m => m.Name == "ToString" && m.DeclaringType == type && m.GetParameters().Length == 0);
			if (toStrings.Any())
			{
				var toStringResult = toStrings.First().Invoke(value, new object[0]);
				return toStringResult != null ? toStringResult.ToString() : string.Empty;
			}

			var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.GetIndexParameters().Length == 0);
			return "{" + string.Join(", ", properties.Select(p => p.Name + ": \"" + p.GetValue(value) + "\"")) + "}";
		}

		public static string Nicely(MemberInfo property)
		{
			return property.Name.Replace("_", " ");
		}
	}
}
