using System;
using System.Reflection.Emit;

namespace GeneralDataLayer.Dynamics.Implements
{
    static class OpCodesFactory
    {
        public static void BoxIfNeeded(ILGenerator generator, Type type)
        {
            if (type.IsValueType)
            {
                generator.Emit(OpCodes.Box, type);
            }
        }

        public static void UnboxIfNeeded(ILGenerator generator, Type type)
        {
            if (type.IsValueType)
            {
                generator.Emit(OpCodes.Unbox_Any, type);
            }
        }
    }
}