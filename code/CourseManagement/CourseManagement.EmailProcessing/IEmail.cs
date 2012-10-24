using CourseManagement.Model;

namespace CourseManagement.EmailProcessing
{
    public interface IEmail
    {
        string EmailSubject { get; set; }
        string Address { get; set; }
    }
}
