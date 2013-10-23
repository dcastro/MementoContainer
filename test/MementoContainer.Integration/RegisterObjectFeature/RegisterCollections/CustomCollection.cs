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
            public Queue<T> Collection { get; set; }

            public int Count
            {
                get { return Collection.Count; }
            }

            public void Clear()
            {
                Collection.Clear();
            }

            public void AddRange(IEnumerable<T> items)
            {
                foreach (var item in items)
                {
                    Collection.Enqueue(item);
                }
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                Collection.CopyTo(array, arrayIndex);
            }
        }
    }
}
