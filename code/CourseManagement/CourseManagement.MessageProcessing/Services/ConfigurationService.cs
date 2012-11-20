namespace CourseManagement.MessageProcessing.Services
{
    using System;
    using System.Configuration;

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
