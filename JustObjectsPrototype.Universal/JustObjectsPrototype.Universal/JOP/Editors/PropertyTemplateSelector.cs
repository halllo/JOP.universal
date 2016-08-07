using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JustObjectsPrototype.Universal.JOP.Editors
{
	public class PropertyTemplateSelector : DataTemplateSelector
	{
		public DataTemplate DateTimePropertyTemplate { get; set; }
		public DataTemplate BooleanPropertyTemplate { get; set; }
		public DataTemplate SimpleTypePropertyTemplate { get; set; }
		public DataTemplate EnumTypePropertyTemplate { get; set; }
		public DataTemplate SimpleTypeListPropertyTemplate { get; set; }
		public DataTemplate ReferenceTypePropertyTemplate { get; set; }
		public DataTemplate ReferenceTypeListPropertyTemplate { get; set; }

		protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
		{
			if (item is CustomViewViewModel) { return ((CustomViewViewModel)item).CustomView; }
			else if (item is DateTimePropertyViewModel) { return DateTimePropertyTemplate; }
			else if (item is BooleanPropertyViewModel) { return BooleanPropertyTemplate; }
			else if (item is SimpleTypePropertyViewModel) { return SimpleTypePropertyTemplate; }
			else if (item is EnumTypePropertyViewModel) { return EnumTypePropertyTemplate; }
			else if (item is SimpleTypeListPropertyViewModel) { return SimpleTypeListPropertyTemplate; }
			else if (item is ReferenceTypePropertyViewModel) { return ReferenceTypePropertyTemplate; }
			else if (item is ReferenceTypeListPropertyViewModel) { return ReferenceTypeListPropertyTemplate; }
			else { return SimpleTypePropertyTemplate; }
		}
	}
}
