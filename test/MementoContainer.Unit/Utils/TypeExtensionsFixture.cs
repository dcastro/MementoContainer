using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Utils;
using NUnit.Framework;

namespace MementoContainer.Unit.Utils
{
    [TestFixture]
    public class TypeExtensionsFixture
    {
        [Test]
        public void TestGetAttributesMap()
        {
            //Arrange
            var prop1 = typeof (TestClass).GetProperty("Prop1");
            var prop2 = typeof (TestClass).GetProperty("Prop2");
            var prop3 = typeof (TestClass).GetProperty("Prop3");
            var prop4 = typeof (TestClass).GetProperty("Prop4");
            var prop5 = typeof (TestClass).GetProperty("Prop5");
            var prop6 = typeof (TestClass).GetProperty("Prop6");
            var prop7 = typeof (TestClass).GetProperty("Prop7");

            var expectedAttributes1 = new List<Attribute>
                {
                    typeof (ITestInterface).GetProperty("Prop1").GetCustomAttribute<MementoPropertyAttribute>(),
                    prop1.GetCustomAttribute<MementoCollectionAttribute>()
                };

            var expectedAttributes2 = new List<Attribute>
                {
                    typeof (ITestInterface).GetProperty("Prop2").GetCustomAttribute<MementoPropertyAttribute>(),
                    typeof (ITestInterface2).GetProperty("Prop2").GetCustomAttribute<MementoCollectionAttribute>()
                };

            var expectedAttributes3 = new List<Attribute>();

            var expectedAttributes4 = new List<Attribute>
                {
                    prop4.GetCustomAttribute<MementoCollectionAttribute>()
                };

            var expectedAttributes5 = new List<Attribute>
                {
                    typeof (ITestInterface).GetProperty("Prop5").GetCustomAttribute<MementoCollectionAttribute>(),
                    typeof (ITestInterface2).GetProperty("Prop5").GetCustomAttribute<MementoCollectionAttribute>()
                };

            var expectedAttributes6 = new List<Attribute>
                {
                    prop6.GetCustomAttribute<MementoPropertyAttribute>()
                };

            var expectedAttributes7 = new List<Attribute>
                {
                    typeof (IBaseInterface).GetProperty("Prop7").GetCustomAttribute<MementoPropertyAttribute>()
                };

            //Act
            var attributesMap = typeof (TestClass).GetFullAttributesMap();

            //Assert
            Assert.AreEqual(7, attributesMap.Count);

            //aggregate properties of both class and interface
            Assert.True(attributesMap.ContainsKey(prop1));
            CollectionAssert.AreEquivalent(expectedAttributes1, attributesMap[prop1]);

            //fetch interface attributes
            Assert.True(attributesMap.ContainsKey(prop2));
            CollectionAssert.AreEquivalent(expectedAttributes2, attributesMap[prop2]);

            //no attributes defined
            Assert.True(attributesMap.ContainsKey(prop3));
            CollectionAssert.AreEquivalent(expectedAttributes3, attributesMap[prop3]);

            //attributes on derived classes override properties on base classes
            Assert.True(attributesMap.ContainsKey(prop4));
            CollectionAssert.AreEquivalent(expectedAttributes4, attributesMap[prop4]);

            //when two separate interfaces define the same attribute, both are returned
            Assert.True(attributesMap.ContainsKey(prop5));
            CollectionAssert.AreEquivalent(expectedAttributes5, attributesMap[prop5]);

            //base class attributes are inherited
            Assert.True(attributesMap.ContainsKey(prop6));
            CollectionAssert.AreEquivalent(expectedAttributes6, attributesMap[prop6]);

            //attributes are inherited even if the interface is not implemented directly
            Assert.True(attributesMap.ContainsKey(prop7));
            CollectionAssert.AreEquivalent(expectedAttributes7, attributesMap[prop7]);
        }

        private interface ITestInterface
        {
            [MementoProperty]
            List<string> Prop1 { get; set; }

            [MementoProperty]
            string Prop2 { get; set; }

            [MementoCollection]
            string Prop5 { get; set; }
        }

        private interface ITestInterface2 : IBaseInterface
        {
            [MementoCollection]
            string Prop2 { get; set; }

            [MementoCollection(false)]
            string Prop4 { get; set; }

            [MementoCollection(false)]
            string Prop5 { get; set; }
        }

        private interface IBaseInterface
        {
            [MementoProperty]
            string Prop7 { get; set; }
        }

        private class TestClass : BaseClass, ITestInterface, ITestInterface2
        {
            [MementoCollection]
            public List<string> Prop1 { get; set; }

            public string Prop2 { get; set; }

            public string Prop3 { get; set; }

            [MementoCollection]
            public string Prop4 { get; set; }

            public string Prop5 { get; set; }

            public string Prop7 { get; set; }
        }

        private class BaseClass
        {
            [MementoProperty]
            public string Prop6 { get; set; }
        }
    }
}
