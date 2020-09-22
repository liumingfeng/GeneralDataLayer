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
        Type type;

        FieldInfo info;

        DynamicFieldSetHandler setHandler;

        DynamicFieldGetHandler getHandler;

        public DynamicFieldInfo(FieldInfo info)
        {
            this.type = info.DeclaringType;

            this.info = info;
        }

        public object GetValue(object obj)
        {
            if (this.getHandler != null)
            {
                return this.getHandler(obj);
            }

            int moduleKey = info.Module.GetHashCode();
            int handlerKey = info.MetadataToken;

            this.getHandler = DynamicCacheFactory<DynamicFieldGetHandler>
                .DictInstance
                .GetOrAdd(moduleKey, innerModuleKey => new ConcurrentDictionary<int, DynamicFieldGetHandler>())
                .GetOrAdd(handlerKey, innerHandlerKey => DynamicMethodFactory.CreateGetHandler(type, info));

            return this.getHandler(obj);
        }

        public void SetValue(object obj, object value)
        {
            if (this.setHandler != null)
            {
                this.setHandler(obj, value);

                return;
            }

            int moduleKey = info.Module.GetHashCode();
            int handlerKey = info.MetadataToken;

            this.setHandler = DynamicCacheFactory<DynamicFieldSetHandler>
                .DictInstance
                .GetOrAdd(moduleKey, innerModuleKey => new ConcurrentDictionary<int, DynamicFieldSetHandler>())
                .GetOrAdd(handlerKey, innerHandlerKey => DynamicMethodFactory.CreateSetHandler(type, info));

            this.setHandler(obj, value);
        }
    }
}
