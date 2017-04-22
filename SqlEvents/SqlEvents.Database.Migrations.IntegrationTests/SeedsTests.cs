using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlEvents.Data.SqlClient;

namespace SqlEvents.Database.Migrations.IntegrationTests
{
    [TestClass]
    public class SeedsTests
    {
        [TestInitialize]
        public void ResetTable()
        {
            Migrations.ClearTable("Logs", TestConnections.Default.Value);
        }

        [TestMethod]
        public void SeedsLogTable()
        {
            var lr = new LogRepository(TestConnections.Default.Value);

            Seeds.FillLogTable(lr);
            var logs = lr.GetLogs().ToList();

            Assert.AreEqual(5, logs.Count());
        }
    }
}
