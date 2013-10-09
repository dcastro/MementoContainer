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

            var children = new List<ICompositeMemento>
                                   {
                                       new Mock<ICompositeMemento>().Object,
                                       new Mock<ICompositeMemento>().Object
                                   };

            var factoryMock = new Mock<IMementoFactory>();
            factoryMock.Setup(f => f.CreateMementos(InitialVal))
                       .Returns(children);

            var propertyMock = new Mock<IPropertyAdapter>();
            propertyMock.Setup(p => p.IsStatic).Returns(false);
            propertyMock.Setup(p => p.GetValue(obj)).Returns(InitialVal);

            //Act
            var propertyMemento = new PropertyMemento(obj, true, propertyMock.Object, factoryMock.Object);

            //Assert
            Assert.AreEqual(2, propertyMemento.Children.Count());
            CollectionAssert.AreEquivalent(children, propertyMemento.Children);
        }

        [Test]
        public void TestRestore()
        {
            var obj = new object();

            var child1 = new Mock<ICompositeMemento>();
            var child2 = new Mock<ICompositeMemento>();

            var children = new List<ICompositeMemento>
                                      {
                                          child1.Object,
                                          child2.Object
                                      };


            var factoryMock = new Mock<IMementoFactory>();

            var propertyMock = new Mock<IPropertyAdapter>();
            propertyMock.Setup(p => p.IsStatic).Returns(false);
            propertyMock.Setup(p => p.GetValue(obj)).Returns(InitialVal);
            propertyMock.Setup(p => p.SetValue(obj, InitialVal));

            var propertyMemento = new PropertyMemento(obj, false, propertyMock.Object, factoryMock.Object);
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
