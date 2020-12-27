using System;
using System.Reflection;
using NHibernate.Properties;

namespace Hamster.Plugin.NHibernate
{
    public class DataObjectAccessor : IPropertyAccessor
    {
        public bool CanAccessThroughReflectionOptimizer => false;

        private class DataObjectGetter : IGetter
        {
            private string property;

            public MethodInfo Method => null;
            public string PropertyName => null;
            public Type ReturnType => typeof(object);

            public DataObjectGetter(string property)
            {
                this.property = property;
            }

            public object Get(object target)
            {
                var o = (IDataObject)target;
                return o.HasProperty(property) ? o.GetProperty(property) : null;
            }

            public object GetForInsert(object owner, System.Collections.IDictionary mergeMap, global::NHibernate.Engine.ISessionImplementor session)
            {
                return Get(owner);
            }
        };

        private class DataObjectSetter : ISetter
        {
            public MethodInfo Method => null;
            public string PropertyName => null;

            private string property;

            public DataObjectSetter(string property)
            {
                this.property = property;
            }


            public void Set(object target, object value)
            {
                ((IDataObject)target).SetProperty(property, value);
            }
        }

        public IGetter GetGetter(Type theClass, string propertyName)
        {
            return new DataObjectGetter(propertyName);
        }

        public ISetter GetSetter(Type theClass, string propertyName)
        {
            return new DataObjectSetter(propertyName);
        }


    }
}
