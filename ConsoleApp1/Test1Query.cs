using GeneralDataLayer.Mappings;

namespace ConsoleApp1
{
    public class Test1Query
    {
        [Parameter(Name = "@Id")]
        public long Id { get; set; }
    }
}