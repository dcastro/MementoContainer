using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature.DisableCascading
{
    [TestFixture]
    public class CascadingDefinedTwice
    {
        /// <summary>
        /// If a class implements multiple interfaces, and if any of them disables cascading, then cascading should not happen.
        /// </summary>
        [Test]
        public void Test()
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

        private interface IArticle
        {
            [MementoProperty(false)]
            Author Author { get; set; }
        }

        private interface IArticle2
        {
            [MementoProperty(true)]
            Author Author { get; set; }
        }

        private class Article : IArticle, IArticle2
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
