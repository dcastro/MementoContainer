using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature
{
    [TestFixture]
    public class StaticProperties
    {
        [Test]
        public void Test()
        {
            var article = new Article();
            Article.Count = 1;

            var memento = Memento.Create()
                                 .Register(article);

            Article.Count++;

            memento.Restore();

            Assert.AreEqual(1, Article.Count);
        }

        [MementoClass]
        private class Article
        {
            public static int? Count { get; set; }
        }
    }
}
