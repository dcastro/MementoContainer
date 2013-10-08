﻿
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

            while (expression != null)
            {
                //validate expression
                bool isDoneAnalyzing;
                ValidateExpression(ref expression, hasParameter, out isDoneAnalyzing);

                if(isDoneAnalyzing)
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

            return ValidateProperties(props);
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

        public IList<IPropertyAdapter> GetProperties(object obj)
        {
            Type type = obj.GetType();
            TypeInfo typeInfo = type.GetTypeInfo();
            var props = type.GetRuntimeProperties();
            

            //check if type has MementoClassAttribute
            if (IsMementoClass(typeInfo))
            {
                return WrapProperties(
                    LookupProperties(typeInfo, true));
            }

            //lookup properties in the type's interfaces
            var interfaces = typeInfo.ImplementedInterfaces;
            var interfaceAnnotatedProps = interfaces.SelectMany(LookupProperties).ToList();

            //filter annotated properties
            var annotatedProps = props.Where(p =>
                                p.IsDefined(typeof (MementoPropertyAttribute)) ||
                                interfaceAnnotatedProps.Any(interfaceProp => interfaceProp.Name == p.Name))
                         .ToList();
            return ValidateProperties(annotatedProps);
        }

        private bool IsMementoClass(TypeInfo typeInfo)
        {
            return typeInfo.IsDefined(typeof (MementoClassAttribute));
        }

        private IList<PropertyInfo> LookupProperties(Type type)
        {
            TypeInfo typeinfo = type.GetTypeInfo();
            if (IsMementoClass(typeinfo))
                return LookupProperties(typeinfo, true);
            return LookupProperties(typeinfo, false);
        }

        /// <summary>
        /// Returns the type's annotated properties.
        /// If the type has the MementoClass attribute defined, then all properties with get and set accessors are returned.
        /// Otherwise, all properties with the MementoProperty attribute are returned.
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <param name="isMementoClass"></param>
        /// <returns></returns>
        private IList<PropertyInfo> LookupProperties(TypeInfo typeInfo, bool isMementoClass)
        {
            IList<PropertyInfo> props;

            if (isMementoClass)
            {
                props = typeInfo.DeclaredProperties.Where(p => p.HasGetAndSet()).ToList();
            }
            else
            {
                props = typeInfo.DeclaredProperties.Where(p => p.IsDefined(typeof(MementoPropertyAttribute))).ToList();
            }

            return props;
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
                if (!prop.HasGetAndSet())
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
