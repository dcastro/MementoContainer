using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MementoContainer.Domain;
using MementoContainer.Utils;

namespace MementoContainer.Analysis
{
    /// <summary>
    /// Default implementation of IPropertyAnalyzer to analyzes certain inputs (e.g., expressions and objects) and look for properties to be registered in the memento.
    /// </summary>
    public class PropertyAnalyzer : IPropertyAnalyzer
    {
        delegate bool PropertyFilter(MementoClassAttribute mementoClassAttr, PropertyInfo pi, IList<Attribute> propertyAttrs);
        private readonly PropertyFilter DefaultFilter = (mementoClassAttr, pi, propertyAttrs) =>
        mementoClassAttr != null
            ? propertyAttrs.Any(attr => attr is MementoPropertyAttribute) && pi.HasGetAndSet()
            : propertyAttrs.Any(attr => attr is MementoPropertyAttribute);

        PropertyFilter Filter { get; }

        /// <summary>
        /// Gets property adapters for the properties given an expression 
        /// </summary>
        /// <typeparam name="TOwner">Owner class type</typeparam>
        /// <typeparam name="TProperty">Property type</typeparam>
        /// <param name="expression">Expression returning property to be analyzed</param>
        /// <returns>Property adapters </returns>
        public IList<IPropertyAdapter> GetProperties<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> expression)
        {
            return GetProperties(expression.Body, true);
        }

        /// <summary>
        /// Gets property adapters for the properties given an expression 
        /// </summary>
        /// <typeparam name="TProperty">Property type</typeparam>
        /// <param name="expression">Expression returning property to be analyzed</param>
        /// <returns>Property adapters </returns>
        public IList<IPropertyAdapter> GetProperties<TProperty>(Expression<Func<TProperty>> expression)
        {
            return GetProperties(expression.Body, false);
        }

        private IList<IPropertyAdapter> GetProperties(Expression expression, bool hasParameter)
        {
            var props = new List<PropertyInfo>();

            while (expression != null)
            {
                //validate expression
                bool isDoneAnalyzing;
                ValidateExpression(ref expression, hasParameter, out isDoneAnalyzing);

                if (isDoneAnalyzing)
                    break;

                var memberExp = expression as MemberExpression;
                var prop = memberExp.Member as PropertyInfo;

                props.Insert(0, prop);

                expression = memberExp.Expression;
            }

            //If the expression has a parameter (i.e., should map to an object's property)
            //then the remaining expression must contain the parameter.
            if (hasParameter && expression == null)
                throw InvalidExpressionException.UnexpectedOperation;

            Validate(props);

            return props.Select(Wrap).ToList();
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

        /// <summary>
        /// Gets data for all the properties in a given object which match the requisites
        /// </summary>
        /// <param name="obj">Object to analyze</param>
        /// <returns>Property data of the selected properties</returns>
        public virtual IList<IPropertyData> GetProperties(object obj)
        {
            Type type = obj.GetType();
            IDictionary<PropertyInfo, IList<Attribute>> attributesMap = type.GetFullAttributesMap();

            //check if type has MementoClassAttribute
            if (type.IsMementoClass())
            {
                var typeInfo = type.GetTypeInfo();
                var mementoClassAttr = typeInfo.GetCustomAttribute<MementoClassAttribute>();

                return attributesMap
                    .Where(PropertySelectorForMementoClass)
                    .Select(kv => kv.Key)
                    .Select(Validate)
                    .Select(prop => new PropertyData(prop, obj, attributesMap[prop], mementoClassAttr))
                    .Cast<IPropertyData>()
                    .ToList();
            }

            return attributesMap
                .Where(PropertySelectorForNonMementoClass)
                .Select(kv => kv.Key)
                .Select(Validate)
                .Select(prop => new PropertyData(prop, obj, attributesMap[prop]))
                .Cast<IPropertyData>()
                .ToList();
        }

        /// <summary>
        /// Tells if a given property has to be registered in the memento in case the class is decorated with MementoClassAttribute
        /// </summary>
        /// <param name="kv">Property info and its attributes</param>
        /// <returns>True if the property must be registered in the memento</returns>
        protected virtual bool PropertySelectorForMementoClass(KeyValuePair<PropertyInfo, IList<Attribute>> kv)
        {
            return kv.Value.Any(attr => attr is MementoPropertyAttribute) || kv.Key.PropertyType.IsCollection();
        }

        /// <summary>
        /// Tells if a given property has to be registered in the memento in case the class is not decorated with MementoClassAttribute
        /// </summary>
        /// <param name="kv">Property info and its attributes</param>
        /// <returns>True if the property must be registered in the memento</returns>
        protected virtual bool PropertySelectorForNonMementoClass(KeyValuePair<PropertyInfo, IList<Attribute>> kv)
        {
            return kv.Value.Any(attr => attr is MementoPropertyAttribute);
        }

        /// <summary>
        /// Wraps a property in an adapter
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private IPropertyAdapter Wrap(PropertyInfo property)
        {
            return new PropertyInfoAdapter(property);
        }

        private PropertyInfo Validate(PropertyInfo property)
        {
            if (!property.HasGetAndSet())
                throw PropertyException.MissingAccessors(property);
            return property;
        }

        /// <summary>
        /// Validates a chain of properties.
        /// All properties must be readable, and the last property must be writable.
        /// </summary>
        /// <param name="props"></param>
        private void Validate(IList<PropertyInfo> props)
        {
            var cantBeRead = props.FirstOrDefault(p => !p.CanRead);
            if (cantBeRead != null)
                throw PropertyException.MissingGetAccessor(cantBeRead);

            var last = props.Last();

            if(! last.CanWrite)
                throw PropertyException.MissingSetAccessor(last);
        } 
    }
}
