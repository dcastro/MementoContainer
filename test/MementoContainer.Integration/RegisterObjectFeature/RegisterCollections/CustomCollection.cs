using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature.RegisterCollections
{
    [TestFixture]
    public class CustomCollection
    {
        [Test]
        public void TestWithAdapter()
        {
            Article article = new Article
                {
                    Pages = new Queue<int>(
                        new[] {1, 2, 3})
                };

            //Act
            var memento = Memento.Create()
                                 .Register(article);

            article.Pages.Dequeue();
            article.Pages.Dequeue();

            memento.Restore();

            //Assert
            CollectionAssert.AreEquivalent(new[] {1, 2, 3}, article.Pages);
        }

        [Test]
        public void TestWithoutAdapter()
        {
            Magazine magazine = new Magazine
            {
                Pages = new Queue<int>(
                    new[] { 1, 2, 3 })
            };

            //Act
            var memento = Memento.Create();

            Assert.Throws<CollectionException>(() => memento.Register(magazine));
        }

        private class Article
        {
            [MementoCollection(typeof(QueueAdapter<int>))]
            public Queue<int> Pages { get; set; } 
        }

        private class Magazine
        {
            [MementoCollection]
            public Queue<int> Pages { get; set; } 
        }

        private class QueueAdapter<T> : ICollectionAdapter<Queue<T>, T>
        {
            private Queue<T> _queue;
            public void Initalize(Queue<T> collection)
            {
                _queue = collection;
            }

            public int Count { get { return _queue.Count; } }
            public void Clear()
            {
                _queue.Clear();
            }

            public void AddRange(IEnumerable<T> items)
            {
                foreach (var item in items)
                {
                    _queue.Enqueue(item);
                }
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                _queue.CopyTo(array, arrayIndex);
            }
        }
    }
}
