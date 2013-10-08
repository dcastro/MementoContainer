﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Attributes;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature
{
    [TestFixture]
    public class GenericClass
    {
        [Test]
        public void Test()
        {
            Article<string> article = new Article<string> { Title = "Draft" };

            var memento = Memento.Create()
                                 .Register(article);

            article.Title = "Something else";

            memento.Restore();

            Assert.AreEqual("Draft", article.Title);
        }

        private class Article<T>
        {
            [MementoProperty]
            public T Title { get; set; }
        }
    }
}