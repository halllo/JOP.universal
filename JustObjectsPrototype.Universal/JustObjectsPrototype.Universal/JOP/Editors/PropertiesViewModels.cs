using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JustObjectsPrototype.Universal.JOP.Editors
{
	public static class PropertiesViewModels
	{
		public static List<IPropertyViewModel> Of(Type type, Objects objects, ObjectProxy selectedObject, Action<ObjectChangedEventArgs> objectChangedCallback)
		{
			var properties = type
				.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
				.Where(p => p.GetIndexParameters().Length == 0);

			var propertiesViewModels =
				from property in properties
				let valueStore = new PropertyValueStore { Instance = selectedObject, Property = property, ObjectChanged = objectChangedCallback }
				select property.CanRead && property.PropertyType == typeof(DateTime) ? (IPropertyViewModel)new DateTimePropertyViewModel { ValueStore = valueStore }

					 : property.CanRead && property.PropertyType == typeof(bool) ? (IPropertyViewModel)new BooleanPropertyViewModel { ValueStore = valueStore }

					 : property.CanRead && property.PropertyType == typeof(string) ? (IPropertyViewModel)new SimpleTypePropertyViewModel { ValueStore = valueStore }

					 : property.CanRead
							&& property.PropertyType.GetTypeInfo().IsGenericType
							&& (property.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
								||
								property.PropertyType.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IEnumerable)))
							&& objects.Types.Contains(property.PropertyType.GetGenericArguments().FirstOrDefault()) ? (IPropertyViewModel)new ReferenceTypeListPropertyViewModel { ValueStore = valueStore, Objects = objects.OfType(property.PropertyType.GetGenericArguments().FirstOrDefault()).Select(o => o.ProxiedObject) }

					: property.CanRead
							&& property.PropertyType.GetTypeInfo().IsGenericType
							&& (property.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
								||
								property.PropertyType.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IEnumerable))) ?
																														(IPropertyViewModel)new SimpleTypeListPropertyViewModel { ValueStore = valueStore }

					 : property.CanRead && objects.Types.Contains(property.PropertyType) ? (IPropertyViewModel)new ReferenceTypePropertyViewModel { ValueStore = valueStore, Objects = objects.OfType(property.PropertyType).Select(o => o.ProxiedObject) }

					 : property.CanRead ? (IPropertyViewModel)new SimpleTypePropertyViewModel { ValueStore = valueStore }
					 : null;

			return propertiesViewModels.Where(p => p != null).ToList<IPropertyViewModel>();
		}
	}
}
