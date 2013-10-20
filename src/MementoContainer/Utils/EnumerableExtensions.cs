using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoContainer.Utils
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> ForAll<T>(this IEnumerable<T> data, Action<T> action)
        {
            foreach (var item in data)
            {
                action(item);
                yield return item;
            }
        }
    }
}
