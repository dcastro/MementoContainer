using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Analysis;
using MementoContainer.Exceptions;
using Moq;
using NUnit.Framework;

namespace MementoContainer.Unit.Analysis
{
    [TestFixture]
    public class CollectionAnalyzerFixture
    {
        private ICollectionAnalyzer _analyzer;

        [SetUp]
        public void SetUp()
        {
            _analyzer = new CollectionAnalyzer();
        }

        [Test]
        public void TestGetCollections()
        {
            //Arrange
            var article = new Article
                {
                    Authors = new Collection<string>(),
                    Pages = new Collection<int>()
                };
            var expectedResult = new[] {article.Pages};

            //Act
            var result = _analyzer.GetCollections(article);

            //Assert
            CollectionAssert.AreEquivalent(expectedResult, result);
        }

        [Test]
        public void TestNullCollections()
        {
            //Arrange
            var article = new Article();

            //Act
            var result = _analyzer.GetCollections(article);

            //Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void TestNonCollections()
        {
            //Arrange
            var article = new Magazine();

            //Act & Assert
            var ex = Assert.Throws<CollectionException>(() =>  _analyzer.GetCollections(article));
            StringAssert.Contains("Title", ex.Message);
            StringAssert.Contains(typeof(string).Name, ex.Message);
        }

        private class Article
        {
            public ICollection<string> Authors { get; set; }

            [MementoCollection]
            public ICollection<int> Pages { get; set; }
        }

        private class Magazine
        {
            [MementoCollection]
            private string Title { get; set; }
        }
    }
}
