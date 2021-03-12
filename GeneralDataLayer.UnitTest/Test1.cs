using System;
using System.Data;
using System.Globalization;
using GeneralDataLayer.Mappings;

namespace GeneralDataLayer.UnitTest
{
    public class Test1
    {
        [Parameter(Name = "@NewId", Direction = ParameterDirection.Output)]
        public long Id { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public int Age { get; set; }

        [Parameter]
        public bool IsActive { get; set; }

        [Column(Name = "CreateAt")]
        [Parameter(Name = "CreateAt")]
        public DateTime CreateDateTime { get; set; }

        [Parameter]
        public double DoubleValue { get; set; }

        [Column]
        [Parameter]
        public Guid? MyGuid { get; set; }

        [Parameter]
        public Int16 Byte { get; set; }

        [Parameter]
        public string Char { get; set; }

        [Column]
        [Parameter]
        public decimal Amt { get; set; }

        [Column]
        [Parameter]
        public byte[] Binary { get; set; }

        [Column]
        [Parameter]
        public DateTimeOffset? DateTimeOffset { get; set; }

        [Parameter(Direction = ParameterDirection.ReturnValue)]
        public int? ReturnData { get; set; }

        public override string ToString()
        {
            return Id + "," + Name + "," + Age + "," + IsActive + "," + CreateDateTime.ToLongDateString() + "," + DoubleValue.ToString(CultureInfo.InvariantCulture) + "," + Byte + "," + Char + "," + Amt.ToString(CultureInfo.InvariantCulture);
        }
    }
}
