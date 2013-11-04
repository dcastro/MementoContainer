using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature.Cascading
{
    [TestFixture]
    public class DisableForCollections
    {
        [Test]
        public void TestWithMementoClassAttribute()
        {
            var author = new Author {Name = "DCastro"};
            var article = new Article
                {
                    Authors = new List<Author> {author}
                };

            var memento = Memento.Create()
                                 .Register(article);

            author.Name = "No one";

            memento.Rollback();

            Assert.AreEqual("No one", article.Authors.First().Name);
        }

        [Test]
        public void TestWithMementoCollectionAttribute()
        {
            var author = new Author {Name = "DCastro"};
            var article = new Article2
                {
                    Authors = new List<Author> {author}
                };

            var memento = Memento.Create()
                                 .Register(article);

            author.Name = "No one";

            memento.Rollback();

            Assert.AreEqual("No one", article.Authors.First().Name);
        }

        private class Article
        {
            [MementoCollection(false)]
            public List<Author> Authors { get; set; }
        }

        [MementoClass(false)]
        private class Article2
        {
            public List<Author> Authors { get; set; }
        }

        [MementoClass]
        private class Author
        {
            public string Name { get; set; }
        }
    }
}
