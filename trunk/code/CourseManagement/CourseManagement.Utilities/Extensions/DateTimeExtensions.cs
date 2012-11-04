namespace CourseManagement.Utilities.Extensions
{
    using System;

    public static class DateTimeExtensions
    {
        public static int Semester(this DateTime date)
        {
            return date.Month <= 6 ? 1 : 2;
        }

        public static string ToIsoFormat(this DateTime date)
        {
            string year = date.Year.ToString();
            string month = date.Month.ToString();
            string day = date.Day.ToString();

            if (day.Length == 1)
            {
                day = "0" + day;
            }

            if (month.Length == 1)
            {
                month = "0" + month;
            }

            return year + month + day;
        }
    }
}
