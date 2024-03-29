﻿namespace CourseManagement.Web.Helpers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    public static class Helpers
    {
        public static SelectList ToSelectList<TEnum>(this TEnum enumObj)
        {
          var values = from TEnum e in Enum.GetValues(typeof(TEnum))
                       select new { Id = e, Name = e.ToString() };

          return new SelectList(values, "Id", "Name", enumObj);
        }
    }
}