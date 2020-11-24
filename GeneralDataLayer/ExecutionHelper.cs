using GeneralDataLayer.Mappings;
using GeneralDataLayer.Mappings.Implements;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeneralDataLayer
{
    public static class ExecutionHelper
    {
        public static async Task<TReturn> ExecuteReaderAsync<TParams, TReturn>(string connectionString
            , CommandContent commandContent
            , TParams paramsObj)
        {
            var wraps = ParameterWrapsFactory.ToParameterWraps(paramsObj);
            var parameters = wraps.Select(o => o.SqlParameter).ToArray();

            return await ExecuteReaderAsync<TReturn>(connectionString, commandContent, parameters);
        }

        public static async Task<T> ExecuteReaderAsync<T>(string connectionString
            , CommandContent commandContent
            , SqlParameter[] parameters = null)
        {
            var list = await ExecuteReaderListAsync<T>(connectionString, commandContent, parameters);
            return list.FirstOrDefault();
        }

        public static async Task<List<TReturn>> ExecuteReaderListAsync<TParams, TReturn>(string connectionString
            , CommandContent commandContent
            , TParams paramsObj)
        {
            var wraps = ParameterWrapsFactory.ToParameterWraps(paramsObj);
            var parameters = wraps.Select(o => o.SqlParameter).ToArray();

            return await ExecuteReaderListAsync<TReturn>(connectionString, commandContent, parameters);
        }

        public static async Task<List<T>> ExecuteReaderListAsync<T>(string connectionString
            , CommandContent commandContent
            , SqlParameter[] parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var dbCommand = new SqlCommand(commandContent.Value, connection)
                {
                    CommandType = commandContent.Type
                })
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        dbCommand.Parameters.AddRange(parameters);
                    }

                    await connection.OpenAsync(CancellationToken.None);
                    using (var reader = await dbCommand.ExecuteReaderAsync(CancellationToken.None))
                    {
                        List<T> list = new List<T>();

                        HashSet<string> fieldList = new HashSet<string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            fieldList.Add(reader.GetName(i).ToLower());
                        }

                        var columns = ColumnsFactory.ToColumns(typeof(T));

                        var sqlColumns = columns.FindAll(p => fieldList.Contains(p.Name.ToLower()));

                        if (sqlColumns.Count > 0)
                        {
                            Dictionary<String, Int32> columnNameDict = new Dictionary<String, Int32>();
                            Dictionary<Int32, Type> typeDict = new Dictionary<Int32, Type>();

                            while (reader.Read())
                            {
                                object item = Activator.CreateInstance(typeof(T), true);

                                foreach (SqlColumn column in sqlColumns)
                                {
                                    int columnIndex = GetFieldIndex(columnNameDict, column.Name, reader);
                                    Type columnType = GetFieldType(typeDict, columnIndex, reader);
                                    object value = GetFieldValue(columnIndex, columnType, reader, column.DataType);

                                    column.SetValue(item, value);
                                }

                                list.Add((T)item);
                            }
                        }
                        return list;
                    }
                }
            }
        }

        public static async Task<int> ExecuteNonQueryAsync<T>(string connectionString
            , CommandContent commandContent
            , T entity)
        {
            var wraps = ParameterWrapsFactory.ToParameterWraps(entity);

            var parameters = wraps.Select(o => o.SqlParameter).ToArray();

            int result = await ExecuteNonQueryAsync(connectionString, commandContent, parameters);


            var outputWraps = wraps.FindAll(p => p.SqlParameter.Direction == ParameterDirection.Output 
                                                      || p.SqlParameter.Direction == ParameterDirection.InputOutput
                                                      || p.SqlParameter.Direction == ParameterDirection.ReturnValue);

            foreach(var outputWrap in outputWraps)
            {
                outputWrap.DataBridge.Write(entity, outputWrap.SqlParameter.Value);
            }

            return result;
        }

        public static async Task<int> ExecuteNonQueryAsync(string connectionString
            , CommandContent commandContent
            , SqlParameter[] parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var dbCommand = new SqlCommand(commandContent.Value, connection)
            {
                CommandType = commandContent.Type
            })
            {
                if (parameters.Length > 0)
                {
                    dbCommand.Parameters.AddRange(parameters);
                }

                await connection.OpenAsync(CancellationToken.None);
                return await dbCommand.ExecuteNonQueryAsync(CancellationToken.None);
            }
        }

        public static async Task<object> ExecuteScalarAsync<TParams>(string connectionString
            , CommandContent commandContent
            , TParams tp)
        {
            var wraps = ParameterWrapsFactory.ToParameterWraps(tp);
            var parameters = wraps.Select(o => o.SqlParameter).ToArray();

            return await ExecuteScalarAsync(connectionString, commandContent, parameters);
        }

        public static async Task<object> ExecuteScalarAsync(string connectionString
            , CommandContent commandContent
            , SqlParameter[] parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var dbCommand = new SqlCommand(commandContent.Value, connection)
            {
                CommandType = commandContent.Type
            })
            {
                if (parameters.Length > 0)
                {
                    dbCommand.Parameters.AddRange(parameters);
                }

                await connection.OpenAsync(CancellationToken.None);
                return await dbCommand.ExecuteScalarAsync(CancellationToken.None);
            }
        }

        public static List<T> GenerateListByDataTable<T>(DataTable dataTable)
        {
            var results = new List<T>();

            if (dataTable == null)
            {
                return results;
            }

            int columnCount = dataTable.Columns.Count;

            if (columnCount == 0)
            {
                return results;
            }

            int rowCount = dataTable.Rows.Count;

            if (rowCount == 0)
            {
                return results;
            }

            var columns = ColumnsFactory.ToColumns(typeof(T));
            if (columns == null)
            {
                return results;
            }

            HashSet<string> fieldList = new HashSet<string>();

            foreach (DataColumn item in dataTable.Columns)
            {
                fieldList.Add(item.ColumnName.ToLower());
            }

            List<SqlColumn> sqlColumns = columns.FindAll(p => fieldList.Contains(p.Name.ToLower()));

            if (sqlColumns.Count > 0)
            {
                Dictionary<string, int> columnNameDict = new Dictionary<string, int>();
                Dictionary<int, Type> typeDict = new Dictionary<int, Type>();

                foreach (DataRow dataRow in dataTable.Rows)
                {
                    object item = Activator.CreateInstance(typeof(T), true);

                    foreach (SqlColumn column in sqlColumns)
                    {
                        int columnIndex = GetFieldIndex(columnNameDict, column.Name, dataTable);
                        Type columnType = GetFieldType(typeDict, columnIndex, dataTable);
                        object value = GetFieldValue(columnIndex, columnType, dataRow, column.DataType);
                        column.SetValue(item, value);
                    }

                    results.Add((T)item);
                }
            }

            return results;
        }

        private static int GetFieldIndex(Dictionary<string, int> columnNameDict, string columnName, IDataReader reader)
        {
            if (!columnNameDict.TryGetValue(columnName, out var index))
            {
                index = reader.GetOrdinal(columnName);
                columnNameDict.Add(columnName, index);
            }

            return index;
        }

        private static Int32 GetFieldIndex(Dictionary<string, int> dict, String columnName, DataTable dataTable)
        {
            if (!dict.TryGetValue(columnName, out var index))
            {
                index = dataTable.Columns.IndexOf(columnName);
                dict.Add(columnName, index);
            }

            return index;
        }

        private static Type GetFieldType(Dictionary<int, Type> typeDict, int columnIndex, IDataReader reader)
        {
            if (!typeDict.TryGetValue(columnIndex, out var t))
            {
                t = reader.GetFieldType(columnIndex);
                typeDict.Add(columnIndex, t);
            }

            return t;
        }

        private static Type GetFieldType(Dictionary<int, Type> dict, Int32 columnIndex, DataTable dataTable)
        {
            if (!dict.TryGetValue(columnIndex, out var t))
            {
                t = dataTable.Columns[columnIndex].DataType;
                dict.Add(columnIndex, t);
            }

            return t;
        }

        private static object GetFieldValue(int columnIndex, Type columnType, IDataReader reader, Type entityType)
        {
            if (columnIndex < 0)
            {
                return null;
            }

            if (columnType == null)
            {
                return null;
            }

            if (reader == null)
            {
                return null;
            }

            if (reader.IsDBNull(columnIndex))
            {
                return null;
            }

            object value;

            if (columnType == typeof(string))
            {
                string fieldValue = reader.GetString(columnIndex);
                if (entityType == typeof(char) || entityType == typeof(char?))
                {
                    value = fieldValue[0];
                }
                else
                {
                    value = fieldValue;
                }
            }
            else if (columnType == typeof(int))
            {
                if (entityType == typeof(short) || entityType == typeof(short?))
                {
                    value = Convert.ToInt16(reader[columnIndex]);
                }
                else
                {
                    value = reader.GetInt32(columnIndex);
                }
            }
            else if (columnType == typeof(long))
            {
                value = reader.GetInt64(columnIndex);
            }
            else if (columnType == typeof(DateTime))
            {
                value = reader.GetDateTime(columnIndex);
            }
            else if (columnType == typeof(decimal))
            {
                value = reader.GetDecimal(columnIndex);
            }
            else if (columnType == typeof(byte))
            {
                value = reader.GetByte(columnIndex);
            }
            else if (columnType == typeof(byte[]))
            {
                value = GetBytes(reader[columnIndex].ToString());
            }
            else if (columnType == typeof(short))
            {
                value = reader.GetInt16(columnIndex);
            }
            else if (columnType == typeof(char))
            {
                value = reader.GetChar(columnIndex);
            }
            else if (columnType == typeof(float))
            {
                value = reader.GetFloat(columnIndex);
            }
            else if (columnType == typeof(double))
            {
                value = reader.GetDouble(columnIndex);
            }
            else if (columnType == typeof(sbyte))
            {
                if (entityType == typeof(byte) || entityType == typeof(byte?))
                {
                    value = Convert.ToByte(reader[columnIndex]);
                }
                else
                {
                    value = sbyte.Parse(reader[columnIndex].ToString());
                }
            }
            else if (columnType == typeof(ushort))
            {
                value = ushort.Parse(reader[columnIndex].ToString());
            }
            else if (columnType == typeof(uint))
            {
                value = uint.Parse(reader[columnIndex].ToString());
            }
            else if (columnType == typeof(ulong))
            {
                value = ulong.Parse(reader[columnIndex].ToString());
            }
            else if (columnType == typeof(TimeSpan))
            {
                value = TimeSpan.Parse(reader[columnIndex].ToString());
            }
            else if (columnType == typeof(Boolean))
            {
                value = reader.GetBoolean(columnIndex);
            }
            else if (columnType == typeof(Guid))
            {
                value = reader.GetGuid(columnIndex);
            }
            else if (columnType.Name.Equals("MySqlDateTime"))
            {
                value = reader.GetDateTime(columnIndex);
            }
            else
            {
                value = reader.GetValue(columnIndex);
            }

            return value;
        }

        private static object GetFieldValue(int columnIndex, Type columnType, DataRow dataRow, Type entityType)
        {
            if (columnIndex < 0)
            {
                return null;
            }

            if (columnType == null)
            {
                return null;
            }

            if (dataRow == null)
            {
                return null;
            }

            if (dataRow.IsNull(columnIndex))
            {
                return null;
            }

            object value;

            if (columnType == typeof(string))
            {
                string fieldValue = dataRow[columnIndex].ToString();
                if (entityType == typeof(char) || entityType == typeof(char?))
                {
                    value = fieldValue[0];
                }
                else
                {
                    value = fieldValue;
                }
            }
            else if (columnType == typeof(int))
            {
                if (entityType == typeof(short) || entityType == typeof(short?))
                {
                    value = Convert.ToInt16(dataRow[columnIndex]);
                }
                else
                {
                    value = dataRow[columnIndex];
                }
            }
            else if (columnType == typeof(sbyte))
            {
                if (entityType == typeof(byte) || entityType == typeof(byte?))
                {
                    value = Convert.ToByte(dataRow[columnIndex]);
                }
                else
                {
                    value = dataRow[columnIndex];
                }
            }
            else if (columnType.Name.Equals("MySqlDateTime"))
            {
                value = Convert.ToDateTime(dataRow[columnIndex]);
            }
            else
            {
                value = dataRow[columnIndex];
            }

            return value;
        }

        private static Byte[] GetBytes(String value)
        {
            byte[] bytes = new byte[value.Length * sizeof(char)];
            Buffer.BlockCopy(value.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }

    public class CommandContent
    {
        public CommandType Type{ get; set; }
        public string Value { get; set; }
    }

}
