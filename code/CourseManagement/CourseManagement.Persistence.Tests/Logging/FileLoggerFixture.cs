using CourseManagement.Persistence.Logging;

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
        public void ShouldOpenLogFileWhenWritingEntry()
        {
            const string FileName = "Log.txt";

            this.configurationService.Setup(cs => cs.GetValue("LogFile")).Returns(FileName);

            var logger = this.CreateLogger();

            logger.Log(LogLevel.Information, "Message");

            Assert.Fail();
        }

        private FileLogger CreateLogger()
        {
            return new FileLogger(this.configurationService.Object);
        }
    }
}
