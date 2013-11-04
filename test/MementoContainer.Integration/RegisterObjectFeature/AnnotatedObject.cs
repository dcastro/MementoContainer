using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature
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

            memento.Rollback();

            Assert.AreEqual("Draft", article.Title);
        }

        [MementoClass]
        private class Article
        {
            public string Title { get; set; }
        }
    }
}
