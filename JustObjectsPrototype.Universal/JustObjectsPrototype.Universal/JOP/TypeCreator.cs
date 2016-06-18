using System;
using System.Collections.Generic;
//using System.Reflection;
//using System.Reflection.Emit;

namespace JustObjectsPrototype.Universal.JOP
{
	public static class TypeCreator
	{
		public static Type New(string typeName, IEnumerable<KeyValuePair<string, Type>> properties)
		{
			throw new NotImplementedException();
			//var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("DynamicAssembly"), AssemblyBuilderAccess.Run);
			//var moduleBuilder = assemblyBuilder.DefineDynamicModule("Dynamic.dll");
			//var typeBuilder = moduleBuilder.DefineType(typeName);

			//foreach (var newProperty in properties)
			//{
			//	var propertyName = newProperty.Key;
			//	var propertyType = newProperty.Value;

			//	FieldBuilder propertyField = typeBuilder.DefineField("m" + propertyName, propertyType, FieldAttributes.Private);
			//	PropertyBuilder property = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

			//	MethodBuilder propertyGetter = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
			//	ILGenerator propertyGetterIL = propertyGetter.GetILGenerator();
			//	propertyGetterIL.Emit(OpCodes.Ldarg_0);
			//	propertyGetterIL.Emit(OpCodes.Ldfld, propertyField);
			//	propertyGetterIL.Emit(OpCodes.Ret);

			//	MethodBuilder propertySetter = typeBuilder.DefineMethod("set_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new Type[] { propertyType });
			//	ILGenerator propertySetterIL = propertySetter.GetILGenerator();
			//	propertySetterIL.Emit(OpCodes.Ldarg_0);
			//	propertySetterIL.Emit(OpCodes.Ldarg_1);
			//	propertySetterIL.Emit(OpCodes.Stfld, propertyField);
			//	propertySetterIL.Emit(OpCodes.Ret);

			//	property.SetGetMethod(propertyGetter);
			//	property.SetSetMethod(propertySetter);
			//}

			//return typeBuilder.CreateType();
		}
	}
}
