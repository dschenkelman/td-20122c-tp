namespace CourseManagement.Persistence.Tests
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using Initialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class CompositeDatabaseInitializerFixture
    {
        private MockRepository mockRepository;

        private List<IDatabaseInitializer<DbContext>> initializerCallList;

        private Mock<DbContext> dataContext;

        [TestInitialize]
        public void TestInitialize()
        {
            // loose to avoid error from moq when mocking DbContext
            this.mockRepository = new MockRepository(MockBehavior.Loose);
            this.initializerCallList = new List<IDatabaseInitializer<DbContext>>();
            this.dataContext = this.mockRepository.Create<DbContext>();
        }

        [TestMethod]
        public void ShouldInvokeInitializeDatabaseForAllInitializersInSpecifiedOrder()
        {
            var initializer1 = this.SetupMockInitializer();
            var initializer2 = this.SetupMockInitializer();
            var initializer3 = this.SetupMockInitializer();
            var initializer4 = this.SetupMockInitializer();

            var compositeInitializer = this.CreateCompositeInitializer(initializer1.Object, initializer2.Object);

            compositeInitializer.AddInitializer(initializer3.Object)
                .AddInitializer(initializer4.Object);

            Assert.AreEqual(0, this.initializerCallList.Count);

            compositeInitializer.InitializeDatabase(this.dataContext.Object);

            Assert.AreEqual(4, this.initializerCallList.Count);
            Assert.AreEqual(initializer1.Object, this.initializerCallList[0]);
            Assert.AreEqual(initializer2.Object, this.initializerCallList[1]);
            Assert.AreEqual(initializer3.Object, this.initializerCallList[2]);
            Assert.AreEqual(initializer4.Object, this.initializerCallList[3]);

            initializer1.Verify(i => i.InitializeDatabase(this.dataContext.Object), Times.Once());
            initializer2.Verify(i => i.InitializeDatabase(this.dataContext.Object), Times.Once());
            initializer3.Verify(i => i.InitializeDatabase(this.dataContext.Object), Times.Once());
            initializer4.Verify(i => i.InitializeDatabase(this.dataContext.Object), Times.Once());
        }

        [TestMethod]
        public void ShouldNotInvokeInitializeDatabaseForRemovedInitializersInSpecifiedOrder()
        {
            var initializer1 = this.SetupMockInitializer();
            var initializer2 = this.SetupMockInitializer();
            var initializer3 = this.SetupMockInitializer();
            var initializer4 = this.SetupMockInitializer();

            var compositeInitializer = this.CreateCompositeInitializer(initializer1.Object, initializer2.Object);

            compositeInitializer.AddInitializer(initializer3.Object)
                .RemoveInitializer(initializer1.Object)
                .RemoveInitializer(initializer3.Object)
                .AddInitializer(initializer4.Object);

            Assert.AreEqual(0, this.initializerCallList.Count);

            compositeInitializer.InitializeDatabase(this.dataContext.Object);

            Assert.AreEqual(2, this.initializerCallList.Count);
            Assert.AreEqual(initializer2.Object, this.initializerCallList[0]);
            Assert.AreEqual(initializer4.Object, this.initializerCallList[1]);

            initializer1.Verify(i => i.InitializeDatabase(this.dataContext.Object), Times.Never());
            initializer2.Verify(i => i.InitializeDatabase(this.dataContext.Object), Times.Once());
            initializer3.Verify(i => i.InitializeDatabase(this.dataContext.Object), Times.Never());
            initializer4.Verify(i => i.InitializeDatabase(this.dataContext.Object), Times.Once());
        }

        private Mock<IDatabaseInitializer<DbContext>> SetupMockInitializer()
        {
            var initializer = this.mockRepository.Create<IDatabaseInitializer<DbContext>>();
            initializer.Setup(i => i.InitializeDatabase(this.dataContext.Object)).Callback<DbContext>(
                c => this.initializerCallList.Add(initializer.Object)).Verifiable();

            return initializer;
        }

        private CompositeDatabaseInitializer<DbContext> CreateCompositeInitializer(params IDatabaseInitializer<DbContext>[] databaseInitializers)
        {
            return new CompositeDatabaseInitializer<DbContext>(databaseInitializers);
        }
    }
}