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

        Type type;

        PropertyInfo info;

        DynamicPropertySetHandler setHandler;

        DynamicPropertyGetHandler getHandler;

        public DynamicPropertyInfo(PropertyInfo info)
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

            this.getHandler = DynamicCacheFactory<DynamicPropertyGetHandler>
                .DictInstance
                .GetOrAdd(moduleKey, innerModuleKey => new ConcurrentDictionary<int, DynamicPropertyGetHandler>())
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

            this.setHandler = DynamicCacheFactory<DynamicPropertySetHandler>
                .DictInstance
                .GetOrAdd(moduleKey, innerModuleKey => new ConcurrentDictionary<int, DynamicPropertySetHandler>())
                .GetOrAdd(handlerKey, innerHandlerKey => DynamicMethodFactory.CreateSetHandler(type, info));

            this.setHandler(obj, value);
        }
    }
}
