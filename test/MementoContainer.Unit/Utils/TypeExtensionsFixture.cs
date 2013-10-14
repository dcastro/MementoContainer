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
            var expectedAttributes1 = new List<Type>
                {
                    typeof (MementoPropertyAttribute),
                    typeof (MementoCollectionAttribute)
                };

            var expectedAttributes2 = new List<Type>
                {
                    typeof (MementoPropertyAttribute),
                    typeof (MementoCollectionAttribute)
                };

            var expectedAttributes3 = new List<Type>();

            //Act
            var attributesMap = typeof (TestClass).GetFullAttributesMap();

            //Assert
            Assert.AreEqual(3, attributesMap.Count);

            var prop1 = typeof(TestClass).GetProperty("Prop1");
            Assert.True(attributesMap.ContainsKey(prop1));
            CollectionAssert.AreEquivalent(expectedAttributes1, attributesMap[prop1]);

            var prop2 = typeof(TestClass).GetProperty("Prop2");
            Assert.True(attributesMap.ContainsKey(prop2));
            CollectionAssert.AreEquivalent(expectedAttributes2, attributesMap[prop2]);

            var prop3 = typeof(TestClass).GetProperty("Prop3");
            Assert.True(attributesMap.ContainsKey(prop3));
            CollectionAssert.AreEquivalent(expectedAttributes3, attributesMap[prop3]);
        }

        private interface ITestInterface
        {
            [MementoProperty]
            List<string> Prop1 { get; set; }

            [MementoProperty]
            string Prop2 { get; set; }
        }


        private interface ITestInterface2
        {
            [MementoCollection]
            string Prop2 { get; set; }
        }

        private class TestClass : ITestInterface, ITestInterface2
        {
            [MementoCollection]
            public List<string> Prop1 { get; set; }

            public string Prop2 { get; set; }

            public string Prop3 { get; set; }
        }
    }
}
