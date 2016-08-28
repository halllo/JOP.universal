using System;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JustObjectsPrototype.Universal.JOP.Editors
{
	public class ObjectDisplay : ContentControl
	{
		public static readonly DependencyProperty DisplayProperty = DependencyProperty.Register(
			"Display",
			typeof(object),
			typeof(ObjectDisplay),
			new PropertyMetadata(null, new PropertyChangedCallback(DisplayPropertyChanged))
		);

		public object Display
		{
			get { return (object)GetValue(DisplayProperty); }
			set { SetValue(DisplayProperty, value); }
		}

		private static void DisplayPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var objectDisplay = o as ObjectDisplay;
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
				objectDisplay.Content = "";
			}
			else
			{
				objectDisplay.Content = ToStringOrJson(e.NewValue);
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
			else
			{
				var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.GetIndexParameters().Length == 0);
				return "{" + string.Join(", ", properties.Select(p => p.Name + ": \"" + p.GetValue(value) + "\"")) + "}";
			}
		}

		public static string ToStringOrFirstString(object value)
		{
			var type = value.GetType();
			var toStrings = type.GetMethods().Where(m => m.Name == "ToString" && m.DeclaringType == type && m.GetParameters().Length == 0);
			if (toStrings.Any())
			{
				var toStringResult = toStrings.First().Invoke(value, new object[0]);
				return toStringResult != null ? toStringResult.ToString() : string.Empty;
			}
			else
			{
				var properties = type
					.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
					.Where(p => p.GetIndexParameters().Length == 0)
					.ToList();

				var firstString = properties.FirstOrDefault(p => p.PropertyType == typeof(string));
				return firstString != null ? FirstCharacters(100, firstString.GetValue(value, new object[0]) as string) : value.ToString();
			}
		}

		private static string FirstCharacters(int count, string text)
		{
			return string.IsNullOrEmpty(text) ? text : new string(text.Take(count).ToArray());
		}

		internal static string Nicely(Type type)
		{
			return type.Name.Replace("_", " ");
		}

		public static string Nicely(MemberInfo property)
		{
			return property.Name.Replace("_", " ");
		}

		public static string Nicely(ParameterInfo parameter)
		{
			return parameter.Name.Replace("_", " ");
		}
	}
}
