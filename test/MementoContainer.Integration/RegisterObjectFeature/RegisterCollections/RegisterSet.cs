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
    public class RegisterSet
    {
        [Test]
        public void Test()
        {
            var article = new Article
                {
                    Pages = new SetMock<int> {1, 2, 3}
                };

            var memento = Memento.Create()
                                 .Register(article);

            memento.Rollback();

            Assert.True(((SetMock<int>)article.Pages).UnionWithCalled);
        }

        private class Article
        {
            [MementoCollection]
            public ICollection<int> Pages { get; set; }
        }

        private class SetMock<T> : ISet<T>
        {
            private readonly HashSet<T> _set = new HashSet<T>();

            public SetMock()
            {
                UnionWithCalled = false;
            } 

            public bool UnionWithCalled { get; private set; }

            public IEnumerator<T> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(T item)
            {
                _set.Add(item);
            }

            public void UnionWith(IEnumerable<T> other)
            {
                UnionWithCalled = true;
            }

            public void IntersectWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public void ExceptWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public void SymmetricExceptWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsSubsetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsSupersetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsProperSupersetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsProperSubsetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool Overlaps(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool SetEquals(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            bool ISet<T>.Add(T item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                _set.Clear();
            }

            public bool Contains(T item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                _set.CopyTo(array, arrayIndex);
            }

            public bool Remove(T item)
            {
                throw new NotImplementedException();
            }

            public int Count
            {
                get { return _set.Count; }
            }
            public bool IsReadOnly { get; private set; }
        }
    }
}
