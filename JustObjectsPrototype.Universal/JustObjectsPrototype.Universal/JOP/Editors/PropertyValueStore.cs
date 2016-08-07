using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace JustObjectsPrototype.Universal.JOP.Editors
{
	public class PropertyValueStore : IValueStore
	{
		public static List<IValueStore> ForPropertiesOf(Type type, ObjectProxy selectedObject, Action<ObjectChangedEventArgs> objectChangedCallback)
		{
			var propertyValueStores = type
				.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
				.Where(property => property.GetIndexParameters().Length == 0)
				.Select(property => new PropertyValueStore { Instance = selectedObject, Property = property, ObjectChanged = objectChangedCallback })
				.ToList<IValueStore>();

			return propertyValueStores;
		}


		ObjectProxy _Instance;
		public ObjectProxy Instance
		{
			get { return _Instance; }
			set
			{
				_Instance = value;
				_Instance.PropertyChanged += (s, e) =>
				{
					if (e.PropertyName == Property.Name || e.PropertyName == string.Empty)
					{
						PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.PropertyName));
					}
				};
			}
		}
		public PropertyInfo Property { get; set; }
		public Action<ObjectChangedEventArgs> ObjectChanged { get; set; }


		public string Identifier
		{
			get { return ObjectDisplay.Nicely(Property); }
		}

		public Type ValueType
		{
			get { return Property.PropertyType; }
		}

		public bool CanRead
		{
			get
			{
				return Property.CanRead
					&& !(Property.GetCustomAttribute<EditorAttribute>()?.Hide ?? false);
			}
		}

		public bool CanWrite
		{
			get
			{
				return Property.CanWrite
					&& Property.SetMethod.IsPublic
					&& Property.Name != "ID" && Property.Name != "Id"
					&& !(Property.GetCustomAttribute<EditorAttribute>()?.Readonly ?? false);
			}
		}

		public object Value
		{
			get
			{
				return Property.GetValue(Instance.ProxiedObject);
			}
		}

		public object CustomView
		{
			get
			{
				return Property.GetCustomAttribute<CustomViewAttribute>()?.ResourceKey;
			}
		}

		public void SetValue(object value)
		{
			Property.SetValue(Instance.ProxiedObject, value);

			ObjectChanged?.Invoke(new ObjectChangedEventArgs
			{
				Object = Instance.ProxiedObject,
				PropertyName = Property.Name
			});
			Instance.RaisePropertyChanged(string.Empty);
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
