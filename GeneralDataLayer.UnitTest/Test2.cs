using System;
using System.Globalization;
using GeneralDataLayer.Mappings;

namespace GeneralDataLayer.UnitTest
{
    public class Test2
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public bool IsActive { get; set; }

        [Column(Name = "CreateAt")]
        public DateTime CreateDateTime { get; set; }

        public double DoubleValue { get; set; }

        [Column]
        public Guid? MyGuid { get; set; }

        public Int16 Byte { get; set; }

        public string Char { get; set; }

        [Column]
        public decimal Amt { get; set; }

        public byte[] Binary { get; set; }

        public DateTimeOffset? DateTimeOffset { get; set; }

        public int? ReturnData { get; set; }

        public override string ToString()
        {
            return Id + "," + Name + "," + Age + "," + IsActive + "," + CreateDateTime.ToLongDateString() + "," + DoubleValue.ToString(CultureInfo.InvariantCulture) + "," + Byte + "," + Char + "," + Amt.ToString(CultureInfo.InvariantCulture);
        }
    }
}
