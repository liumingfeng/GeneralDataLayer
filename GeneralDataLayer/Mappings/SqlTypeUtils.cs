using System;
using System.Collections;
using System.Data;

namespace GeneralDataLayer.Mappings
{
    internal static class SqlTypeUtils
    {
        private static readonly Hashtable _typeMap;
        static SqlTypeUtils()
        {
            _typeMap = new Hashtable(17, 1f);
            _typeMap.Add(typeof(string), DbType.String);
            _typeMap.Add(typeof(int), DbType.Int32);
            _typeMap.Add(typeof(bool), DbType.Boolean);
            _typeMap.Add(typeof(DateTime), DbType.DateTime);
            _typeMap.Add(typeof(double), DbType.Double);
            _typeMap.Add(typeof(long), DbType.Int64);
            _typeMap.Add(typeof(short), DbType.Int16);
            _typeMap.Add(typeof(byte), DbType.Byte);
            _typeMap.Add(typeof(char), DbType.StringFixedLength);
            _typeMap.Add(typeof(decimal), DbType.Decimal);
            _typeMap.Add(typeof(float), DbType.Single);
            _typeMap.Add(typeof(uint), DbType.UInt32);
            _typeMap.Add(typeof(ulong), DbType.UInt64);
            _typeMap.Add(typeof(ushort), DbType.UInt16);
            _typeMap.Add(typeof(sbyte), DbType.SByte);
            _typeMap.Add(typeof(Guid), DbType.Guid);
            _typeMap.Add(typeof(byte[]), DbType.Binary);
            _typeMap.Add(typeof(TimeSpan), DbType.Time);
            _typeMap.Add(typeof(DateTimeOffset), DbType.DateTimeOffset);
        }
        
        internal static DbType ResolveType(Type type)
        {
            if (type != null)
            {
                Type t = type;
                if (t.IsGenericType && t.IsValueType)
                {
                    Type[] genericTypes = t.GetGenericArguments();
                    if (genericTypes.Length > 0)
                        t = genericTypes[0];
                }
                if (_typeMap.ContainsKey(t))
                    return (DbType)_typeMap[t];

            }
            return DbType.Object;
        }
    }
}
