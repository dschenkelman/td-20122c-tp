namespace CourseManagement.Utilities.Extensions
{
    using System;
    using System.Collections.Generic;

    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            new ArgumentNullException("items").ThrowIfNull(items);
            new ArgumentNullException("action").ThrowIfNull(action);
            
            foreach (var item in items)
            {
                action(item);
            }
        }
    }
}
