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
                PropertyBridge data = new PropertyBridge(prop);

                if (prop.IsDefined(typeof(ColumnAttribute), false))
                {
                    ColumnAttribute colAttr = (ColumnAttribute)
                        prop.GetCustomAttributes(typeof(ColumnAttribute), false)[0];

                    if (string.IsNullOrEmpty(colAttr.Name))
                        colAttr.Name = prop.Name;

                    SqlColumn column = new SqlColumn(colAttr.Name, data);
                    sqlColumns.Add(column);
                }
                else
                {
                    SqlColumn column = new SqlColumn(prop.Name, data);
                    sqlColumns.Add(column);
                }
            }

            FieldInfo[] fields = type.GetFields(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                FieldBridge data = new FieldBridge(field);
                if (field.IsDefined(typeof(ColumnAttribute), false))
                {
                    ColumnAttribute colAttr = (ColumnAttribute)
                        field.GetCustomAttributes(typeof(ColumnAttribute), false)[0];


                    if (string.IsNullOrEmpty(colAttr.Name))
                        colAttr.Name = field.Name;

                    SqlColumn column = new SqlColumn(colAttr.Name, data);
                    sqlColumns.Add(column);
                }
                else
                {
                    SqlColumn column = new SqlColumn(field.Name, data);
                    sqlColumns.Add(column);
                }
            }

            return sqlColumns;
        }
    }
}