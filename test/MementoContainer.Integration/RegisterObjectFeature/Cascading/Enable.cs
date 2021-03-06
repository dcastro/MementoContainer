﻿using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature.Cascading
{
    [TestFixture]
    public class Enable
    {
        [Test]
        public void Test()
        {
            Author author = new Author {Name = "DCastro"};
            Article article = new Article {Author = author};

            var memento = Memento.Create()
                                 .Register(article);

            author.Name = "No one";
            article.Author = new Author {Name = "No one"};

            memento.Rollback();

            Assert.AreSame(author, article.Author);
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
