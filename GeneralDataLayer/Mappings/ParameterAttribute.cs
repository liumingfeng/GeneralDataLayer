using System;
using System.Data;

namespace GeneralDataLayer.Mappings
{
    public class ParameterAttribute : Attribute
    {
        public string Name { get; set; }

        public DbType DbType
        {
            get; set;
        }

        public ParameterDirection Direction { get; set; }
    }
}
