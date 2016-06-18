using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace JustObjectsPrototype.Universal.JOP
{
	public class ObjectProxy : DynamicObject, INotifyPropertyChanged
	{
		public object ProxiedObject { get; set; }

		public ObjectProxy() { }
		public ObjectProxy(object proxiedObject)
		{
			ProxiedObject = proxiedObject;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public override bool TryConvert(ConvertBinder binder, out object result)
		{
			if (binder.Type == typeof(INotifyPropertyChanged))
			{
				result = this;
				return true;
			}

			if (ProxiedObject != null && binder.Type.IsAssignableFrom(ProxiedObject.GetType()))
			{
				result = ProxiedObject;
				return true;
			}
			else
				return base.TryConvert(binder, out result);
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			result = GetMember(binder.Name);
			return true;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			SetMember(binder.Name, value);
			return true;
		}

		private PropertyInfo GetPropertyInfo(string propertyName)
		{
			return ProxiedObject.GetType().GetProperties().First(propertyInfo => propertyInfo.Name == propertyName);
		}

		private void SetMember(string propertyName, object value)
		{
			GetPropertyInfo(propertyName).SetValue(ProxiedObject, value, null);
			RaisePropertyChanged(propertyName);
		}

		private object GetMember(string propertyName)
		{
			var result = GetPropertyInfo(propertyName).GetValue(ProxiedObject, null);

			if (result != null)
			{
				var resultType = result.GetType();
				if (resultType.IsConstructedGenericType//IsGenericType
					&& (resultType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
						||
						resultType.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IEnumerable)))
					)
				{
					var ie = (IEnumerable)result;
					return "Count = " + ie.OfType<object>().Count();
				}
			}

			return result;
		}
	}
}
