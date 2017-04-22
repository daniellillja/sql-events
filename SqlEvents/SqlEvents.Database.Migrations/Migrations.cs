using System;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SqlEvents.Database.Migrations.Properties;

namespace SqlEvents.Database.Migrations
{
    public static class Migrations
    {
        public static void ResetDatabase(SqlConnection conn)
        {
            DropTable("Logs", conn);
            DropTable("Migrations", conn);

            var cmd = new SqlCommand("select count(*) from sysobjects where xtype = 'U'", conn);
            var tableCount = cmd.ExecuteScalar();
            if ((int)tableCount != 0)
            {
                throw new Exception("failed to clear all tables");
            }
        }

        public static void CreateMigrationsTable(SqlConnection conn)
        {
            if (!TableExists("Migrations", conn))
            {
                ExecuteMigrationScript("_2017_04_22_0_CreateMigrationsTable",
                    Resources._2017_04_22_0_CreateMigrationsTable, conn);
            }
        }

        private static bool ScriptNotPreviouslyExecuted(string name, string sql, SqlConnection conn)
        {
            var cmd = new SqlCommand("select count(Name) from Migrations where Name=@name and Hash=@hash", conn);
            var hash = GetHash(sql);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@hash", hash);
            return (int)cmd.ExecuteScalar() == 0;
        }

        private static void ExecuteMigrationScript(string name, string sql, SqlConnection conn)
        {
            var cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();

            var hash = GetHash(sql);
            var time = DateTime.Now;
            cmd = new SqlCommand(@"insert into Migrations values (@name, @date, @hash)", conn);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@date", time);
            cmd.Parameters.AddWithValue("@hash", hash);
            cmd.ExecuteNonQuery();
        }

        private static string GetHash(string sql)
        {
            using (var md5 = MD5.Create())
            {
                var enc = Encoding.UTF8;
                var hash = md5.ComputeHash(enc.GetBytes(sql));
                return Convert.ToBase64String(hash, 0, 5);
            }
        }

        public static void Execute(SqlConnection conn)
        {
            CreateMigrationsTable(conn);
            CreateLogsTable(conn);
        }

        private static void CreateLogsTable(SqlConnection conn)
        {
            if (ScriptNotPreviouslyExecuted("_2017_04_21_0_CreateLogsTable",
                    Resources._2017_04_21_0_CreateLogsTable, conn))
            {
                ExecuteMigrationScript("_2017_04_21_0_CreateLogsTable",
                    Resources._2017_04_21_0_CreateLogsTable, conn);
            }
        }

        public static void ClearTable(string name, SqlConnection conn)
        {
            var cmd = new SqlCommand($"truncate table {name}", conn);
            cmd.Parameters.AddWithValue("@TableName", name);
            cmd.ExecuteNonQuery();
        }

        public static void DropTable(string name, SqlConnection conn)
        {
            if (TableExists(name, conn))
            {
                var cmd = new SqlCommand($"drop table {name}", conn);
                cmd.Parameters.AddWithValue("@TableName", name);
                cmd.ExecuteNonQuery();
            }
        }

        public static bool TableExists(string name, SqlConnection conn)
        {
            var cmd = new SqlCommand("select count(*) from sysobjects where xtype = 'U' and name=@TableName", conn);
            cmd.Parameters.AddWithValue("@TableName", name);
            var count = (int)cmd.ExecuteScalar();
            return count.Equals(1);
        }
    }
}
