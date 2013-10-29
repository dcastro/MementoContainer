using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature
{
    [TestFixture]
    public class AnnotatedProperties
    {
        [Test]
        public void TestRestoreAnnotatedProperty()
        {
            var article = new Article {Title = "Draft"};

            var memento = Memento.Create()
                                 .Register(article);

            article.Title = "Something else";

            memento.Rollback();

            Assert.AreEqual("Draft", article.Title);
        }

        [Test]
        public void TestDoesntRestoreRegularProperty()
        {
            var article = new Article { Id = 1 };

            var memento = Memento.Create()
                                 .Register(article);

            article.Id = 2;

            memento.Rollback();

            Assert.AreEqual(2, article.Id);
        }

        private class Article
        {
            [MementoProperty]
            public string Title { get; set; }

            public int Id { get; set; }
        }
    }
}
