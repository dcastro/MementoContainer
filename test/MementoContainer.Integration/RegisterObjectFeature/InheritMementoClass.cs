using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature
{
    [TestFixture]
    public class InheritMementoClass
    {
        [Test]
        public void Test()
        {
            Article article = new Article {Title = "Draft"};

            var memento = Memento.Create()
                                 .Register(article);

            article.Title = "Something else";

            memento.Restore();

            Assert.AreEqual("Draft", article.Title);
        }

        [MementoClass]
        private abstract class BaseArticle
        {
            
        }

        private class Article : BaseArticle
        {
            public string Title { get; set; }
        }
    }
}
