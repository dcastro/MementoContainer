using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Attributes;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterPropertiesFeature
{
    [TestFixture]
    public class DeepHierarchy
    {
        [Test]
        public void Test()
        {
            Author author = new Author { Name = "DCastro" };
            Article article = new Article { Author = author };

            var memento = Memento.Create()
                                 .RegisterProperty(article, a => a.Author.Name);

            article.Author = new Author { Name = "No one" };

            memento.Restore();

            Assert.AreEqual("DCastro", article.Author.Name);
        }


        private class Article
        {
            [MementoProperty]
            public Author Author { get; set; }
        }

        private class Author
        {
            [MementoProperty]
            public string Name { get; set; }
        }
    }
}
