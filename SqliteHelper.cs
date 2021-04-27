using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBServerMonitor
{
    public class SqliteHelper
    {
        private string db;

        public SqliteHelper(string db)
        {
            this.db = db;
        }

        private SQLiteConnection createConnection()
        {
            var connection = new SQLiteConnection("Data Source=" + db + "");
            connection.Open();
            return connection;
        }

        public void ExecuteSql(string sql)
        {
            var conn = createConnection();
            try
            {
                SQLiteCommand command = new SQLiteCommand(conn);
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
            catch
            {
            }
            finally
            {
                conn.Close();
            }
        }

        public SQLiteDataReader ExecuteSqlReader(string sql)
        {
            var conn = createConnection();
            SQLiteCommand command = new SQLiteCommand(conn);
            command.CommandText = sql;
            var reader = command.ExecuteReader();
            conn.Close();
            return reader;
        }
    }
}
