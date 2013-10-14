using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature
{
    [TestFixture]
    public class NonPublicProperties
    {
        [Test]
        public void Test()
        {
            var article = new Article("Draft", 1, "DCastro");

            var memento = Memento.Create()
                                 .Register(article);

            article.ChangeState("Something else", 2, "No one");

            memento.Restore();

            Assert.AreEqual("Draft", article.GetTitle());
            Assert.AreEqual(1, article.GetId());
            Assert.AreEqual("DCastro", article.GetAuthor());
        }

        [MementoClass]
        private class Article
        {
            private string Title { get; set; }

            protected int? Id { get; set; }

            internal string Author { get; set; }

            public Article(string title, int id, string author)
            {
                Title = title;
                Id = id;
                Author = author;
            }

            public void ChangeState(string title, int id, string author)
            {
                Title = title;
                Id = id;
                Author = author;
            }

            public string GetTitle()
            {
                return Title;
            }

            public int? GetId()
            {
                return Id;
            }

            public string GetAuthor()
            {
                return Author;
            }
        }
    }


}
