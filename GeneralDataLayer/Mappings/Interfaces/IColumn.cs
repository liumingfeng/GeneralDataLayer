using System;

namespace GeneralDataLayer.Mappings.Interfaces
{
    public interface IColumn
    {
        string Name { get; }
        Type DataType { get; }
    }
}
