using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Attributes;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature
{
    [TestFixture]
    public class InheritedProperties
    {
        [Test]
        public void Test()
        {
            BaseArticle article = new Article { Title = "Draft", Id = 1 };

            var memento = Memento.Create()
                                 .Register(article);

            article.Title = "Something else";
            (article as Article).Id = 2;

            memento.Restore();

            Assert.AreEqual("Draft", article.Title);
            Assert.AreEqual(1, (article as Article).Id);
        }

        private abstract class BaseArticle
        {
            [MementoProperty]
            public string Title { get; set; }
        }

        private class Article : BaseArticle
        {
            [MementoProperty]
            public int? Id { get; set; }
        }
    }
}
