using System;
using System.Reflection;
using System.Reflection.Emit;

namespace GeneralDataLayer.Dynamics.Implements
{
    internal static class DynamicMethodFactory
    {
        /// <summary>
        /// create PropertyInfo get
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        internal static DynamicPropertyGetHandler CreateGetHandler(Type type, PropertyInfo propertyInfo)
        {
            DynamicMethod dynamicGet = CreateGetDynamicMethod(type);

            ILGenerator getGenerator = dynamicGet.GetILGenerator();

            getGenerator.Emit(OpCodes.Ldarg_0);

            MethodInfo getMethodInfo = propertyInfo.GetGetMethod(true);
            if (getMethodInfo != null)
            {
                getGenerator.Emit(OpCodes.Callvirt, getMethodInfo);
                OpCodesFactory.BoxIfNeeded(getGenerator, getMethodInfo.ReturnType);
            }

            getGenerator.Emit(OpCodes.Ret);

            return (DynamicPropertyGetHandler)dynamicGet.CreateDelegate(typeof(DynamicPropertyGetHandler));
        }

        /// <summary>
        /// create PropertyInfo Set
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        internal static DynamicPropertySetHandler CreateSetHandler(Type type, PropertyInfo propertyInfo)
        {
            DynamicMethod dynamicSet = CreateSetDynamicMethod(type);

            ILGenerator setGenerator = dynamicSet.GetILGenerator();

            setGenerator.Emit(OpCodes.Ldarg_0);

            MethodInfo setMethodInfo = propertyInfo.GetSetMethod(true);

            if (setMethodInfo != null)
            {
                setGenerator.Emit(OpCodes.Ldarg_1);

                OpCodesFactory.UnboxIfNeeded(setGenerator, setMethodInfo.GetParameters()[0].ParameterType);

                setGenerator.Emit(OpCodes.Call, setMethodInfo);
            }

            setGenerator.Emit(OpCodes.Ret);

            return (DynamicPropertySetHandler)dynamicSet.CreateDelegate(typeof(DynamicPropertySetHandler));
        }

        /// <summary>
        /// create Field dynamic get
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        internal static DynamicFieldGetHandler CreateGetHandler(Type type, FieldInfo fieldInfo)
        {
            DynamicMethod dynamicGet = CreateGetDynamicMethod(type);
            ILGenerator getGenerator = dynamicGet.GetILGenerator();

            getGenerator.Emit(OpCodes.Ldarg_0); // Ldarg => load argument[0]
            getGenerator.Emit(OpCodes.Ldfld, fieldInfo); // Ldfld => load field
            OpCodesFactory.BoxIfNeeded(getGenerator, fieldInfo.FieldType);
            getGenerator.Emit(OpCodes.Ret); 

            return (DynamicFieldGetHandler)dynamicGet.CreateDelegate(typeof(DynamicFieldGetHandler));
        }

        /// <summary>
        /// create Field dynamic set
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        internal static DynamicFieldSetHandler CreateSetHandler(Type type, FieldInfo fieldInfo)
        {
            DynamicMethod dynamicSet = CreateSetDynamicMethod(type);
            ILGenerator setGenerator = dynamicSet.GetILGenerator();

            setGenerator.Emit(OpCodes.Ldarg_0);
            setGenerator.Emit(OpCodes.Ldarg_1);
            OpCodesFactory.UnboxIfNeeded(setGenerator, fieldInfo.FieldType);
            setGenerator.Emit(OpCodes.Stfld, fieldInfo); // Stfld => set field
            setGenerator.Emit(OpCodes.Ret);

            return (DynamicFieldSetHandler)dynamicSet.CreateDelegate(typeof(DynamicFieldSetHandler));
        }

        private static DynamicMethod CreateGetDynamicMethod(Type type)
        {
            return new DynamicMethod("DynamicGet"
                , typeof(object)
                , new Type[]
                {
                    typeof(object)
                }
                , type
                , true);
        }

        private static DynamicMethod CreateSetDynamicMethod(Type type)
        {
            return new DynamicMethod("DynamicSet"
                , typeof(void)
                , new Type[]
                {
                    typeof(object)
                    , typeof(object)
                }
                , type
                , true);
        }
    }
}