using MementoContainer.Attributes;

namespace MementoContainer.Mocks
{
    public class SimpleMock
    {
        [MementoProperty]
        public string Property { get; set; }

        public string Field = "";
    }
}
