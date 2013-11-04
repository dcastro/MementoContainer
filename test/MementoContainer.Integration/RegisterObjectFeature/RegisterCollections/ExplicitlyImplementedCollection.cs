using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature.RegisterCollections
{
    [TestFixture]
    public class ExplicitlyImplementedCollection
    {
        [Test]
        public void Test()
        {
            var article = new Article
                {
                    Ids = new ExplicitCollection(new[] {1, 2, 3})
                };

            var memento = Memento.Create()
                                 .Register(article);

            (article.Ids as ICollection<int>).Remove(2);

            memento.Rollback();

            CollectionAssert.AreEqual(new[] {1,2,3}, article.Ids);
        }

        private class Article
        {
            [MementoCollection]
            public ExplicitCollection Ids { get; set; }
        }

        private class ExplicitCollection : ICollection<int>
        {
            private readonly List<int> _list = new List<int>();  

            public ExplicitCollection(IEnumerable<int> collection)
            {
                _list = new List<int>(collection);
            }

            public IEnumerator<int> GetEnumerator()
            {
                return _list.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            void ICollection<int>.Add(int item)
            {
                _list.Add(item);
            }

            void ICollection<int>.Clear()
            {
                _list.Clear();
            }

            bool ICollection<int>.Contains(int item)
            {
                return _list.Contains(item);
            }

            void ICollection<int>.CopyTo(int[] array, int arrayIndex)
            {
                _list.CopyTo(array, arrayIndex);
            }

            bool ICollection<int>.Remove(int item)
            {
                return _list.Remove(item);
            }

            int ICollection<int>.Count
            {
                get { return _list.Count; }
            }

            bool ICollection<int>.IsReadOnly
            {
                get { return (_list as ICollection<int>).IsReadOnly; }
            }
        }
    }
}
