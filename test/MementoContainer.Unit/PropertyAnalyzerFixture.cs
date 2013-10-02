using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Adapters;
using MementoContainer.Mocks;
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

        /// <summary>
        /// Test that an expression containing an instance property is properly resolved
        /// </summary>
        [Test]
        public void TestGetProperties()
        {
            //Arrange
            var expectedProperties = new[]
                {
                    new PropertyInfoAdapter(typeof (SimpleMock).GetProperty("Property"))
                };

            //Act
            var properties = Analyzer.GetProperties((SimpleMock m) => m.Property);

            //Assert
            CollectionAssert.AreEquivalent(expectedProperties, properties);
        }

        /// <summary>
        /// Test that an expression containing a static property is properly resolved
        /// </summary>
        [Test]
        public void TestGetStaticProperty()
        {
            //Arrange
            var expectedProperties = new[]
                {
                    new PropertyInfoAdapter(typeof (StaticMock).GetProperty("StaticProperty"))
                };

            //Act
            var properties = Analyzer.GetProperties(() => StaticMock.StaticProperty);

            //Assert
            CollectionAssert.AreEquivalent(expectedProperties, properties);
        }

        /// <summary>
        /// Test that only properties with the MementoProperty attribute are returned
        /// </summary>
        [Test]
        public void TestGetAnnotatedProperties()
        {
            //Arrange
            Type t = typeof (AnnotatedMock);
            var expectedProperties = new[]
                                         {
                                             new PropertyInfoAdapter(t.GetProperty("NestedProperty")),
                                             new PropertyInfoAdapter(t.GetProperty("PublicProperty")),
                                             new PropertyInfoAdapter(t.GetProperty("PrivateProperty", BindingFlags.NonPublic | BindingFlags.Instance)),
                                             new PropertyInfoAdapter(t.GetProperty("StaticProperty", BindingFlags.NonPublic | BindingFlags.Static))
                                         };

            //Act
            var properties = Analyzer.GetProperties(new AnnotatedMock());

            //Assert
            CollectionAssert.AreEquivalent(expectedProperties, properties);
        }

        /// <summary>
        /// Test that all properties of as class with the MementoClass attribute are returned
        /// </summary>
        [Test]
        public void TestGetMementoClassProperties()
        {
            //Arrange
            Type t = typeof(AnnotatedClassMock);
            var expectedProperties = new[]
                                         {
                                             new PropertyInfoAdapter(t.GetProperty("Property"))
                                         };

            //Act
            var properties = Analyzer.GetProperties(new AnnotatedClassMock());

            //Assert
            CollectionAssert.AreEquivalent(expectedProperties, properties);
        }

        [Test]
        public void TestGetDeepProperties()
        {
            //Arrange
            var expectedProperties = new[]
                {
                    new PropertyInfoAdapter(typeof (DeepMock).GetProperty("DeepProperty")),
                    new PropertyInfoAdapter(typeof (SimpleMock).GetProperty("Property"))
                };

            //Act
            var properties = Analyzer.GetProperties((DeepMock m) => m.DeepProperty.Property);

            //Assert
            CollectionAssert.AreEqual(expectedProperties, properties);
        }

        [Test]
        public void TestGetStaticDeepProperties()
        {
            //Arrange
            var expectedProperties = new[]
                {
                    new PropertyInfoAdapter(typeof (DeepMock).GetProperty("DeepStaticProperty")),
                    new PropertyInfoAdapter(typeof (SimpleMock).GetProperty("Property"))
                };

            //Act
            var properties = Analyzer.GetProperties(() => DeepMock.DeepStaticProperty.Property);

            //Assert
            CollectionAssert.AreEqual(expectedProperties, properties);
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
                                                          (AnnotatedMock m) => m.GetPrivate()
                                                          ));
            Assert.True(ex.Message.Contains("method"));

            ex = Assert.Throws<InvalidExpressionException>(() =>
                                          Analyzer.GetProperties(
                                              () => StaticMock.Method()
                                              ));
            Assert.True(ex.Message.Contains("method"));
        }

        /// <summary>
        /// Tests that an exception is thrown when an expression containing closures is supplied.
        /// </summary>
        [Test]
        public void TestInvalidExpressionWithClosures()
        {
            DeepMock closureObj = new DeepMock();
            var ex = Assert.Throws<InvalidExpressionException>(() =>
                                                      Analyzer.GetProperties(
                                                          (SimpleMock m) => closureObj.DeepProperty
                                                          ));

            Assert.True(ex.Message.Contains("closure"));

            ex = Assert.Throws<InvalidExpressionException>(() =>
                                                      Analyzer.GetProperties(
                                                          () => closureObj.DeepProperty
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
                                                          (SimpleMock m) => StaticMock.StaticProperty
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
                                                          (SimpleMock m) => m.Field
                                                          ));

            Assert.True(ex.Message.Contains("field"));

            ex = Assert.Throws<InvalidExpressionException>(() =>
                                          Analyzer.GetProperties(
                                              () => StaticMock.Field
                                              ));

            Assert.True(ex.Message.Contains("field"));
        }
    }
}
