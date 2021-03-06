﻿using System;
using System.Reflection;
using GeneralDataLayer.Dynamics.Interfaces;
using GeneralDataLayer.Mappings.Interfaces;

namespace GeneralDataLayer.Mappings.Implements
{
    public class PropertyBridge : IDataBridge
    {
        private PropertyInfo _propertyInfo;

        /// <summary>
        /// Dynamic + emit
        /// </summary>
        private IDynamicDataInfo _sor;

        public PropertyBridge(PropertyInfo propertyInfo)
        {
            this._propertyInfo = propertyInfo;
            _sor = ReflectionManager.CreateDynamicInfo(propertyInfo);
        }

        public Type DataType => _propertyInfo.PropertyType;

        public object Read(object obj)
        {
            //return _propertyInfo.GetValue(obj);
            return _sor.GetValue(obj);
        }

        public void Write(object obj, object val)
        {
            //_propertyInfo.SetValue(obj, val);
            _sor.SetValue(obj, val);
        }
    }
}
