using System.Collections.Generic;
using MementoContainer.Analysis;
using MementoContainer.Attributes;
using NUnit.Framework;

namespace MementoContainer.Unit.Analysis
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
            public string GetOnlyProperty { get { return ""; } }
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

            [MementoProperty]
            public string GetOnly { get { return ""; } }
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
            StringAssert.Contains("method", ex.Message);

            ex = Assert.Throws<InvalidExpressionException>(() =>
                                          Analyzer.GetProperties(
                                              () => Article.StaticPrint()
                                              ));
            StringAssert.Contains("method", ex.Message);
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

            StringAssert.Contains("closure", ex.Message);

            ex = Assert.Throws<InvalidExpressionException>(() =>
                                                      Analyzer.GetProperties(
                                                          () => magazine.Articles
                                                          ));

            StringAssert.Contains("closure", ex.Message);
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

            StringAssert.Contains("field", ex.Message);

            ex = Assert.Throws<InvalidExpressionException>(() =>
                                          Analyzer.GetProperties(
                                              () => Article.StaticField
                                              ));

            StringAssert.Contains("field", ex.Message);
        }

        /// <summary>
        /// Tests that an exception is thrown when a property missing a set accessor is registered.
        /// </summary>
        [Test]
        public void TestGetOnlyProperty()
        {
            var ex = Assert.Throws<PropertyException>(() => Analyzer.GetProperties(new Magazine()));
            StringAssert.Contains("accessors", ex.Message);

            ex = Assert.Throws<PropertyException>(() => Analyzer.GetProperties((Magazine m) => m.GetOnly));
            StringAssert.Contains("accessors", ex.Message);
        }
    }
}
