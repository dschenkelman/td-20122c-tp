using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseManagement.Persistence.Configuration
{
    public interface IConfigurationService
    {
        string GetValue(string logFilePathKey);

        string AttachmentsRootPath { get; }

        string LogsRootPath { get; }

        int MonitoredSubjectId { get; }
        
        string IncomingMessageProtocol { get; }

        string OutgoingMessageProtocol { get; }
    }
}
