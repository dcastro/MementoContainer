using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Adapters;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature.CollectionAdapters
{
    [TestFixture]
    public class QueueAdapter
    {
        [Test]
        public void Test()
        {
            Article article = new Article
            {
                Pages = new Queue<int>(
                    new[] { 1, 2, 3 })
            };

            //Act
            var memento = Memento.Create()
                                 .Register(article);

            article.Pages.Dequeue();
            article.Pages.Dequeue();

            memento.Restore();

            //Assert
            CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, article.Pages);
        }

        private class Article
        {
            [MementoCollection(typeof(QueueAdapter<int>))]
            public Queue<int> Pages { get; set; }
        }
    }
}
