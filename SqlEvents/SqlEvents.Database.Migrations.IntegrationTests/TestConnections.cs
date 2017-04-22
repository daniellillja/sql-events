using System;
using System.Data.SqlClient;

namespace SqlEvents.Database.Migrations.IntegrationTests
{
    public static class TestConnections
    {
        public static Lazy<SqlConnection> Default = new Lazy<SqlConnection>(CreateDefaultConnection);

        private static SqlConnection CreateDefaultConnection()
        {
            string cs = "Data Source=.;Initial Catalog=SqlEvents;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=False;TrustServerCertificate=True";
            var c = new SqlConnection(cs);
            c.Open();
            return c;
        }
    }
}
