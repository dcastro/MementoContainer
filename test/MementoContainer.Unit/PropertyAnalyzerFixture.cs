using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Adapters;
using MementoContainer.Utils;
using NUnit.Framework;

namespace MementoContainer.Unit
{
    [TestFixture]
    class PropertyAnalyzerFixture
    {
        public PropertyAnalyzer Analyzer { get; set; }

        [SetUp]
        public void SetUp()
        {
            Analyzer = new PropertyAnalyzer();
        }

        #region Fixture
        private class Article
        {
            public int Field = 0;
            public static int StaticField = 0;

            public int Print()
            {
                return 0;
            }

            public static int StaticPrint()
            {
                return 0;
            }
        }
        #endregion

        private class Magazine
        {
            public IList<Article> Articles { get; set; } 
        }

        /// <summary>
        /// Tests that an exception is thrown when an expression containing method calls is supplied.
        /// </summary>
        [Test]
        public void TestInvalidExpressionWithMethods()
        {
            //methods
            var ex = Assert.Throws<InvalidExpressionException>(() =>
                                                      Analyzer.GetProperties(
                                                          (Article a) => a.Print()
                                                          ));
            Assert.True(ex.Message.Contains("method"));

            ex = Assert.Throws<InvalidExpressionException>(() =>
                                          Analyzer.GetProperties(
                                              () => Article.StaticPrint()
                                              ));
            Assert.True(ex.Message.Contains("method"));
        }

        /// <summary>
        /// Tests that an exception is thrown when an expression containing closures is supplied.
        /// </summary>
        [Test]
        public void TestInvalidExpressionWithClosures()
        {
            Magazine magazine = new Magazine();
            var ex = Assert.Throws<InvalidExpressionException>(() =>
                                                      Analyzer.GetProperties(
                                                          (Article a) => magazine.Articles
                                                          ));

            Assert.True(ex.Message.Contains("closure"));

            ex = Assert.Throws<InvalidExpressionException>(() =>
                                                      Analyzer.GetProperties(
                                                          () => magazine.Articles
                                                          ));

            Assert.True(ex.Message.Contains("closure"));
        }

        /// <summary>
        /// Test that when an expression with a parameter returns a static property, an exception is thrown.
        /// </summary>
        [Test]
        public void TestInvalidExpressionWithUnrelatedProperty()
        {
            //supplying a parameter and a static property
            var ex = Assert.Throws<InvalidExpressionException>(() =>
                                                      Analyzer.GetProperties(
                                                          (Article a) => Article.StaticPrint()
                                                          ));
        }

        /// <summary>
        /// Tests that an exception is thrown when an expression containing fields is supplied.
        /// </summary>
        [Test]
        public void TestInvalidExpressionWithFields()
        {
            var ex = Assert.Throws<InvalidExpressionException>(() =>
                                                      Analyzer.GetProperties(
                                                          (Article a) => a.Field
                                                          ));

            Assert.True(ex.Message.Contains("field"));

            ex = Assert.Throws<InvalidExpressionException>(() =>
                                          Analyzer.GetProperties(
                                              () => Article.StaticField
                                              ));

            Assert.True(ex.Message.Contains("field"));
        }
    }
}
