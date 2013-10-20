using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Utils;

namespace MementoContainer.Domain
{
    internal class CollectionData : ICollectionData
    {
        public object Collection { get; private set; }
        public bool Cascade { get; private set; }

        private void Validate(IEnumerable<MementoCollectionAttribute> attrs, Type propertyType)
        {
            //Checks whether the property type implements ICollection<T>.
            //This check should be done on the declared property type (instead of the runtime object type).
            bool isCollection = propertyType.IsCollection();

            bool hasAdapter = false;
            if (attrs != null)
            {
                //Find an attribute with an adapter type
                MementoCollectionAttribute attr = attrs.FirstOrDefault(a => a.CollectionAdapterType != null);
                if (attr != null)
                {
                    //get adapter type
                    Type adapterType = attr.CollectionAdapterType;
                    if (!adapterType.ImplementsGeneric(typeof (ICollectionAdapter<,>)))
                        throw CollectionException.InvalidAdapterType(adapterType);

                    hasAdapter = true;

                    //create adapter
                    dynamic adapter = null;
                    try
                    {
                        adapter = new DynamicInvoker(Activator.CreateInstance(adapterType));
                    }
                    catch (MissingMemberException ex)
                    {
                        throw CollectionException.FailedAdapterActivation(adapterType, ex);
                    }
                    

                    if (Collection != null)
                    {
                        //initialize adapter
                        adapter.Initalize(Collection);

                        //replace collection with adapter
                        Collection = adapter.InnerObject;
                    }
                }
            }

            //if its not a valid collection and a valid adapter was not supplied
            if (!isCollection && !hasAdapter)
            {
                throw CollectionException.IsNotCollection(propertyType);
            }
        }

        public CollectionData(object collection, Type propertyType, IEnumerable<Attribute> attrs)
        {
            Collection = collection;

            var collectionAttrs = attrs.OfType<MementoCollectionAttribute>().ToList();
            Cascade = collectionAttrs.All(a => a.Cascade);

            Validate(collectionAttrs, propertyType);
        }

        public CollectionData(object collection, MementoClassAttribute attr)
        {
            Collection = collection;
            Cascade = attr.Cascade;
        }
    }
}
