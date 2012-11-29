using System;
using System.IO.Moles;
using CourseManagement.Persistence.Logging;
using CourseManagement.Utilities.Extensions;

namespace CourseManagement.Persistence.Tests.Logging
{
    using Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class FileLoggerFixture
    {
        private MockRepository mockRepository;
        private Mock<IConfigurationService> configurationService;

        [TestInitialize]
        public void Initialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.configurationService = this.mockRepository.Create<IConfigurationService>();
        }

        [TestMethod]
        public void ShouldRetrieveLogFileNameFromConfiguration()
        {
            const string FileName = "Log.txt";

            this.configurationService.Setup(cs => cs.GetValue("LogFile")).Returns(FileName).Verifiable();

            this.CreateLogger();

            this.configurationService.Verify(cs => cs.GetValue("LogFile"), Times.Once());
        }

        [TestMethod]
        [HostType("Moles")]
        public void ShouldOpenLogFileWhenWritingEntry()
        {
            const string FileName = "Log.txt";

            this.configurationService.Setup(cs => cs.GetValue("LogFile")).Returns(FileName);

            var logger = this.CreateLogger();

            bool appendTextInvoked = false;

            MFile.AppendTextString = fileName =>
            {
                appendTextInvoked = true;
                Assert.AreEqual(FileName, fileName);
                return new MStreamWriter();
            };

            MTextWriter.AllInstances.WriteLineString = (tw, lineContent) =>
            {
            };

            MTextWriter.AllInstances.Dispose = tw => { };

            logger.Log(LogLevel.Information, "Message");

            Assert.IsTrue(appendTextInvoked);
        }

        [TestMethod]
        [HostType("Moles")]
        public void ShouldWriteToFileWhenLogging()
        {
            const string FileName = "Log.txt";

            this.configurationService.Setup(cs => cs.GetValue("LogFile")).Returns(FileName);

            var logger = this.CreateLogger();

            var streamWriter = new MStreamWriter();

            MFile.AppendTextString = fileName =>
            {
                return streamWriter;
            };

            bool writeLineInvoked = false;

            // we should probably mock DateTime.Now, but it is overkill
            MTextWriter.AllInstances.WriteLineString = (tw, lineContent) =>
                                                           {
                                                               writeLineInvoked = true;
                                                               Assert.AreEqual(
                                                                   string.Format("{0};Information;Message", DateTime.Now.ToIsoFormat()), lineContent);
                                                           };

            MTextWriter.AllInstances.Dispose = tw => { };

            logger.Log(LogLevel.Information, "Message");

            Assert.IsTrue(writeLineInvoked);
        }

        private FileLogger CreateLogger()
        {
            return new FileLogger(this.configurationService.Object);
        }
    }
}
