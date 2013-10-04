using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MementoContainer.Integration.RegisterPropertiesFeature
{
    [TestFixture]
    public class ConvertExpressions
    {
        [Test]
        public void TestExplicitConversion()
        {
            var article = new Article
                {
                    Author = new Person
                        {
                            Name = "DCastro"
                        }
                };

            var memento = Memento.Create()
                                 .RegisterProperty(article, a => ((Person)a.Author).Name);

            ((Person)article.Author).Name = "No one";

            memento.Restore();

            Assert.AreEqual("DCastro", ((Person)article.Author).Name);
        }

        private class Article
        {
            public IPerson Author { get; set; }
        }

        private interface IPerson
        {

        }

        private class Person : IPerson
        {
            public string Name { get; set; }
        }
    }
}
