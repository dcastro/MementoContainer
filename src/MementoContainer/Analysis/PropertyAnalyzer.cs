using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MementoContainer.Adapters;
using MementoContainer.Utils;

namespace MementoContainer.Analysis
{
    internal class PropertyAnalyzer : BaseAnalyzer, IPropertyAnalyzer
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
            var props = new List<IPropertyAdapter>();

            while (expression != null)
            {
                //validate expression
                bool isDoneAnalyzing;
                ValidateExpression(ref expression, hasParameter, out isDoneAnalyzing);

                if (isDoneAnalyzing)
                    break;

                var memberExp = expression as MemberExpression;
                var prop = memberExp.Member as PropertyInfo;

                props.Insert(0, Wrap(prop));

                expression = memberExp.Expression;
            }

            //If the expression has a parameter (i.e., should map to an object's property)
            //then the remaining expression must contain the parameter.
            if (hasParameter && expression == null)
                throw InvalidExpressionException.UnexpectedOperation;

            return props;
        }

        /// <summary>
        /// Validates an expression and returns whether the expression has been fully analyzed
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="hasParameter"></param>
        /// <param name="isDoneAnalyzing"></param>
        private void ValidateExpression(ref Expression expression, bool hasParameter, out bool isDoneAnalyzing)
        {
            isDoneAnalyzing = false;

            //validate node type
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    break;
                case ExpressionType.TypeAs:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var unaryExpression = expression as UnaryExpression;
                    expression = ((unaryExpression != null) ? unaryExpression.Operand : null);
                    break;
                case ExpressionType.Parameter:
                    if (hasParameter)
                        isDoneAnalyzing = true;
                    else
                        throw InvalidExpressionException.UnexpectedOperation;
                    break;
                case ExpressionType.Call:
                    throw InvalidExpressionException.MethodFound;
                default:
                    throw InvalidExpressionException.UnexpectedOperation;
            }

            //further validation is needed
            if (!isDoneAnalyzing)
            {
                //validate expression type
                if (!(expression is MemberExpression))
                    throw InvalidExpressionException.UnexpectedOperation;

                //validate member type
                var memberExp = expression as MemberExpression;
                if (memberExp.Member is FieldInfo)
                    throw InvalidExpressionException.FieldFound;
            }
        }

        public IList<Tuple<IPropertyAdapter, bool>> GetProperties(object obj)
        {
            Type type = obj.GetType();

            //check if type has MementoClassAttribute
            if (type.IsMementoClass())
            {
                var typeInfo = type.GetTypeInfo();
                var mementoClassAttr = typeInfo.GetCustomAttribute<MementoClassAttribute>();

                return typeInfo.DeclaredProperties
                               .Where(p => p.HasGetAndSet())
                               .Select(p => Tuple.Create(p, GetCascade(mementoClassAttr)))
                               .Select(pair => Tuple.Create(Wrap(pair.Item1), pair.Item2))
                               .ToList();
            }

            var attributesMap = type.GetFullAttributesMap();
            return attributesMap
                .Where(kv => kv.Value.Any(attr => attr is MementoPropertyAttribute))
                .Select(kv => kv.Key)
                .Select(prop => Tuple.Create(prop, GetCascade(attributesMap[prop])))
                .Select(pair => Tuple.Create(Wrap(pair.Item1), pair.Item2))
                .ToList();
        }

        private bool GetCascade(IEnumerable<Attribute> attrs)
        {
            var collectionAttr = ((MementoPropertyAttribute)attrs.First(attr => attr is MementoPropertyAttribute));
            return collectionAttr.Cascade;
        }

        /// <summary>
        /// Validates and wraps a property in an adapter
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private IPropertyAdapter Wrap(PropertyInfo property)
        {
            if (!property.HasGetAndSet())
                throw PropertyException.MissingAccessors(property);
            return new PropertyInfoAdapter(property);
        }
    }
}
