using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Adapters;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature.RegisterCollections
{
    [TestFixture]
    public class OverrideMementoClass
    {
        [Test]
        public void Test()
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

        [MementoClass(false)]
        private class Article
        {
            [MementoCollection(typeof(QueueAdapter<Author>), true)]
            public Queue<Author> Authors { get; set; }
        }

        private class Author
        {
            [MementoProperty]
            public string Name { get; set; }
        }
    }
}
