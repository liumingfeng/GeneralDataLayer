using System;
using System.Data;
using GeneralDataLayer.Mappings.Interfaces;

namespace GeneralDataLayer.Mappings.Implements
{
    public class SqlColumn : IColumn
    {
        public string Name { get; }
        private IDataBridge Data { get; }
        public Type DataType => Data.DataType;

        public SqlColumn(string name, IDataBridge data)
        {
            Name = name.ToLower();
            Data = data;
        }

        public void SetValue(object obj, object val)
        {
            try
            {
                if (DBNull.Value.Equals(val))
                    Data.Write(obj, null);
                else
                    Data.Write(obj, val);
            }
            catch (Exception ex)
            {
                throw new Exception("Property is" + Name + " and set " + val +" as the value failed.", ex);
            }
        }
    }
}