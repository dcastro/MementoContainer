using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Adapters;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature.CollectionAdapters
{
    [TestFixture]
    public class ConcurrentBagAdapterFixture
    {
        [Test]
        public void Test()
        {
            Article article = new Article
                {
                    Pages = new ConcurrentBag<int>(
                        new[] {1, 2, 3})
                };

            //Act
            var memento = Memento.Create()
                                 .Register(article);

            int ignore;
            article.Pages.TryTake(out ignore);
            article.Pages.TryTake(out ignore);

            memento.Rollback();

            //Assert
            CollectionAssert.AreEquivalent(new[] {1, 2, 3}, article.Pages);
        }

        private class Article
        {
            [MementoCollection(typeof (ProducerConsumerCollectionAdapter<int>))]
            public ConcurrentBag<int> Pages { get; set; }
        }
    }
}