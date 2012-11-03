using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CourseManagement.MessageProcessing.Actions;
using CourseManagement.Messages;

namespace CourseManagement.MessageProcessing.Rules
{
    class NewGroupInCourseRule : BaseRule
    {
        private IActionFactory actionFactory;

        private const string MatchingSubject = "[ALTA-GRUPO]";

        public NewGroupInCourseRule(IActionFactory actionFactory) : base(actionFactory)
        {
            this.actionFactory = actionFactory;
        }

        public override bool IsMatch(IMessage message)
        {
            var subjectMatch = message.Subject.Equals(MatchingSubject);

            var onlyOneAttachmentInMessage = (message.Attachments.Count() == 1);

            var attachmentMatchExtensionTxt = false;

            if ( onlyOneAttachmentInMessage )
            {
                var attachmentName = message.Attachments.First().Name;

                var extension = "";
                
                try
                {
                    extension = Path.GetExtension(attachmentName);
                    if ( extension != null)
                    {
                        attachmentMatchExtensionTxt = extension.Equals(".txt");    
                    }
                }catch (Exception)
                {

                }
                
            }
            return (subjectMatch && onlyOneAttachmentInMessage && attachmentMatchExtensionTxt);
        }
    }
}
