using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Domain;
using Moq;
using NUnit.Framework;

namespace MementoContainer.Unit.Domain
{
    [TestFixture]
    public class CollectionDataFixture
    {
        [Test]
        public void TestInvalidAdapterTypeThrowsException()
        {
            Attribute[] attributes = new Attribute[]
                {
                    new MementoCollectionAttribute(typeof(string))
                };
            
            //Act
            var ex = Assert.Throws<CollectionException>(() => new CollectionData(null, typeof (ICollection<int>), attributes));
            Debug.WriteLine(ex.Message);
        }

        [Test]
        public void TestInvalidAdapterImplementationThrowsException()
        {
            Attribute[] attributes = new Attribute[]
                {
                    new MementoCollectionAttribute(typeof(InvalidAdapter))
                };

            //Act
            var ex = Assert.Throws<CollectionException>(() => new CollectionData(null, typeof(Stack<int>), attributes));
            Debug.WriteLine(ex.Message);
            StringAssert.Contains("instantiate", ex.Message);
            StringAssert.Contains(typeof(InvalidAdapter).Name, ex.Message);
        }

        [Test]
        public void TestValidAdapterWrapsCollection()
        {

            Attribute[] attributes = new Attribute[]
                {
                    new MementoCollectionAttribute(typeof(ValidAdapter))
                };

            //Act
            var data = new CollectionData(new Stack<int>(), typeof (Stack<int>), attributes);

            Assert.IsInstanceOf<ValidAdapter>(data.Collection);
        }
        
        [Test]
        public void InvalidCollectionWithoutAdapter()
        {
            Attribute[] attributes = new Attribute[0];

            //Act
            var ex = Assert.Throws<CollectionException>(() => new CollectionData(new Stack<int>(), typeof(Stack<int>), attributes));
            Debug.WriteLine(ex.Message);
            StringAssert.Contains(typeof(Stack<int>).Name, ex.Message);
        }

        [Test]
        public void TestAdapterTypeAndCollectionTypeMismatch()
        {
            Attribute[] attributes = new Attribute[]
                {
                    new MementoCollectionAttribute(typeof(ValidAdapter))
                };

            //Act
            var ex = Assert.Throws<CollectionException>(() => new CollectionData(new Queue<int>(), typeof(Queue<int>), attributes));
            Debug.WriteLine(ex.Message);
            StringAssert.Contains("cannot be used", ex.Message);
        }

        /// <summary>
        /// ICollectionAdapter with no public parameterless constructor
        /// </summary>
        private class InvalidAdapter : ICollectionAdapter<Stack<int>, int>
        {
            public InvalidAdapter(int i) { }
            public void Initalize(Stack<int> collection)
            {
                throw new NotImplementedException();
            }

            public int Count { get; private set; }
            public void Clear()
            {
                throw new NotImplementedException();
            }

            public void AddRange(IEnumerable<int> items)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(int[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }
        }

        private class ValidAdapter : ICollectionAdapter<Stack<int>, int>
        {
            private Stack<int> _stack = new Stack<int>(); 

            public void Initalize(Stack<int> collection)
            {
                _stack = collection;
            }

            public int Count { get; private set; }
            public void Clear()
            {
                throw new NotImplementedException();
            }

            public void AddRange(IEnumerable<int> items)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(int[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }
        }
    }


}
