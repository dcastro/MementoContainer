using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Attributes;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature.RegisterCollections
{
    [TestFixture]
    public class NullCollection
    {
        [Test]
        public void Test()
        {
            var article = new Article
                {
                    Ids = null
                };

            var memento = Memento.Create()
                                 .Register(article);

            article.Ids = new List<int> {1};

            memento.Restore();

            CollectionAssert.AreEqual(new List<int> {1}, article.Ids);
        }

        class Article
        {
            [MementoCollection]
            public List<int> Ids { get; set; }
        }
    }
}
