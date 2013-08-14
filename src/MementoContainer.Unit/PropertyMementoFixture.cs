using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MementoContainer.Adapters;
using MementoContainer.Factories;
using MementoContainer.Utils;
using Moq;
using NUnit.Framework;

namespace MementoContainer.Unit
{
    [TestFixture]
    public class PropertyMementoFixture : TestBase
    {
        [Test]
        public void TestGenerateChildren()
        {
            //Arrange
            var obj = new object();

            var childProperties = new List<IPropertyAdapter>
                                      {
                                          new Mock<IPropertyAdapter>().Object,
                                          new Mock<IPropertyAdapter>().Object
                                      };

            var analyzerMock = new Mock<IPropertyAnalyzer>();
            analyzerMock.Setup(a => a.GetProperties(InitialVal)).Returns(childProperties);

            var factoryMock = new Mock<IMementoFactory>();
            factoryMock.Setup(f => f.CreateMemento(obj, It.IsAny<IPropertyAdapter>()))
                       .Returns(new Mock<ICompositePropertyMemento>().Object);

            var propertyMock = new Mock<IPropertyAdapter>();
            propertyMock.Setup(p => p.IsStatic()).Returns(false);
            propertyMock.Setup(p => p.GetValue(obj)).Returns(InitialVal);

            //Act
            var propertyMemento = new PropertyMemento(obj, true, propertyMock.Object, factoryMock.Object, analyzerMock.Object);

            //Assert
            Assert.AreEqual(2, propertyMemento.Children.Count());
        }

        [Test]
        public void TestRestore()
        {
            var obj = new object();

            var child1 = new Mock<ICompositePropertyMemento>();
            var child2 = new Mock<ICompositePropertyMemento>();

            var children = new List<ICompositePropertyMemento>
                                      {
                                          child1.Object,
                                          child2.Object
                                      };

            var analyzerMock = new Mock<IPropertyAnalyzer>();
            analyzerMock.Setup(a => a.GetProperties(InitialVal)).Returns(new List<IPropertyAdapter>());

            var factoryMock = new Mock<IMementoFactory>();

            var propertyMock = new Mock<IPropertyAdapter>();
            propertyMock.Setup(p => p.IsStatic()).Returns(false);
            propertyMock.Setup(p => p.GetValue(obj)).Returns(InitialVal);
            propertyMock.Setup(p => p.SetValue(obj, InitialVal));

            var propertyMemento = new PropertyMemento(obj, false, propertyMock.Object, factoryMock.Object, analyzerMock.Object);
            propertyMemento.Children = children;

            //Act
            propertyMemento.Restore();

            //Assert
            propertyMock.Verify(p => p.SetValue(obj, InitialVal), Times.Once());
            child1.Verify(c => c.Restore(), Times.Once());
            child2.Verify(c => c.Restore(), Times.Once());

        }
    }
}
