using GeneralDataLayer.Mappings.Interfaces;
using System.Data.SqlClient;

namespace GeneralDataLayer.Mappings
{
    public class ParameterWrap
    {
        public SqlParameter SqlParameter { get; set; }
        public IDataBridge DataBridge { get; set; }
    }
}
