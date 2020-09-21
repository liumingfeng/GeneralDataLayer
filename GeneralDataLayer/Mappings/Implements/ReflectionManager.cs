using System.Reflection;
using GeneralDataLayer.Dynamics.Implements;
using GeneralDataLayer.Dynamics.Interfaces;

namespace GeneralDataLayer.Mappings.Implements
{
    public static class ReflectionManager
    {
        public static IDynamicDataInfo CreateDynamicInfo(PropertyInfo info)
        {
            return new DynamicPropertyInfo(info);
        }

        public static IDynamicDataInfo CreateDynamicInfo(FieldInfo info)
        {
            return new DynamicFieldInfo(info);
        }
    }
}