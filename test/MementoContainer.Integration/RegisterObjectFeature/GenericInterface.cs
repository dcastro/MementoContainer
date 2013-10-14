using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature
{
    [TestFixture]
    public class GenericInterface
    {
        [Test]
        public void Test()
        {
            IArticle<string> article = new Article { Title = "Draft", Id = 1 };

            var memento = Memento.Create()
                                 .Register(article);

            (article as Article).Title = "Something else";
            (article as Article).Id = 2;

            memento.Restore();

            Assert.AreEqual("Draft", article.Title);
            Assert.AreEqual(1, (article as Article).Id);
        }

        private interface IArticle<T>
        {
            [MementoProperty]
            T Title { get; }
        }

        private class Article : IArticle<string>
        {
            public string Title { get; set; }

            [MementoProperty]
            public int? Id { get; set; }
        }
    }
}
