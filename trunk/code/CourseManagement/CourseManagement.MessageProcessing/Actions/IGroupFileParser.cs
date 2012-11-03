using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CourseManagement.Messages;

namespace CourseManagement.MessageProcessing.Actions
{
    public interface IGroupFileParser
    {
        IEnumerable<int> ObtainIdsFromMessage(IMessage message);
    }
}
