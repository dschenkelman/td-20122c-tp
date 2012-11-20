using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CourseManagement.Utilities.Extensions;

namespace CourseManagement.Utilities.Log
{
    public class Logger
    {
        private static Logger _instance;

        public static Logger GetInstance()
        {
            return _instance ?? (_instance = new Logger());
        }

        private Logger(){}

        public void LogInvalidOperation(string rootPath, string subjectId, string ruleName, string errorMessage)
        {
            var directory = Path.Combine(rootPath, subjectId);
            Directory.CreateDirectory(directory);

            StreamWriter writer = File.AppendText(Path.Combine(directory, "Failures.txt"));
            writer.WriteLine("<{0}> InvalidOperationException from Rule: {1} - Error message: {2}",
                             DateTime.Now.Date.ToString("yyyy/M/d hh:mm tt"), ruleName, errorMessage);
            writer.Close();
        }

        public void LogProcessedRule(string rootPath, string subjectId, string ruleName, string from, DateTime date)
        {
            var directory = Path.Combine(rootPath, subjectId);
            Directory.CreateDirectory(directory);

            StreamWriter writer = File.AppendText(Path.Combine(directory, "Processed.txt"));
            writer.WriteLine("<{0}> Rule: {1} has been applied to a message from {2} sended {3}",
                             DateTime.Now.Date.ToString("yyyy/M/d hh:mm tt"), ruleName, from,
                             date.ToString("yyyy/M/d hh:mm tt"));
            writer.Close();
        }
    }
}
