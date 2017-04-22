using SqlEvents.Data.SqlClient;

namespace SqlEvents.Database.Migrations
{
    public static class Seeds
    {
        public static void FillLogTable(LogRepository lr)
        {
            lr.InsertLog("test1");
            lr.InsertLog("test2");
            lr.InsertLog("test3");
            lr.InsertLog("test4");
            lr.InsertLog("test5");
        }
    }
}
