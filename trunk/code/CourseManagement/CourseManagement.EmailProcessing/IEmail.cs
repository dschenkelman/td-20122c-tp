using System;

namespace CourseManagement.EmailProcessing
{
    public interface IEmail
    {
        string EmailSubject { get; set; }
        string Address { get; set; }
        DateTime Date { get; set; }
    }
}
