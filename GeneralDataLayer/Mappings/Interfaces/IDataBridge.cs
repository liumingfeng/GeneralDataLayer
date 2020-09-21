using System;

namespace GeneralDataLayer.Mappings.Interfaces
{
    public interface IDataBridge
    {
        object Read(object obj);
        void Write(object obj, object val);
        Type DataType { get; }
    }
}
