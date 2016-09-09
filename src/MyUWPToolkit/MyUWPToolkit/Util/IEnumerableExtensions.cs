using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUWPToolkit.Util
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {

            foreach (TSource item in source)
            {
                action(item);
            }

            return source;
        }

        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource, int> action)
        {

            int index = 0;
            foreach (TSource item in source)
            {
                action(item, index++);
            }

            return source;
        }

        public static int FindIndex<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {

            int index = 0;
            foreach (TSource item in source)
            {
                if (predicate(item))
                    return index;

                index++;
            }

            return -1;
        }

        public static ObservableCollection<TSource> ToObservableCollection<TSource>(this IEnumerable<TSource> source)
        {

            return new ObservableCollection<TSource>(source);
        }

        public static bool Exists<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            foreach (var item in items)
            {
                if (predicate(item))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
