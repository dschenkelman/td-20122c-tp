namespace CourseManagement.MessageProcessing.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Messages;

    public class GroupFileParser : IGroupFileParser
    {
        public IEnumerable<int> GetIdsFromMessage(IMessage message)
        {
            return ParseIdsFromAttachmentInMessage(message);
        }

        private static IEnumerable<int> ParseIdsFromAttachmentInMessage(IMessage message)
        {
            if (message.Attachments.Count() != 1)
            {
                throw new Exception("Message for Rule AddGroup doesn't have the unique attachment requirement");
            }

            var lines = message.Attachments.First().RetrieveLines();

            return lines.Select(line => Convert.ToInt32(line)).ToList();
        }
    }
}
