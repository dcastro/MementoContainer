using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Factories;
using Moq;
using NUnit.Framework;

namespace MementoContainer.Unit
{
    [TestFixture]
    class MementoFixture : TestBase
    {
        public Memento Memento { get; set; }

        [SetUp]
        public void SetUp()
        {
            Memento = new Memento();
        }

        [Test]
        public void TestNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() =>Memento.Create().Register(null));

            Assert.Throws<ArgumentNullException>(() => Memento.Create().RegisterProperty(null, (String str) => str.Length));

            Assert.Throws<ArgumentNullException>(() => Memento.Create().RegisterProperty<String, object>("", null));

            Assert.Throws<ArgumentNullException>(() => Memento.Create().RegisterProperty<object>(null));
        }

        [Test]
        public void TestRegisterProperty()
        {
            //Arrange
            var obj = new object();
            Expression<Func<object, int>> expression = s => 1;

            var propertyMementoMock = new Mock<IMementoComponent>();

            var factoryMock = new Mock<IMementoFactory>();
            factoryMock
                .Setup(f => f.CreateMemento(obj, expression))
                .Returns(propertyMementoMock.Object);

            Memento.Factory = factoryMock.Object;

            //Act
            Memento.RegisterProperty(obj, expression);

            //Assert
            CollectionAssert.Contains(Memento.Components, propertyMementoMock.Object);
            factoryMock.Verify(f => f.CreateMemento(obj, expression), Times.Once());
        }

        [Test]
        public void TestRegisterStaticProperty()
        {
            //Arrange
            Expression<Func<string>> expression = () => InitialVal;

            var propertyMementoMock = new Mock<IMementoComponent>();

            var factoryMock = new Mock<IMementoFactory>();
            factoryMock
                .Setup(f => f.CreateMemento(expression))
                .Returns(propertyMementoMock.Object);

            Memento.Factory = factoryMock.Object;

            //Act
            Memento.RegisterProperty(expression);

            //Assert
            CollectionAssert.Contains(Memento.Components, propertyMementoMock.Object);
            factoryMock.Verify(f => f.CreateMemento(expression), Times.Once());
        }

        [Test]
        public void TestRegister()
        {
            //Arrange
            var obj = new object();

            var propertyMementoMock = new Mock<IMementoComponent>();

            var factoryMock = new Mock<IMementoFactory>();
            factoryMock
                .Setup(f => f.CreateMemento(obj))
                .Returns(propertyMementoMock.Object);

            Memento.Factory = factoryMock.Object;

            //Act
            Memento.Register(obj);

            //Assert
            CollectionAssert.Contains(Memento.Components, propertyMementoMock.Object);
            factoryMock.Verify(f => f.CreateMemento(obj), Times.Once());
        }

        [Test]
        public void TestRestore()
        {
            //Arrange
            var componentMock1 = new Mock<IMementoComponent>();
            var componentMock2 = new Mock<IMementoComponent>();

            IList<IMementoComponent> components = new List<IMementoComponent>
                {
                    componentMock1.Object,
                    componentMock2.Object
                };

            Memento.Components = components;

            //Act
            Memento.Restore();

            //Assert
            componentMock1.Verify(c => c.Restore(), Times.Once());
            componentMock2.Verify(c => c.Restore(), Times.Once());
        }
    }
}
