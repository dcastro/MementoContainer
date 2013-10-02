using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Adapters;
using MementoContainer.Factories;
using MementoContainer.Utils;
using Moq;
using NUnit.Framework;

namespace MementoContainer.Unit
{
    [TestFixture]
    class ObjectMementoFixture : TestBase
    {
        [Test]
        public void TestConstructor()
        {
            //Arrange
            var obj = new object();

            var propertyMock1 = new Mock<IPropertyAdapter>();
            var propertyMock2 = new Mock<IPropertyAdapter>();

            var componentMock1 = new Mock<ICompositePropertyMemento>();
            var componentMock2 = new Mock<ICompositePropertyMemento>();

            IList<IPropertyAdapter> properties = new List<IPropertyAdapter>
                {
                    propertyMock1.Object,
                    propertyMock2.Object
                };

            var analyzerMock = new Mock<IPropertyAnalyzer>();
            analyzerMock.Setup(a => a.GetProperties(obj)).Returns(properties);

            var factoryMock = new Mock<IMementoFactory>();
            factoryMock.Setup(f => f.CreateMemento(obj, It.IsAny<IPropertyAdapter>())).Returns<object, IPropertyAdapter>(
                (o, prop) =>
                    {
                        if (prop == propertyMock1.Object)
                            return componentMock1.Object;
                        if (prop == propertyMock2.Object)
                            return componentMock2.Object;
                        return null;
                    }
                );

            //Act
            var objectMemento = new ObjectMemento(obj, factoryMock.Object, analyzerMock.Object);

            //Assert
            Assert.AreEqual(2, objectMemento.Components.Count());
            CollectionAssert.Contains(objectMemento.Components, componentMock1.Object);
            CollectionAssert.Contains(objectMemento.Components, componentMock2.Object);
        }

        [Test]
        public void TestRestore()
        {
            //Arrange
            var componentMock1 = new Mock<ICompositePropertyMemento>();
            componentMock1.Setup(c => c.Restore());
            var componentMock2 = new Mock<ICompositePropertyMemento>();
            componentMock1.Setup(c => c.Restore());

            IList<ICompositePropertyMemento> components = new List<ICompositePropertyMemento>
                                                              {
                                                                  componentMock1.Object,
                                                                  componentMock2.Object
                                                              };
            var obj = new object();

            var analyzerMock = new Mock<IPropertyAnalyzer>();
            analyzerMock.Setup(a => a.GetProperties(obj)).Returns(new List<IPropertyAdapter>());

            var factoryMock = new Mock<IMementoFactory>();

            var objectMemento = new ObjectMemento(obj, factoryMock.Object, analyzerMock.Object);
            objectMemento.Components = components;

            //Act
            objectMemento.Restore();

            //Assert
            componentMock1.Verify(c => c.Restore(), Times.Once());
            componentMock2.Verify(c => c.Restore(), Times.Once());
        }
    }
}
