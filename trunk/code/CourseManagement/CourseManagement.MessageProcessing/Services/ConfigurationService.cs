﻿using System;
using System.Configuration;

namespace CourseManagement.MessageProcessing.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public string GetValue(string rulesConfigurationFilePathKey)
        {
            return ConfigurationManager.AppSettings[rulesConfigurationFilePathKey];
        }

        public string RootPath
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int MonitoringCourseSubjectId
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string MonitoringCourseIncomingMessageProtocol
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}