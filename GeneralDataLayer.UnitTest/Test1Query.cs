using System;
using System.Data;
using System.Globalization;
using GeneralDataLayer.Mappings;

namespace GeneralDataLayer.UnitTest
{
    public class Test1Query
    {
        [Parameter(Name = "@Id")]
        public long Id { get; set; }
    }
}