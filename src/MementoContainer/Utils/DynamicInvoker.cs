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

            if (!TryInvokeMember(binder, args, out result, methods))
            {
                var interfaceMethods = _objType.GetTypeInfo()
                                               .ImplementedInterfaces
                                               .SelectMany(intf => intf.GetRuntimeMethods())
                                               .Where(m => m.Name == binder.Name);
                return TryInvokeMember(binder, args, out result, interfaceMethods);
            }

            return true;
        }

        private bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result,
                                     IEnumerable<MethodInfo> methods)
        {
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
            //return wrapped object
            if (binder.Name == "InnerObject")
            {
                result = _obj;
                return true;
            }

            var property = FindProperty(binder.Name);

            if (property != null)
            {
                result = property.GetValue(_obj);
                return true;
            }

            result = null;
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var property = FindProperty(binder.Name);

            if (property == null)
            {
                return false;
            }

            property.SetValue(_obj, value);
            return true;
        }

        /// <summary>
        /// Try to find the property in the inner object's type.
        /// If it can't be found, it inspects the interfaces for explicitly implemented properties.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>The requested property.</returns>
        private PropertyInfo FindProperty(string name)
        {
            return _objType
                       .GetRuntimeProperty(name) ??
                   _objType.GetTypeInfo()
                           .ImplementedInterfaces
                           .Select(intf => intf.GetRuntimeProperty(name))
                           .FirstOrDefault(m => m != null);
        }
    }
}
