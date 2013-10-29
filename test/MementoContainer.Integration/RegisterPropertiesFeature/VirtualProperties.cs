using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterPropertiesFeature
{
    [TestFixture]
    public class VirtualProperties
    {
        [Test]
        public void Test()
        {
            ConcreteArticle concreteArticle = new ConcreteArticle {Title = "Draft"};
            AbstractArticle abstractArticle = new ConcreteArticle {Title = "Draft"};

            var memento = Memento.Create()
                                 .RegisterProperty(concreteArticle, a => a.Title)
                                 .RegisterProperty(abstractArticle, a => a.Title);

            concreteArticle.Title = "Something else";
            abstractArticle.Title = "Something else";

            memento.Rollback();

            Assert.AreEqual("Draft", concreteArticle.Title);
            Assert.AreEqual("Draft", abstractArticle.Title);
        }

        private abstract class AbstractArticle
        {
            public virtual string Title { get; set; }
        }

        private class ConcreteArticle : AbstractArticle
        {
            
        }
    }
}
