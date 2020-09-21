using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using GeneralDataLayer.Mappings.Implements;
using GeneralDataLayer.Mappings.Interfaces;

namespace GeneralDataLayer.Mappings
{
    public static class ParameterWrapsFactory
    {
        public static List<SqlParameter> ToParameters<T>(T info)
        {
            var wraps = ToParameterWraps(info);
            return wraps.Select(o => o.SqlParameter).ToList();
        }

        public static List<ParameterWrap> ToParameterWraps<T>(T info)
        {
            var wraps = new List<ParameterWrap>();

            Type type = info.GetType();

            PropertyInfo[] propsInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (PropertyInfo prop in propsInfos)
            {
                if (!prop.IsDefined(typeof(ParameterAttribute), false))
                    continue;
                ParameterAttribute parameterAttr = (ParameterAttribute)
                    prop.GetCustomAttributes(typeof(ParameterAttribute), false)[0];

                if (string.IsNullOrEmpty(parameterAttr.Name))
                    parameterAttr.Name = "@" + prop.Name;

                if (!parameterAttr.Name.StartsWith("@"))
                {
                    parameterAttr.Name = "@" + parameterAttr.Name;
                }

                IDataBridge data = new PropertyBridge(prop);

                ParameterWrap wrap = new ParameterWrap()
                {
                    SqlParameter = new SqlParameter(parameterAttr.Name, data.Read(info))
                    , DataBridge = data
                };

                wrap.SqlParameter.Direction = parameterAttr.Direction;

                wrap.SqlParameter.DbType = 
                    parameterAttr.DbType != default(DbType) 
                    ? parameterAttr.DbType 
                    : SqlTypeUtils.ResolveType(prop.PropertyType);

                wraps.Add(wrap);
            }

            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                if (!field.IsDefined(typeof(ParameterAttribute), false))
                    continue;
                ParameterAttribute parameterAttr = (ParameterAttribute) field.GetCustomAttributes(typeof(ParameterAttribute), false)[0];

                if (string.IsNullOrEmpty(parameterAttr.Name))
                    parameterAttr.Name = "@" + field.Name;

                if (!parameterAttr.Name.StartsWith("@"))
                {
                    parameterAttr.Name = "@" + parameterAttr.Name;
                }

                IDataBridge data = new FieldBridge(field);
                ParameterWrap wrap = new ParameterWrap()
                {
                    SqlParameter = new SqlParameter(parameterAttr.Name, data.Read(info))
                    , DataBridge = data
                };

                wrap.SqlParameter.Direction = parameterAttr.Direction;

                wrap.SqlParameter.DbType =
                    parameterAttr.DbType != default(DbType)
                        ? parameterAttr.DbType
                        : SqlTypeUtils.ResolveType(field.FieldType);

                wraps.Add(wrap);
            }

            return wraps;
        }
    }
}