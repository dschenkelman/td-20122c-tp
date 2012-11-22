using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CourseManagement.Persistence.Configuration
{
    public class ConfigurationService : IConfigurationService
    {
        public string GetValue(string rulesConfigurationFilePathKey)
        {
            return ConfigurationManager.AppSettings[rulesConfigurationFilePathKey];
        }

        public string AttachmentsRootPath
        {
            get { return this.GetValue("AttachmentsRootPath"); }
        }

        public string LogsRootPath
        {
            get { return this.GetValue("LogsRootPath"); }
        }

        public int MonitoredSubjectId
        {
            get { return Convert.ToInt32(this.GetValue("MonitoredSubjectId")); }
        }

        public string IncomingMessageProtocol
        {
            get { return this.GetValue("IncomingMessageProtocol"); }
        }

        public string OutgoingMessageProtocol
        {
            get { return this.GetValue("OutgoingMessageProtocol"); }
        }
    }
}
