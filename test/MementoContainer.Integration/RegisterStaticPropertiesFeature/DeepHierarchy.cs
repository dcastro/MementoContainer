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
            Article.Count = new Count {Views = 1};

            var memento = Memento.Create()
                                 .RegisterProperty(() => Article.Count.Views);

            Article.Count.Views++;

            memento.Rollback();

            Assert.AreEqual(1, Article.Count.Views);
        }

        private static class Article
        {
            public static Count Count { get; set; }
        }

        private class Count
        {
            public int Views { get; set; }
        }
    }
}
