using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SqlEvents.Database.Migrations.IntegrationTests
{
    [TestClass]
    public class MigrationsTests
    {
        [TestMethod]
        public void ResettingDatabaseRemovesSchema()
        {
            Migrations.ResetDatabase(TestConnections.Default.Value);
        }

        [TestMethod]
        public void ResettingAndMigrating()
        {
            var conn = TestConnections.Default.Value;
            Migrations.ResetDatabase(TestConnections.Default.Value);
            Migrations.Execute(conn);

            Assert.IsTrue(Migrations.TableExists("Migrations", conn));
            Assert.IsTrue(Migrations.TableExists("Logs", conn));
        }
    }
}
