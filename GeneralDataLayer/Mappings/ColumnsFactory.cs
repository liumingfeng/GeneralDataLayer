using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using GeneralDataLayer.Mappings.Implements;

namespace GeneralDataLayer.Mappings
{
    public static class ColumnsFactory
    {
        public static List<SqlColumn> ToColumns(Type type)
        {
            var sqlColumns = new List<SqlColumn>();

            PropertyInfo[] propsInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (PropertyInfo prop in propsInfos)
            {
                if (!prop.IsDefined(typeof(ColumnAttribute), false))
                    continue;
                ColumnAttribute colAttr = (ColumnAttribute)
                    prop.GetCustomAttributes(typeof(ColumnAttribute), false)[0];

                PropertyBridge data = new PropertyBridge(prop);

                if (string.IsNullOrEmpty(colAttr.Name))
                    colAttr.Name = prop.Name;

                SqlColumn column = new SqlColumn(colAttr.Name, data);

                colAttr.DbType =
                    colAttr.DbType != default(DbType)
                        ? colAttr.DbType
                        : SqlTypeUtils.ResolveType(prop.PropertyType);

                sqlColumns.Add(column);
            }

            FieldInfo[] fields = type.GetFields(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                if (!field.IsDefined(typeof(ColumnAttribute), false))
                    continue;
                ColumnAttribute colAttr = (ColumnAttribute)
                    field.GetCustomAttributes(typeof(ColumnAttribute), false)[0];

                FieldBridge data = new FieldBridge(field);

                if (string.IsNullOrEmpty(colAttr.Name))
                    colAttr.Name = field.Name;

                SqlColumn column = new SqlColumn(colAttr.Name, data);

                colAttr.DbType =
                    colAttr.DbType != default(DbType)
                        ? colAttr.DbType
                        : SqlTypeUtils.ResolveType(field.FieldType);

                sqlColumns.Add(column);
            }

            return sqlColumns;
        }
    }
}