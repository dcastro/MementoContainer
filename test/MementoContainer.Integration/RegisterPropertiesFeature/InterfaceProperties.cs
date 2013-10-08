using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterPropertiesFeature
{
    [TestFixture]
    public class InterfaceProperties
    {
        [Test]
        public void Test()
        {
            IArticle article = new Article {Title = "Draft"};

            var memento = Memento.Create()
                                 .RegisterProperty(article, a => a.Title);

            article.Title = "Something else";

            memento.Restore();

            Assert.AreEqual("Draft", article.Title);
        }

        private interface IArticle
        {
            string Title { get; set; }
        }

        private class Article : IArticle
        {
            public string Title { get; set; }
        }
    }
}
