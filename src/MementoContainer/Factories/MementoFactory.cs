using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Adapters;
using MementoContainer.Utils;

namespace MementoContainer.Factories
{
    internal class MementoFactory : IMementoFactory
    {
        //dependencies
        private IPropertyAnalyzer Analyzer { get; set; }

        public MementoFactory(IPropertyAnalyzer analyzer)
        {
            Analyzer = analyzer;
        }

        public IEnumerable<ICompositeMemento> CreateMementos(object owner)
        {
            var props = Analyzer.GetProperties(owner);
            return props.Select(p => new PropertyMemento(owner, true, p, this)).ToList();
        }

        public IMementoComponent CreateMemento<TOwner, TProp>(TOwner owner, Expression<Func<TOwner, TProp>> propertyExpression)
        {
            var props = Analyzer.GetProperties(propertyExpression);

            if (props.Count == 1)
                return new PropertyMemento(owner, false, props.First(), this);
            return new DeepPropertyMemento(owner, props);
        }

        public IMementoComponent CreateMemento<TProp>(Expression<Func<TProp>> propertyExpression)
        {
            var props = Analyzer.GetProperties(propertyExpression);

            if (props.Count == 1)
                return new PropertyMemento(null, false, props.First(), this);
            return new DeepPropertyMemento(null, props);
        }
    }
}
