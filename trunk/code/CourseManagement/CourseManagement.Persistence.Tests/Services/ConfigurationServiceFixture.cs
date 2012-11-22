namespace CourseManagement.Persistence.Tests.Services
{
    using System.Collections.Specialized;
    using System.Configuration.Moles;
    using Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConfigurationServiceFixture
    {
        [TestMethod]
        [HostType("Moles")]
        public void ShouldGetValuesThroughConfigurationManager()
        {
            // arrange
            ConfigurationService configurationService = this.CreateConfigurationService();

            var collection = new NameValueCollection();

            collection.Add("Key1", "Value1");
            collection.Add("Key2", "Value2");
            collection.Add("Key3", "Value3");

            MConfigurationManager.AppSettingsGet = () => collection;

            // act
            var value1 = configurationService.GetValue("Key1");
            var value2 = configurationService.GetValue("Key2");
            var value3 = configurationService.GetValue("Key3");

            // assert
            Assert.AreEqual("Value1", value1);
            Assert.AreEqual("Value2", value2);
            Assert.AreEqual("Value3", value3);
        }

        private ConfigurationService CreateConfigurationService()
        {
            return new ConfigurationService();
        }
    }
}