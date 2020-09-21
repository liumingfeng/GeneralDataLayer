using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GeneralDataLayer.Mappings;
using GeneralDataLayer.Mappings.Implements;

namespace GeneralDataLayer
{
    public static class ExecutionHelper
    {
        public static async Task<TReturn> ExecuteReaderAsync<TParams, TReturn>(string connectionString
            , string storedProcedure
            , TParams tp)
        {
            var wraps = ParameterWrapsFactory.ToParameterWraps(tp);
            var parameters = wraps.Select(o => o.SqlParameter).ToArray();

            return await ExecuteReaderAsync<TReturn>(connectionString, storedProcedure, parameters);
        }

        public static async Task<T> ExecuteReaderAsync<T>(string connectionString
            , string storedProcedure
            , SqlParameter[] parameters = null)
        {
            var list = await ExecuteReaderListAsync<T>(connectionString, storedProcedure, parameters);
            return list.FirstOrDefault();
        }

        public static async Task<List<TReturn>> ExecuteReaderListAsync<TParams, TReturn>(string connectionString
            , string storedProcedure
            , TParams tp)
        {
            var wraps = ParameterWrapsFactory.ToParameterWraps(tp);
            var parameters = wraps.Select(o => o.SqlParameter).ToArray();

            return await ExecuteReaderListAsync<TReturn>(connectionString, storedProcedure, parameters);
        }

        public static async Task<List<T>> ExecuteReaderListAsync<T>(string connectionString
            , string storedProcedure
            , SqlParameter[] parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var dbCommand = new SqlCommand(storedProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
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

        public static async Task ExecuteNonQueryAsync<T>(string connectionString
            , string storedProcedure
            , T entity)
        {
            var wraps = ParameterWrapsFactory.ToParameterWraps(entity);

            var parameters = wraps.Select(o => o.SqlParameter).ToArray();

            await ExecuteNonQueryAsync(connectionString, storedProcedure, parameters);


            var outputWraps = wraps.FindAll(p => p.SqlParameter.Direction == ParameterDirection.Output 
                                                      || p.SqlParameter.Direction == ParameterDirection.InputOutput
                                                      || p.SqlParameter.Direction == ParameterDirection.ReturnValue);

            foreach(var outputWrap in outputWraps)
            {
                outputWrap.DataBridge.Write(entity, outputWrap.SqlParameter.Value);
            }
        }

        public static async Task ExecuteNonQueryAsync(string connectionString
            , string storedProcedure
            , SqlParameter[] parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var dbCommand = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                if (parameters.Length > 0)
                {
                    dbCommand.Parameters.AddRange(parameters);
                }

                await connection.OpenAsync(CancellationToken.None);
                await dbCommand.ExecuteNonQueryAsync(CancellationToken.None);
            }
        }

        public static async Task<object> ExecuteScalarAsync<TParams>(string connectionString
            , string storedProcedure
            , TParams tp)
        {
            var wraps = ParameterWrapsFactory.ToParameterWraps(tp);
            var parameters = wraps.Select(o => o.SqlParameter).ToArray();

            return await ExecuteScalarAsync(connectionString, storedProcedure, parameters);
        }

        public static async Task<object> ExecuteScalarAsync(string connectionString
            , string storedProcedure
            , SqlParameter[] parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var dbCommand = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
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

        private static int GetFieldIndex(Dictionary<String, Int32> columnNameDict, string columnName, IDataReader reader)
        {
            int index = -1;

            if (!columnNameDict.TryGetValue(columnName, out index))
            {
                index = reader.GetOrdinal(columnName);
                columnNameDict.Add(columnName, index);
            }

            return index;
        }

        private static Type GetFieldType(Dictionary<Int32, Type> typeDict, int columnIndex, IDataReader reader)
        {
            Type t = null;

            if (!typeDict.TryGetValue(columnIndex, out t))
            {
                t = reader.GetFieldType(columnIndex);
                typeDict.Add(columnIndex, t);
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
                    value = SByte.Parse(reader[columnIndex].ToString());
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
                if (entityType == typeof(bool) || entityType == typeof(bool?))
                {
                    value = Convert.ToBoolean(reader[columnIndex]);
                }
                else
                {
                    value = ulong.Parse(reader[columnIndex].ToString());
                }
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

        private static Byte[] GetBytes(String value)
        {
            byte[] bytes = new byte[value.Length * sizeof(char)];
            Buffer.BlockCopy(value.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
