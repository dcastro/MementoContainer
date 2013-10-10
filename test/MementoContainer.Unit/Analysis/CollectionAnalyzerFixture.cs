using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Analysis;
using MementoContainer.Attributes;
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
            var article = new Article();
            var expectedResult = new[] { article.Pages };

            //Act
            var result = _analyzer.GetCollections(article);

            //Assert
            CollectionAssert.AreEquivalent(expectedResult, result);
        }

        class Article
        {
            public ICollection<string> Authors { get; set; }

            [MementoCollection]
            public ICollection<int> Pages { get; set; } 
        }
    }
}
