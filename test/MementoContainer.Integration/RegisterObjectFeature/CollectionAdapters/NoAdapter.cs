using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature.CollectionAdapters
{
    [TestFixture]
    public class NoAdapter
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
            var memento = Memento.Create();

            var ex = Assert.Throws<CollectionException>(() => memento.Register(article));
            StringAssert.Contains("ICollectionAdapter", ex.Message);
        }

        private class Article
        {
            [MementoCollection]
            public Queue<int> Pages { get; set; }
        }
    }
}
