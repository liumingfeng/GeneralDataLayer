using System;
using System.Collections.Concurrent;
using System.Reflection;
using GeneralDataLayer.Dynamics.Interfaces;

namespace GeneralDataLayer.Dynamics.Implements
{
    internal delegate void DynamicPropertySetHandler(object obj, object value);

    internal delegate object DynamicPropertyGetHandler(object obj);

    internal class DynamicPropertyInfo : IDynamicDataInfo
    {

        Type _type;

        PropertyInfo _info;

        DynamicPropertySetHandler _setHandler;

        DynamicPropertyGetHandler _getHandler;

        public DynamicPropertyInfo(PropertyInfo info)
        {
            this._type = info.DeclaringType;

            this._info = info;
        }

        public object GetValue(object obj)
        {
            if (this._getHandler != null)
            {
                return this._getHandler(obj);
            }

            int moduleKey = _info.Module.GetHashCode();
            int handlerKey = _info.MetadataToken;

            this._getHandler = DynamicCacheFactory<DynamicPropertyGetHandler>
                .DictInstance
                .GetOrAdd(moduleKey, innerModuleKey => new ConcurrentDictionary<int, DynamicPropertyGetHandler>())
                .GetOrAdd(handlerKey, innerHandlerKey => DynamicMethodFactory.CreateGetHandler(_type, _info));

            return this._getHandler(obj);
        }

        public void SetValue(object obj, object value)
        {
            if (this._setHandler != null)
            {
                this._setHandler(obj, value);

                return;
            }

            int moduleKey = _info.Module.GetHashCode();
            int handlerKey = _info.MetadataToken;

            this._setHandler = DynamicCacheFactory<DynamicPropertySetHandler>
                .DictInstance
                .GetOrAdd(moduleKey, innerModuleKey => new ConcurrentDictionary<int, DynamicPropertySetHandler>())
                .GetOrAdd(handlerKey, innerHandlerKey => DynamicMethodFactory.CreateSetHandler(_type, _info));

            this._setHandler(obj, value);
        }
    }
}
