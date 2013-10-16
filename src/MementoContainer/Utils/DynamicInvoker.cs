using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer.Utils
{
    /// <summary>
    /// Wraps an object and performs calls on that object using reflection.
    /// Simple dynamic method calls don't work if the object's type is internal (or nested private) to other assemblies (e.g., otherAssemblyObj.Do() will fail).
    /// <para></para>
    /// To overcome this problem, clients can make dynamic calls on this wrapper,
    /// and those calls will be translated to reflection invocations, which work on objects of any given type, regardless of their origin or access modifiers
    /// </summary>
    internal class DynamicInvoker : DynamicObject
    {
        private readonly object _obj;
        private readonly Type _objType;

        public DynamicInvoker(object obj)
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
                //Let TargetInvocationException (thrown by the called method) bubble up
                try
                {
                    result = method.Invoke(_obj, args);
                    return true;
                }
                catch (ArgumentException)
                {
                }
                catch (TargetParameterCountException)
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
