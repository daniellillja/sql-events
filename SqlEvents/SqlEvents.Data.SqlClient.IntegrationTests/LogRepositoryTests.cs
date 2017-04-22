using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlEvents.Database.Migrations;
using SqlEvents.Database.Migrations.IntegrationTests;

namespace SqlEvents.Data.SqlClient.IntegrationTests
{
    [TestClass]
    public class LogRepositoryTests
    {
        [TestInitialize]
        public void ResetTable()
        {
            Migrations.Execute(TestConnections.Default.Value);
            Migrations.ClearTable("Logs", TestConnections.Default.Value);
        }

        [TestMethod]
        public void InsertedLogCanBeRetrievedAndRaisesEvent()
        {
            var ev = new AutoResetEvent(false);
            var sut = new LogRepository(TestConnections.Default.Value);

            sut.OnLogInsert += (sender, log) => ev.Set();
            sut.InsertLog("log");

            var result = sut.GetLogs();
            Assert.IsTrue(result.Any(l => l.Equals("log")));
            Assert.IsTrue(ev.WaitOne(TimeSpan.FromSeconds(2)));
        }
    }
}
