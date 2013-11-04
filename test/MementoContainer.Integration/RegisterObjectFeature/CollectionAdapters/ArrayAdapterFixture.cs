using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Adapters;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterObjectFeature.CollectionAdapters
{
    [TestFixture]
    public class ArrayAdapterFixture
    {
        [Test]
        public void Test()
        {
            Article article = new Article
            {
                Pages = new[] { 1, 2, 3 }
            };

            //Act
            var memento = Memento.Create()
                                 .Register(article);

            article.Pages[1] = 9;
            article.Pages[2] = 8;

            memento.Rollback();

            //Assert
            CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, article.Pages);
        }

        [Test]
        public void TestRegisterCollection()
        {
            int[] numbers = {1, 2, 3};

            var memento = Memento.Create()
                                 .RegisterCollection(numbers);

            numbers[1] = 9;

            memento.Rollback();

            CollectionAssert.AreEqual(new[] {1,2,3}, numbers);
        }

        private class Article
        {
            [MementoCollection]
            public int[] Pages { get; set; }
        }
    }
}
