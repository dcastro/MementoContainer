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
        public Type ElementType { get; private set; }
        public bool Cascade { get; private set; }

        public CollectionData(object collection, Type propertyType, IEnumerable<Attribute> attrs)
            : this(collection, propertyType, attrs, null)
        {

        }

        public CollectionData(object collection, Type propertyType, IEnumerable<Attribute> attrs,
                              MementoClassAttribute mementoClassAttr)
        {
            Collection = collection;

            var collectionAttrs = attrs.OfType<MementoCollectionAttribute>().ToList();

            //If there are any method-level attributes, use those to decide whether 'cascading' should be performed.
            if (collectionAttrs.Any())
            {
                Cascade = collectionAttrs.All(a => a.Cascade);
            }
            else if (mementoClassAttr != null)//otherwise, use the class-level attribute
            {
                Cascade = mementoClassAttr.Cascade;
            }

            Validate(collectionAttrs, propertyType);
        }

        private void Validate(IEnumerable<MementoCollectionAttribute> attrs, Type propertyType)
        {
            //Checks whether the property type implements ICollection<T>.
            //This check should be done on the declared property type (instead of the runtime object type).
            bool isCollection = propertyType.IsCollection();

            bool hasAdapter = false;
            Type adapterType = null;
            if (attrs != null)
            {
                //Find an attribute with an adapter type
                MementoCollectionAttribute attr = attrs.FirstOrDefault(a => a.CollectionAdapterType != null);
                if (attr != null)
                {
                    //get adapter type
                    adapterType = attr.CollectionAdapterType;
                    ValidateAdapter(adapterType, propertyType);

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
                        adapter.Collection = Collection;

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

            //fetch element type
            if (isCollection)
            {
                ElementType = propertyType.GetCollectionElementType();
            }
            if (hasAdapter)
            {
                ElementType = adapterType.GetAdapterElementType();
            }
        }

        private void ValidateAdapter(Type adapterType, Type propertyType)
        {
            //check that it implements the right interface
            if (!adapterType.ImplementsGeneric(typeof (ICollectionAdapter<,>)))
                throw CollectionException.InvalidAdapterType(adapterType);

            //check that it matches the property type
            var boundGenericType = adapterType.GetBoundGenericInterface(typeof (ICollectionAdapter<,>));
            var tCollectionArgument = boundGenericType.GenericTypeArguments[0];

            //if an instance of propertyType cannot be assigned to the type argument TCollection, throw an exceptions
            if (!tCollectionArgument.GetTypeInfo()
                                    .IsAssignableFrom(propertyType.GetTypeInfo()))
            {
                throw CollectionException.AdapterTypeMismatch(adapterType, boundGenericType, propertyType);
            }
        }
    }
}
