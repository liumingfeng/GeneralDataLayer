using System;
using System.Collections.Concurrent;
using System.Reflection;
using GeneralDataLayer.Dynamics.Interfaces;

namespace GeneralDataLayer.Dynamics.Implements
{
    internal delegate object DynamicFieldGetHandler(object obj);

    internal delegate void DynamicFieldSetHandler(object obj, object value);

    internal class DynamicFieldInfo : IDynamicDataInfo
    {
        Type _type;

        FieldInfo _info;

        DynamicFieldSetHandler _setHandler;

        DynamicFieldGetHandler _getHandler;

        public DynamicFieldInfo(FieldInfo info)
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

            this._getHandler = DynamicCacheFactory<DynamicFieldGetHandler>
                .DictInstance
                .GetOrAdd(moduleKey, innerModuleKey => new ConcurrentDictionary<int, DynamicFieldGetHandler>())
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

            this._setHandler = DynamicCacheFactory<DynamicFieldSetHandler>
                .DictInstance
                .GetOrAdd(moduleKey, innerModuleKey => new ConcurrentDictionary<int, DynamicFieldSetHandler>())
                .GetOrAdd(handlerKey, innerHandlerKey => DynamicMethodFactory.CreateSetHandler(_type, _info));

            this._setHandler(obj, value);
        }
    }
}
