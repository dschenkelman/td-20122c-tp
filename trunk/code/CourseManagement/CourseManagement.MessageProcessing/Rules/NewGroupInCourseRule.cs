namespace CourseManagement.MessageProcessing.Rules
{
    using System;
    using System.IO;
    using System.Linq;
    using Actions;
    using Messages;
    using Persistence.Logging;

    internal class NewGroupInCourseRule : BaseRule
    {
        public NewGroupInCourseRule(IActionFactory actionFactory, ILogger logger)
            : base(actionFactory, logger)
        {
        }

        public override bool IsMatch(IMessage message, bool previouslyMatched)
        {
            if (this.subjectRegex.IsMatch(message.Subject) == false)
            {
                return false;
            }

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

            return onlyOneAttachmentInMessage && attachmentMatchExtensionTxt;
        }
    }
}
