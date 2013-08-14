using MementoContainer.Attributes;

namespace MementoContainer.Mocks
{
    public class AnnotatedMock
    {
        public string NonAnnotatedProperty { get; set; }

        [MementoProperty]
        public SimpleMock NestedProperty { get; set; }

        [MementoProperty]
        public string PublicProperty { get; set; }

        [MementoProperty]
        private string PrivateProperty { get; set; }

        [MementoProperty]
        private static string StaticProperty { get; set; }

        public void SetPrivate(string val)
        {
            PrivateProperty = val;
        }

        public string GetPrivate()
        {
            return PrivateProperty;
        }

        public void SetStatic(string val)
        {
            StaticProperty = val;
        }

        public string GetStatic()
        {
            return StaticProperty;
        }
    }
}
