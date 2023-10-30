using System;
using System.Collections.Generic;
using System.Text;

namespace TempDynamic.Extensions.Lists
{
    public static class ListExtensions
    {
        public static void AddRange<T>(this IList<T> list, T[] array)
        {
            if (null == list || null == array)
                return;

            Array.ForEach(array, (x) =>
            {
                list.Add(x);
            });
        }
    }
}
