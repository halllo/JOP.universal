using System;
using System.ComponentModel;
using System.Reflection;

namespace JustObjectsPrototype.Universal.JOP.Editors
{
	public class PropertyValueStore : IValueStore
	{
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


		public event PropertyChangedEventHandler PropertyChanged;

		public string Identifier
		{
			get { return ObjectDisplay.Nicely(Property); }
		}

		public Type ValueType
		{
			get { return Property.PropertyType; }
		}

		public bool CanWrite
		{
			get
			{
				return Property.CanWrite
					&& Property.SetMethod.IsPublic
					&& Property.Name != "ID" && Property.Name != "Id";
			}
		}

		public object Value
		{
			get
			{
				return Property.GetValue(Instance.ProxiedObject);
			}
		}

		public void SetValue(object value)
		{
			Property.SetValue(Instance.ProxiedObject, value);

			if (ObjectChanged != null) ObjectChanged(new ObjectChangedEventArgs
			{
				Object = Instance.ProxiedObject,
				PropertyName = Property.Name
			});
			Instance.RaisePropertyChanged(string.Empty);
		}
	}
}
