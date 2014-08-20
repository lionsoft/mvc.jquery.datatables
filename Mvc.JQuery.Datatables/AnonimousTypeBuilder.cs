using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Mvc.JQuery.Datatables
{
    public static class AnonimousTypeBuilder
    {
        static readonly AssemblyBuilder _dynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("DynamicAssembly_" + Guid.NewGuid()), AssemblyBuilderAccess.Run);
        static readonly ModuleBuilder _dynamicModule = _dynamicAssembly.DefineDynamicModule("DynamicAssemblyModule_" + Guid.NewGuid());

        public static TypeBuilder CreateTypeBuilder()
        {
            return _dynamicModule.DefineType("DynamicType_" + Guid.NewGuid(), TypeAttributes.Public);
        }

        public static void AddProperty(this TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.HideBySig;

            var field = typeBuilder.DefineField("_" + propertyName, typeof(string), FieldAttributes.Private);
            var property = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType, new[] { propertyType });

            var getMethodBuilder = typeBuilder.DefineMethod("get_value", getSetAttr, propertyType, Type.EmptyTypes);
            var getIl = getMethodBuilder.GetILGenerator();
            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, field);
            getIl.Emit(OpCodes.Ret);

            var setMethodBuilder = typeBuilder.DefineMethod("set_value", getSetAttr, null, new[] { propertyType });
            var setIl = setMethodBuilder.GetILGenerator();
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, field);
            setIl.Emit(OpCodes.Ret);

            property.SetGetMethod(getMethodBuilder);
            property.SetSetMethod(setMethodBuilder);
        }
    }
}