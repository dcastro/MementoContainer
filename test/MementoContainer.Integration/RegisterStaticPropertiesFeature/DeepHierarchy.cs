using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterStaticPropertiesFeature
{
    [TestFixture]
    public class DeepHierarchy
    {
        [Test]
        public void Test()
        {
            Article.Count = new Count {Number = 1};

            var memento = Memento.Create()
                                 .RegisterProperty(() => Article.Count.Number);

            Article.Count.Number++;

            memento.Restore();

            Assert.AreEqual(1, Article.Count.Number);
        }

        private static class Article
        {
            public static Count Count { get; set; }
        }

        private class Count
        {
            public int Number { get; set; }
            public DateTime LastUpdated { get; set; }
        }
    }
}
