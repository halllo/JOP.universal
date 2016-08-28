using JustObjectsPrototype.Universal.JOP;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JustObjectsPrototype.Universal.Shell
{
	public class ItemTemplateSelector : DataTemplateSelector
	{
		public DataTemplate DefaultTemplate { get; set; }

		protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
		{
			var objectProxy = item as ObjectProxy;
			if (objectProxy != null)
			{
				var customViewResourceKey = objectProxy.ProxiedObject.GetType().GetTypeInfo().GetCustomAttribute<CustomViewAttribute>()?.ResourceKey;
				if (customViewResourceKey != null)
				{
					return (DataTemplate)Application.Current.Resources[customViewResourceKey];
				}
			}

			return DefaultTemplate;
		}
	}
}
