namespace CourseManagement.Utilities.Extensions
{
    using System;

    public static class DateTimeExtensions
    {
        public static int Semester(this DateTime date)
        {
            return date.Month <= 6 ? 1 : 2;
        }
    }
}
