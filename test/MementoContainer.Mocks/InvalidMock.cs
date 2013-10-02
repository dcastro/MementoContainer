using System;
using MementoContainer.Attributes;

namespace MementoContainer.Mocks
{
    public class InvalidMock
    {
        private readonly string _field = String.Empty;
        private static readonly string StaticField = String.Empty;

        [MementoProperty]
        private string GetOnlyProperty
        {
            get { return _field; }
        }

        public static string StaticGetOnlyProperty
        {
            get { return StaticField; }
        }
    }
}
