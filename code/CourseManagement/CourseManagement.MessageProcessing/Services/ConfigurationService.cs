﻿namespace CourseManagement.MessageProcessing.Services
{
    using System;
    using System.Configuration;

    public class ConfigurationService : IConfigurationService
    {
        public string GetValue(string rulesConfigurationFilePathKey)
        {
            return ConfigurationManager.AppSettings[rulesConfigurationFilePathKey];
        }

        public string RootPath
        {
            get { return this.GetValue("AttachmentsRootPath"); }
        }

        public int MonitoredSubjectId
        {
            get { return Convert.ToInt32(this.GetValue("MonitoredSubjectId")); }
        }

        public string IncomingMessageProtocol
        {
            get { return this.GetValue("IncomingMessageProtocol"); }
        }
    }
}
