using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature.DisableCascading
{
    [TestFixture]
    public class ForProperties
    {
        [Test]
        public void TestWithMementoClassAttribute()
        {
            var author = new Author {Name = "DCastro"};
            var article = new Article
                {
                    Author = author
                };

            var memento = Memento.Create()
                                 .Register(article);

            author.Name = "No one";

            memento.Restore();

            Assert.AreEqual("No one", article.Author.Name);
        }

        [Test]
        public void TestWithMementoPropertyAttribute()
        {
            var author = new Author {Name = "DCastro"};
            var article = new Article2
                {
                    Author = author
                };

            var memento = Memento.Create()
                                 .Register(article);

            author.Name = "No one";

            memento.Restore();

            Assert.AreEqual("No one", article.Author.Name);
        }

        private class Article
        {
            [MementoProperty(false)]
            public Author Author { get; set; }
        }

        [MementoClass(false)]
        private class Article2
        {
            public Author Author { get; set; }
        }

        [MementoClass]
        private class Author
        {
            public string Name { get; set; }
        }
    }
}
