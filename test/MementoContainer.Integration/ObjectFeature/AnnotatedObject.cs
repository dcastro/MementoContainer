using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Attributes;
using NUnit.Framework;

namespace MementoContainer.Integration.ObjectFeature
{
    [TestFixture]
    public class AnnotatedObject
    {
        [Test]
        public void Test()
        {
            var article = new Article { Title = "Draft" };

            var memento = Memento.Create()
                                 .Register(article);

            article.Title = "Something else";

            memento.Restore();

            Assert.AreEqual("Draft", article.Title);
        }

        [MementoClass]
        private class Article
        {
            public string Title { get; set; }
        }
    }
}
