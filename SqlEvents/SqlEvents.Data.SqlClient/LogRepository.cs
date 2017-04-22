using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace SqlEvents.Data.SqlClient
{
    public class Log
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }

    public class LogRepository
    {
        private readonly SqlConnection _conn;
        private SqlDependencyEx _listener;

        public LogRepository(SqlConnection conn)
        {
            _conn = conn;
            _listener = new SqlDependencyEx(conn.ConnectionString, conn.Database, "Logs");
            _listener.TableChanged += OnTableChanged;
            _listener.Start();
        }

        private void OnTableChanged(object sender, SqlDependencyEx.TableChangedEventArgs e)
        {
            var r = e.Data.Element("inserted").Element("row");
            var l = new Log()
            {
                Id = r.Element("Id").Value,
                Text = r.Element("Text").Value.TrimEnd(),
            };

            OnOnLogInsert(l);
        }

        public void InsertLog(string log)
        {
            var cmd = new SqlCommand("insert into Logs (Text) values (@log)", _conn);
            cmd.Parameters.AddWithValue("log", log);
            cmd.ExecuteNonQuery();
        }

        public IEnumerable<string> GetLogs()
        {
            var cmd = new SqlCommand("select * from Logs", _conn);
            var r = cmd.ExecuteReader();
            while (r.Read())
            {
                yield return r["Text"].ToString().TrimEnd();
            }
        }

        public event EventHandler<Log> OnLogInsert;

        protected virtual void OnOnLogInsert(Log e)
        {
            OnLogInsert?.Invoke(this, e);
        }
    }
}
