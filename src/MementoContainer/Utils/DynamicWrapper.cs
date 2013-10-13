using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer.Utils
{
    internal class DynamicWrapper : DynamicObject
    {
        private readonly object _obj;
        private readonly Type _objType;

        public DynamicWrapper(object obj)
        {
            _obj = obj;
            _objType = _obj.GetType();
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var methods = _objType
                .GetRuntimeMethods()
                .Where(m => m.Name == binder.Name);
            
            foreach (var method in methods)
            {
                try
                {
                    result = method.Invoke(_obj, args);
                    return true;
                }
                catch (ArgumentException)
                {
                }
                catch (TargetInvocationException)
                {
                }
                catch (TargetParameterCountException)
                {
                }
                catch (InvalidOperationException)
                {
                }
                catch (NotSupportedException)
                {
                }
            }

            result = null;
            return false;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var property = _objType
                .GetRuntimeProperty(binder.Name);

            if (property == null)
            {
                result = null;
                return false;
            }

            result = property.GetValue(_obj);
            return true;
        }
    }
}
