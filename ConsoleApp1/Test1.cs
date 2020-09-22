using GeneralDataLayer.Mappings;
using System;
using System.Data;
using System.Globalization;

namespace ConsoleApp1
{
    public class Test1
    {
        [Column]
        [Parameter(Name = "@NewId", Direction = ParameterDirection.Output)]
        public long Id { get; set; }

        [Column]
        [Parameter]
        public string Name { get; set; }

        [Column]
        [Parameter]
        public int Age { get; set; }

        [Column]
        [Parameter]
        public bool IsActive { get; set; }

        [Column(Name = "CreateAt")]
        [Parameter(Name = "CreateAt")]
        public DateTime CreateDateTime { get; set; }

        [Column]
        [Parameter]
        public double DoubleValue { get; set; }

        [Column]
        [Parameter]
        public Guid? MyGuid { get; set; }

        [Column]
        [Parameter]
        public Int16 Byte { get; set; }

        [Column]
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
