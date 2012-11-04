﻿using System;

namespace CourseManagement.Console
{
    using System.Data.Entity;
    using System.Linq;
    using MessageProcessing;
    using Persistence;
    using Persistence.Initialization;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.Configuration;

    class Program
    {
        static void Main(string[] args)
        {
            SetupDatabase();

            IUnityContainer container = new UnityContainer();

            // Initialize the container with the config file
            container.LoadConfiguration();

            var messageProcessor = container.Resolve<MessageProcessor>();

            do
            {
                messageProcessor.Process();
                System.Console.WriteLine("Do you want to continue processing? (press N to quit)");
            } 
            while (System.Console.ReadKey().Key != ConsoleKey.N);
        }

        private static void SetupDatabase()
        {
            CompositeDatabaseInitializer<CourseManagementContext> compositeDatabaseInitializer = 
                new CompositeDatabaseInitializer<CourseManagementContext>(
                    new DropCreateDatabaseIfModelChanges<CourseManagementContext>());

            const string PathToSqlScript = @"Scripts\DbSetup.sql";
            compositeDatabaseInitializer.AddInitializer(new ScriptDataInitializer(PathToSqlScript));

            Database.SetInitializer(compositeDatabaseInitializer);

            // creates the DB));
            using (var db = new CourseManagementContext())
            {
                var s = db.Subjects.FirstOrDefault();
            }
        }
    }
}
