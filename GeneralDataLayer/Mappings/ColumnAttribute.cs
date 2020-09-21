using System;
using System.Data;

namespace GeneralDataLayer.Mappings
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }

        public DbType DbType
        {
            get; set;
        }
    }
}
