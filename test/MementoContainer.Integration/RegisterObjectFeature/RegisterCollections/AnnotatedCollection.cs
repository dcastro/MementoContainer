﻿using System.Collections.Generic;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature.RegisterCollections
{
    [TestFixture]
    public class AnnotatedCollection
    {
        [Test]
        public void Test()
        {
            var ids = new List<int> {1, 2, 3};
            var authors = new List<string> {"a", "b", "c"};
            var article = new Article
                {
                    Ids = ids,
                    Authors = authors
                };

            var memento = Memento.Create()
                                 .Register(article);

            article.Ids.Remove(2);
            article.Ids.Add(4);

            article.Authors.Remove("b");
            article.Authors.Add("d");

            memento.Rollback();

            CollectionAssert.AreEqual(new List<int> {1, 2, 3}, article.Ids);
            CollectionAssert.AreEqual(new List<string> {"a", "c", "d"}, article.Authors);
        }

        class Article
        {
            [MementoCollection]
            public List<int> Ids { get; set; }

            public List<string> Authors { get; set; }
        }
    }
}
