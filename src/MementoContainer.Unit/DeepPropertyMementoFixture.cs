using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MementoContainer.Adapters;
using Moq;
using NUnit.Framework;

namespace MementoContainer.Unit
{
    [TestFixture]
    class DeepPropertyMementoFixture : TestBase
    {
        [Test]
        public void TestGetValue()
        {
            //Arrange
            var obj = (object) 1;
            var link1Val = (object)2;
            var link2Val = (object)3;

            var link1 = new Mock<IPropertyAdapter>();
            link1.Setup(l => l.GetValue(obj)).Returns(link1Val);

            var link2 = new Mock<IPropertyAdapter>();
            link2.Setup(l => l.GetValue(link1Val)).Returns(link2Val);

            var property = new Mock<IPropertyAdapter>();
            property.Setup(p => p.GetValue(link2Val)).Returns(FinalVal);

            var props = new List<IPropertyAdapter>()
                {
                    link1.Object,
                    link2.Object,
                    property.Object
                };

            var memento = new DeepPropertyMemento(obj, props);

            //Act
            object value = memento.GetValue();

            //Assert
            Assert.AreEqual(FinalVal, value);
        }

        [Test]
        public void TestRestore()
        {
            //Arrange
            var obj = (object)1;
            var link1Val = (object)2;
            var link2Val = (object)3;

            var link1 = new Mock<IPropertyAdapter>();
            link1.Setup(l => l.GetValue(obj)).Returns(link1Val);

            var link2 = new Mock<IPropertyAdapter>();
            link2.Setup(l => l.GetValue(link1Val)).Returns(link2Val);

            var property = new Mock<IPropertyAdapter>();

            var props = new List<IPropertyAdapter>()
                {
                    link1.Object,
                    link2.Object,
                    property.Object
                };

            var memento = new DeepPropertyMemento(obj, props);
            memento.SavedValue = FinalVal;

            //Act
            memento.Restore();

            //Assert
            property.Verify(p => p.SetValue(link2Val, FinalVal), Times.Once());
        }
    }
}
