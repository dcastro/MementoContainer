
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using MementoContainer.Adapters;
using MementoContainer.Attributes;

namespace MementoContainer.Utils
{
    internal class PropertyAnalyzer : IPropertyAnalyzer
    {
        public IList<IPropertyAdapter> GetProperties<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> expression)
        {
            return GetProperties(expression.Body, true);
        }

        public IList<IPropertyAdapter> GetProperties<TProperty>(Expression<Func<TProperty>> expression)
        {
            return GetProperties(expression.Body, false);
        }

        private IList<IPropertyAdapter> GetProperties(Expression expression, bool hasParameter)
        {
            var props = new List<PropertyInfo>();

            while(expression != null && expression.NodeType == ExpressionType.MemberAccess)
            {
                var memberExp = expression as MemberExpression;

                if(memberExp.Member is FieldInfo)
                    throw InvalidExpressionException.FieldFound;

                var prop = memberExp.Member as PropertyInfo;
                props.Insert(0, prop);

                expression = memberExp.Expression;
            }

            //If the expression has no parameters (i.e., should begin with a static property)
            //then the whole expression should have been consumed (i.e., expression == null).
            //If the expression has a parameter (i.e., should map to an instance property)
            //then the remaining expression must contain the parameter.
            if (expression != null)
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.Parameter:
                        if (! hasParameter)
                            throw InvalidExpressionException.UnexpectedOperation;
                        break;
                    case ExpressionType.Call:
                        throw InvalidExpressionException.MethodFound;
                    default:
                        throw InvalidExpressionException.UnexpectedOperation;
                }
            }
            else if (hasParameter)
                throw InvalidExpressionException.UnexpectedOperation;

            return ValidateProperties(props);
        }

        public IList<IPropertyAdapter> GetProperties(object obj)
        {
            Type type = obj.GetType();
            var props = type.GetTypeInfo().DeclaredProperties;

            //Return all valid properties
            if (type.GetTypeInfo().IsDefined(typeof(MementoClassAttribute)))
            {
                var validProps = props.Where(p => p.HasGetAndSet()).ToList();
                return WrapProperties(validProps);
            }

            //Return properties with the Memento attribute
            var mementoProps = props.Where(p => p.IsDefined(typeof(MementoPropertyAttribute))).ToList();
            return ValidateProperties(mementoProps);
        }

        /// <summary>
        /// Validates and wraps all properties in an adapter.
        /// </summary>
        /// <param name="properties">A group of properties being validated and wrapped.</param>
        /// <returns>A group of properties wrapped in the IPropertyAdapter interface.</returns>
        private IList<IPropertyAdapter> ValidateProperties(IEnumerable<PropertyInfo> properties)
        {
            var adapters = new List<IPropertyAdapter>();

            foreach (var prop in properties)
            {
                if(! prop.HasGetAndSet())
                    throw new PropertyException(prop);
                adapters.Add(new PropertyInfoAdapter(prop));
            }

            return adapters;
        }

        /// <summary>
        /// Wraps all properties in an adapter.
        /// </summary>
        /// <param name="properties">A group of properties being wrapped.</param>
        /// <returns>A group of properties wrapped in the IPropertyAdapter interface.</returns>
        private IList<IPropertyAdapter> WrapProperties(IEnumerable<PropertyInfo> properties)
        {
            return properties
                .Select(prop => new PropertyInfoAdapter(prop))
                .Cast<IPropertyAdapter>()
                .ToList();
        }
    }
}
