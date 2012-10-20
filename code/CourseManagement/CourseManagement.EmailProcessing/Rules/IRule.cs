namespace CourseManagement.EmailProcessing.Rules
{
    internal interface IRule
    {
        string Name { get; set; }
        
        void RetrieveActions();
    }
}