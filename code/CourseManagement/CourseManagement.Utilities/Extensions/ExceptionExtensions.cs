using System;

namespace CourseManagement.Utilities.Extensions
{
    public static class ExceptionExtensions
    {
        public static void ThrowIfNull<T>(this Exception e, T item) where T : class
        {
            if (item == null)
            {
                throw e;
            }
        }
    }
}
