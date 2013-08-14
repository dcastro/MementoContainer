using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Factories;
using MementoContainer.Utils;

namespace MementoContainer
{
    /// <summary>
    /// Represents a group of registered properties that belong to the same owner object.
    /// If the owner's type has the MementoClass attribute defined, all properties declaring get and set accessors are registered.
    /// Otherwise, only properties with the MementoProperty attribute will be registered.
    /// </summary>
    internal class ObjectMemento : IMementoComponent
    {
        internal IEnumerable<ICompositePropertyMemento> Components { get; set; }

        //dependencies
        private IMementoFactory Factory { get; set; }
        private IPropertyAnalyzer Analyzer { get; set; }

        public ObjectMemento(object obj, IMementoFactory factory, IPropertyAnalyzer analyzer)
        {
            Factory = factory;
            Analyzer = analyzer;

            //Fetches properties
            var props = Analyzer.GetProperties(obj);

            //Create a memento for each property
            Components = props.Select(p => Factory.CreateMemento(obj, p)).ToList();
        }

        public void Restore()
        {
            foreach (var memento in Components)
            {
                memento.Restore();
            }
        }
    }
}
