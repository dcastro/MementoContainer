using System.Collections.Generic;
using MementoContainer.Adapters;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature
{
    [TestFixture]
    public class OverrideMementoClass
    {
        [Test]
        public void WithMementoCollection()
        {
            var author1 = new Author {Name = "DCastro"};
            var author2 = new Author {Name = "JBarbosa"};
            var article = new Article
                {
                    Authors = new Queue<Author>(new[]
                        {
                            author1, author2
                        })
                };

            var memento = Memento.Create()
                                 .Register(article);

            article.Authors.Dequeue();
            author1.Name = "No one";
            author2.Name = "No one";

            memento.Restore();

            CollectionAssert.AreEqual(new[] {author1, author2}, article.Authors);
            Assert.AreEqual("DCastro", author1.Name);
            Assert.AreEqual("JBarbosa", author2.Name);
        }

        [Test]
        public void WithMementoProperty()
        {
            var mainAuthor = new Author { Name = "DCastro" };

            var article = new Article
            {
                MainAuthor = mainAuthor
            };

            var memento = Memento.Create()
                                 .Register(article);

            mainAuthor.Name = "No one";
            article.MainAuthor = new Author();

            memento.Restore();

            Assert.AreSame(mainAuthor, article.MainAuthor);
            Assert.AreEqual("DCastro", mainAuthor.Name);
        }

        [MementoClass(false)]
        private class Article
        {
            [MementoCollection(typeof(QueueAdapter<Author>), true)]
            public Queue<Author> Authors { get; set; }

            [MementoProperty(true)]
            public Author MainAuthor { get; set; }
        }

        [MementoClass]
        private class Author
        {
            public string Name { get; set; }
        }
    }
}
