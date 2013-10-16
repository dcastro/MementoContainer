using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using MementoContainer.Utils;

namespace MementoContainer.Integration.RegisterObjectFeature.RegisterCollections
{
    [TestFixture]
    public class RegisterCollectionItems
    {
        [Test]
        public void Test()
        {
            var author = new Author {Name = "DCastro"};
            var article = new Article
                {
                    Authors = new SimpleCollection<Author> {author}
                };

            var memento = Memento.Create()
                                 .Register(article);


            author.Name = "No one";

            memento.Restore();

            CollectionAssert.AreEqual("DCastro", article.Authors.First().Name);
        }

        private class Article
        {
            [MementoCollection]
            public SimpleCollection<Author> Authors { get; set; }
        }

        [MementoClass]
        private class Author
        {
            public string Name { get; set; }
        }
    }

    public class SimpleCollection<T> : ICollection<T>
    {
        private T[] _arr = new T[15];
        private int _count = 0;

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
            {
                yield return _arr[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            if(_count == 15)
                throw new Exception("The collection is at maximum capacity: items cannot be added.");

            _arr[_count] = item;
            _count++;
        }

        public void Clear()
        {
            _arr = new T[15];
            _count = 0;
        }

        public bool Contains(T item)
        {
            return _arr.Take(_count).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _arr.Take(_count)
                .ToArray()
                .CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }
    }
}
