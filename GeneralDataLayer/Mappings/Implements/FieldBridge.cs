using System;
using System.Reflection;
using GeneralDataLayer.Dynamics.Interfaces;
using GeneralDataLayer.Mappings.Interfaces;

namespace GeneralDataLayer.Mappings.Implements
{
    public class FieldBridge : IDataBridge
    {
        private FieldInfo _field;

        private IDynamicDataInfo _sor;
        public FieldBridge(FieldInfo field)
        {
            _field = field;
            _sor = ReflectionManager.CreateDynamicInfo(field);
        }

        public Type DataType => _field.FieldType;

        public object Read(object obj)
        {
            //return _field.GetValue(obj);
            return _sor.GetValue(obj);
        }

        public void Write(object obj, object val)
        {
            //_field.SetValue(obj, val);
            _sor.SetValue(obj, val);
        }
    }
}
