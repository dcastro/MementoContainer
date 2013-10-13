using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Attributes;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature.RegisterCollections
{
    [TestFixture]
    public class CollectionOfCollections
    {
        [Test]
        public void Test()
        {
            var author = new Author { Name = "DCastro" };
            var article = new Article
            {
                Authors = new List<ICollection<Author>>
                    {
                        new Collection<Author>
                            {
                                author
                            }
                    }
            };

            var memento = Memento.Create()
                                 .Register(article);

            article.Authors[0] = null;
            article.Authors = null;
            author.Name = "No one";

            memento.Restore();

            Assert.NotNull(article.Authors);
            Assert.AreEqual(1, article.Authors.Count);
            Assert.NotNull(article.Authors[0]);
            Assert.AreEqual(1, article.Authors[0].Count);
            CollectionAssert.AreEqual("DCastro", article.Authors.First().First().Name);
        }

        [MementoClass]
        private class Article
        {
            public IList<ICollection<Author>> Authors { get; set; }
        }

        [MementoClass]
        private class Author
        {
            public string Name { get; set; }
        }
    }
}
