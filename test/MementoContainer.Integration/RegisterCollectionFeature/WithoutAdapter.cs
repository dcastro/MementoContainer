using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterCollectionFeature
{
    [TestFixture]
    public class WithoutAdapter
    {
        [Test]
        public void Test()
        {
            var article1 = new Article {Author = "DCastro"};
            var article2 = new Article {Author = "JBarbosa"};
            var articles = new HashSet<Article>
                {
                    article1,
                    article2
                };

            var memento = Memento.Create()
                                 .RegisterCollection(articles);

            articles.Remove(article2);

            memento.Rollback();

            Assert.AreEqual(2, articles.Count);
            CollectionAssert.Contains(articles, article1);
            CollectionAssert.Contains(articles, article2);
        }

        private class Article
        {
            public string Author { get; set; }
        }
    }
}
