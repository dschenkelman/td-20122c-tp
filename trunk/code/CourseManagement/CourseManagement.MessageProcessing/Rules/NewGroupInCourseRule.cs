namespace CourseManagement.MessageProcessing.Rules
{
    using System;
    using System.IO;
    using System.Linq;
    using Actions;
    using Messages;

    internal class NewGroupInCourseRule : BaseRule
    {
        private const string MatchingSubject = "[ALTA-GRUPO]";

        public NewGroupInCourseRule(IActionFactory actionFactory) : base(actionFactory)
        {
        }

        public override bool IsMatch(IMessage message, bool previouslyMatched)
        {
            var subjectMatch = message.Subject.Equals(MatchingSubject);

            var onlyOneAttachmentInMessage = message.Attachments.Count() == 1;

            var attachmentMatchExtensionTxt = false;

            if (onlyOneAttachmentInMessage)
            {
                var attachmentName = message.Attachments.First().Name;

                string extension;

                try
                {
                    extension = Path.GetExtension(attachmentName);
                    if (extension != null)
                    {
                        attachmentMatchExtensionTxt = extension.Equals(".txt");
                    }
                }
                catch (Exception)
                {
                }
            }

            return subjectMatch && onlyOneAttachmentInMessage && attachmentMatchExtensionTxt;
        }
    }
}
