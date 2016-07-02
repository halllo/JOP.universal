using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JustObjectsPrototype.Universal.JOP.Editors
{
	public static class PropertiesViewModels
	{
		public static List<IPropertyViewModel> Of(List<IValueStore> valueStores, Objects objects)
		{
			var propertiesViewModels =
				from valueStore in valueStores
				select valueStore.CanRead && valueStore.ValueType == typeof(DateTime) ? (IPropertyViewModel)new DateTimePropertyViewModel { ValueStore = valueStore }

					 : valueStore.CanRead && valueStore.ValueType == typeof(bool) ? (IPropertyViewModel)new BooleanPropertyViewModel { ValueStore = valueStore }

					 : valueStore.CanRead && valueStore.ValueType == typeof(string) ? (IPropertyViewModel)new SimpleTypePropertyViewModel { ValueStore = valueStore }

					 : valueStore.CanRead && valueStore.ValueType.GetTypeInfo().IsEnum ? (IPropertyViewModel)new EnumTypePropertyViewModel { ValueStore = valueStore }

					 : valueStore.CanRead
							&& valueStore.ValueType.GetTypeInfo().IsGenericType
							&& (valueStore.ValueType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
								||
								valueStore.ValueType.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IEnumerable)))
							&& objects.Types.Contains(valueStore.ValueType.GetGenericArguments().FirstOrDefault()) ? (IPropertyViewModel)new ReferenceTypeListPropertyViewModel { ValueStore = valueStore, Objects = objects.OfType(valueStore.ValueType.GetGenericArguments().FirstOrDefault()).Select(o => o.ProxiedObject) }

					: valueStore.CanRead
							&& valueStore.ValueType.GetTypeInfo().IsGenericType
							&& (valueStore.ValueType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
								||
								valueStore.ValueType.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IEnumerable))) ?
																														(IPropertyViewModel)new SimpleTypeListPropertyViewModel { ValueStore = valueStore }

					 : valueStore.CanRead && objects.Types.Contains(valueStore.ValueType) ? (IPropertyViewModel)new ReferenceTypePropertyViewModel { ValueStore = valueStore, Objects = objects.OfType(valueStore.ValueType).Select(o => o.ProxiedObject) }

					 : valueStore.CanRead ? (IPropertyViewModel)new SimpleTypePropertyViewModel { ValueStore = valueStore }
					 : null;

			return propertiesViewModels.Where(p => p != null).ToList<IPropertyViewModel>();
		}
	}
}
