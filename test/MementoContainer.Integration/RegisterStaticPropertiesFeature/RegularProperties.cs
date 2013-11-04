using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterStaticPropertiesFeature
{
    [TestFixture]
    public class RegularProperties
    {
        [Test]
        public void Test()
        {
            Article.Count = 1;

            var memento = Memento.Create()
                                 .RegisterProperty(() => Article.Count);

            Article.Count++;

            memento.Rollback();

            Assert.AreEqual(1, Article.Count);
        }

        private static class Article
        {
            public static int Count { get; set; }
        }
    }
}
