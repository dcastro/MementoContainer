using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Attributes;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature.RegisterCollections
{
    [TestFixture]
    public class RegisterCollectionItems
    {
        [Test]
        public void Test()
        {
            var author = new Author {Name = "DCastro"};
            var article = new Article
                {
                    Authors = new List<Author> {author}
                };

            var memento = Memento.Create()
                                 .Register(article);


            author.Name = "No one";

            memento.Restore();

            CollectionAssert.AreEqual("DCastro", article.Authors.First().Name);
        }

        private class Article
        {
            [MementoCollection]
            public List<Author> Authors { get; set; }
        }
    }

    [MementoClass]
    public class Author
    {
        public string Name { get; set; }
    }
}
